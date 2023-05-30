using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    // カーソルの位置を取得する関数を呼ぶ
    [SerializeField] Cursor cursor;

    // マップ生成関数を呼ぶ
    [SerializeField] MapGenerator mapGenerator;

    // 
    [SerializeField] CalcMoveRange calcMoveRange;

    // 生成したマップを管理
    TileObj[,] tileObjs;

    private void Start()
    {
        // マップを生成
        tileObjs = mapGenerator.Generator();
    }

    // クリックしたタイルを取得する
    public TileObj GetClickTileObj()
    {
        Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.Raycast(clickPosition, Vector2.down);

        // Rayを飛ばしてヒットしたタイルを取得
        if (hit2D && hit2D.collider)
        {
            cursor.SetPosition(hit2D.transform);
            return hit2D.collider.GetComponent<TileObj>();
        }
        return null;
    }

    public TileObj GetTileOn(Character character)
    {
        for (int i = 0; i < tileObjs.GetLength(0); i++)
        {
            for (int j = 0; j < tileObjs.GetLength(1); j++)
            {
                //Debug.Log($"tileObjs[i, j].positionInt+{ tileObjs[i, j].positionInt}");
                //Debug.Log($"character.Position+{character.Position}");  // エラー

                if (tileObjs[i, j].positionInt == character.Position)
                {
                    return tileObjs[i, j];
                }
            }
        }
        return null;
    }

    // 移動範囲を表示する
    public void ShowMovablePanels(Character character,List<TileObj> movableTiles)
    {
        // エラーの原因:PlayerのPositionを入れているから、マイナスを入れてしまう
        // 何番目のタイルなのか(index)が入る

        // characterが乗っているタイルのIndexを取得する
        Vector2Int index = GetTileOn(character).Index;
        calcMoveRange.SetMoveCost(tileObjs,true);
        int[,] result = calcMoveRange.StartSearch(index.x, index.y, character.MoveRange);

        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
                // 0以上なら移動範囲として追加する
                if(result[i,j] >= 0)
                {
                    movableTiles.Add(tileObjs[i, j]);
                }
            }
        }

        //// characterから上下左右のタイルを探す
        //    movableTiles.Add(tileObjs.Find(tile => tile.positionInt == character.Position));
        //    movableTiles.Add(tileObjs.Find(tile => tile.positionInt == character.Position+Vector2Int.up));
        //    movableTiles.Add(tileObjs.Find(tile => tile.positionInt == character.Position+Vector2Int.down));
        //    movableTiles.Add(tileObjs.Find(tile => tile.positionInt == character.Position+Vector2Int.left));
        //    movableTiles.Add(tileObjs.Find(tile => tile.positionInt == character.Position+Vector2Int.right));

        movableTiles.RemoveAll(tile => tile == null);
        foreach (var tile in movableTiles)
        {
            tile.ShowMovablePanel(true);
        }
    }

    // 移動範囲をリセットする
    public void ResetMovablePanels(List<TileObj> movableTiles)
    {
        foreach (var tile in movableTiles)
        {
            tile.ShowMovablePanel(false);
        }
        movableTiles.Clear();
    }

    //攻撃範囲を表示する
    public void ShowAttackablePanels(Character character, List<TileObj> tiles)
    {
        // characterが乗っているタイルのIndexを取得する
        Vector2Int index = GetTileOn(character).Index;
        calcMoveRange.SetMoveCost(tileObjs, false);
        int[,] result = calcMoveRange.StartSearch(index.x, index.y, character.AttackRange);

        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
                // 0以上なら攻撃範囲として追加する
                if (result[i, j] >= 0)
                {
                    tiles.Add(tileObjs[i, j]);
                }
            }
        }

        //// characterから上下左右のタイルを探す  // マップの外はバグあり
        //TileObj currentTile = GetTileOn(character);
        //tiles.Add(tileObjs[currentTile.Index.x, currentTile.Index.y+1]);
        //tiles.Add(tileObjs[currentTile.Index.x, currentTile.Index.y-1]);
        //tiles.Add(tileObjs[currentTile.Index.x+1, currentTile.Index.y]);
        //tiles.Add(tileObjs[currentTile.Index.x-1, currentTile.Index.y]);

        tiles.RemoveAll(tile => tile == null);
        foreach (var tile in tiles)
        {
            // 攻撃用に表示
            tile.ShowAttackablePanel(true);
        }
    }

    // 攻撃範囲をリセットする
    public void ResetAttackablePanels(List<TileObj> tiles)
    {
        foreach (var tile in tiles)
        {
            tile.ShowAttackablePanel(false);
        }
        tiles.Clear();
    }

    public List<TileObj> GetRoot(Character character,TileObj goalTile)
    {
        return calcMoveRange.GetRoot(GetTileOn(character).Index, goalTile.Index, tileObjs);
    }
}
