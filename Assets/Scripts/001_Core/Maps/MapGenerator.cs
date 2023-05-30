using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// マップ生成
public class MapGenerator : MonoBehaviour
{
    [SerializeField] TileObj GrassObjPrefab;
    [SerializeField] TileObj ForestObjPrefab;
    [SerializeField] TileObj WaterObjPrefab;
    [SerializeField] Transform tileParent;
    [SerializeField] CharactersManager charactersManager;

    public const int WIDTH = 15;
    public const int HEIGHT = 9;
    int WATER_RATE = 10;
    int FOREST_RATE = 30;

    TileObj[,] tileObjs;

    private void Start()
    {
        tileObjs = new TileObj[WIDTH, HEIGHT];
    }

    public TileObj[,] Generator()
    {
        Vector2 offset = new Vector2(-WIDTH / 2, -HEIGHT / 2);
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                // 移動コスト
                // 平原：-1
                // 森  ：-2
                // 水  ：-99

                Vector2 pos = new Vector2(x, y)+offset;
                int rate = Random.Range(0, 100);
                TileObj tileObj = null;

                Character character = charactersManager.GetCharacter(pos);

                if(_Map())
                {
                    tileObj = Instantiate(GrassObjPrefab, pos, Quaternion.identity, tileParent);
                    tileObj.SetCost(-1);
                }
                else if(character != null)
                {
                    tileObj = Instantiate(GrassObjPrefab, pos, Quaternion.identity, tileParent);
                    tileObj.SetCost(-1);
                }
                else if(rate < WATER_RATE)
                {
                    tileObj=Instantiate(WaterObjPrefab, pos, Quaternion.identity, tileParent);
                    tileObj.SetCost(-99);
                }
                else if(rate < FOREST_RATE)
                {
                    tileObj=Instantiate(ForestObjPrefab, pos, Quaternion.identity, tileParent);
                    tileObj.SetCost(-2);
                }
                else
                {
                    tileObj=Instantiate(GrassObjPrefab, pos, Quaternion.identity, tileParent);
                    tileObj.SetCost(-1);
                }
                TileObj n = tileObj.GetComponent<TileObj>();
                if (n != null)
                {
                    n.x = x;
                    n.y = y;
                }

                tileObj.positionInt = new Vector2Int((int)pos.x,(int)pos.y);
                tileObjs[x,y] = tileObj;
                tileObj.SetIndex(x,y);
            }
        }
        return tileObjs;
    }

    public bool _Map()    // Map内判定
    {
        TileObj n = GetComponent<TileObj>();
        if (n == null) return false;

        int x = n.x;
        int y = n.y;
        // Nullだった場合戻る
        tileObjs[x, y] = null;

        if (x - 1 >= 1 && tileObjs[x - 1, y] != null)
        {
            return true;
        }
        if (x + 1 < WIDTH-1 && tileObjs[x + 1, y] != null)
        {
            return true;
        }
        if (y - 1 >= 1 && tileObjs[x, y - 1] != null)
        {
            return true;
        }
        if (y + 1 < HEIGHT-1 && tileObjs[x, y + 1] != null)
        {
            return true;
        }

        return false;
    }

    // キャラのマスは平原にする
    // 端は平原にする
}
