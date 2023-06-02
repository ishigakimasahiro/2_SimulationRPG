using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalcMoveRange : MonoBehaviour
{
    // 移動コストのマップデータ
    int[,] _originalMapList=new int[MapGenerator.WIDTH, MapGenerator.HEIGHT];
    // 移動計算結果のデータ格納用
    int[,] _resultMoveRangeList = new int[MapGenerator.WIDTH, MapGenerator.HEIGHT];

    // マップ上のx,z位置
    int _x;
    int _z;
    // 移動力
    int _m;

    // マップの大きさ
    int _xLength = MapGenerator.WIDTH;
    int _zLength = MapGenerator.HEIGHT;

    [SerializeField] CharactersManager charactersManager;

    public void SetMoveCost(TileObj[,] tileObjs,bool attack)
    {
        // 移動コストのマップデータ(_originalMapList)を作成 (本来はここでは作成せず外部から渡す)
        for (int i = 0; i < _xLength; i++)
        {
            for (int j = 0; j < _zLength; j++)
            {
                Character npc = charactersManager.GetCharacter(tileObjs[i, j].transform.position);
                if(npc && attack)
                {
                    _originalMapList[i, j] = -99;
                }
                else
                {
                    _originalMapList[i, j] = tileObjs[i, j].Cost;
                }
            }
        }
    }

    void Copy()
    {
        for (int i = 0; i < _xLength; i++)
        {
            for (int j = 0; j < _zLength; j++)
            {
                _resultMoveRangeList[i, j] = _originalMapList[i, j];
            }
        }
    }

    /// <summary>
    /// 探索開始
    /// 計算結果のマップデータを返す
    /// </summary>
    public int[,] StartSearch(int currentX, int currentZ, int movePower)
    {
        // _originalMapListのコピー作成
        Copy();

        _xLength = _resultMoveRangeList.GetLength(0);
        _zLength = _resultMoveRangeList.GetLength(1);

        _x = currentX;
        _z = currentZ;
        _m = movePower;

        //Debug.Log($"_x:{_x},_z:{_z} : _xLength:{_xLength},_zLength{_zLength}");
        // 現在位置に現在の移動力を代入
        _resultMoveRangeList[_x,_z] = _m;
        Search4(_x, _z, _m);

        return _resultMoveRangeList;
    }

    /// <summary>
    /// 移動可能な範囲の4方向を調べる
    /// </summary>
    void Search4(int x, int z, int m)
    {

        if (0 <= x && x < _xLength && 0 <= z && z < _zLength)
        {
            // 上方向
            Search(x, z - 1, m);
            // 下方向
            Search(x, z + 1, m);
            // 左方向
            Search(x - 1, z, m);
            // 右方向
            Search(x + 1, z, m);
        }
    }

    /// <summary>
    /// 移動先のセルの調査
    /// </summary>
    void Search(int x, int z, int m)
    {
        // 探索方向のCellがマップエリア領域内かチェック
        if (x < 0 || _xLength <= x) return;
        if (z < 0 || _zLength <= z) return;

        // すでに計算済みのCellかチェック
        if ((m - 1) <= _resultMoveRangeList[x,z]) return;

        // 現在の移動可能量 + 地形コスト
        m = m + _originalMapList[x,z];

        if (m > 0)
        {
            // 進んだ位置に現在の移動力を代入
            _resultMoveRangeList[x,z] = m;
            // 移動量があるのでSearch4を再帰呼びだし
            Search4(x, z, m);
        }
        else
        {
            m = 0;
        }
    }

    // 経路探索
    // 現在の位置から、クリックした場所までのマスを取得したい。
    // クリックしたマス(ゴールマス)から移動コストを戻して、逆順で経路を出す
    public List<TileObj> GetRoot(Vector2Int startIndex,Vector2Int goalIndex,TileObj[,] tileObjs)
    {
        List<TileObj> root = new List<TileObj>();

        root.Add(tileObjs[goalIndex.x, goalIndex.y]);

        Search4Root(root,startIndex,goalIndex,tileObjs);
        root.Reverse();
        return root;
    }

    // 上下左右で、移動コストが一致するものがあればrootに追加して、startIndexと一致するまで調べる
    void Search4Root(List<TileObj> root,Vector2Int startIndex, Vector2Int searchIndex, TileObj[,] tileObjs)
    {
        if(startIndex == searchIndex)
        {
            // 調査終了
            return;
        }
        // 移動コストを戻して一致するものがあればrootに追加してその場所を調べる
        // 現在の移動コスト
        int currentMovePower = _resultMoveRangeList[searchIndex.x, searchIndex.y];
        // 移動前の移動コストに戻す
        currentMovePower = currentMovePower - _originalMapList[searchIndex.x, searchIndex.y];

        Vector2Int[] arround =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
        };
        for (int i = 0; i < arround.Length; i++)
        {
            Vector2Int arroundIndex = searchIndex + arround[i];

            if(IsMatch(currentMovePower,arroundIndex))
            {
                // rootに追加
                root.Add(tileObjs[arroundIndex.x, arroundIndex.y]);
                // さらに、その場所について調べる
                Search4Root(root, startIndex, arroundIndex, tileObjs);
                break;
            }
        }
    }

    // 範囲外エラー解消
    bool IsMatch(int currentMovePower,Vector2Int arroundIndex)
    {
        // 範囲外ならfalse
        if (arroundIndex.x < 0 || arroundIndex.x >= _resultMoveRangeList.GetLength(0))
        {
            return false;
        }
        if (arroundIndex.y < 0 || arroundIndex.y >= _resultMoveRangeList.GetLength(1))
        {
            return false;
        }

        // 一致するならtrue
        if (currentMovePower == _resultMoveRangeList[arroundIndex.x, arroundIndex.y])
        {
            return true;
        }

        return false;
    }

    // ・バグの修正
    // 〇・経路探索で変な動きをするバグの修正 => breakを入れる
    
    // 〇・初期の生成位置について
    // => 生成されるなら平原の上にする
    // => キャラの位置を取得して、その場合マスは平原にする

    // 〇・キャラを通過できないようにする
    // => キャラがいる場所をコスト-99にしてやる
    // => 注意:キャラは移動するので毎回書き換え

    // ・全てのキャラの移動が終わってからターンを終了する
    //   ・ターン終了のボタンを押したら相手のターンになる
    // ・敵の攻撃の実装
}
