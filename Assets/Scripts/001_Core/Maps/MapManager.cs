using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    // �J�[�\���̈ʒu���擾����֐����Ă�
    [SerializeField] Cursor cursor;

    // �}�b�v�����֐����Ă�
    [SerializeField] MapGenerator mapGenerator;

    // 
    [SerializeField] CalcMoveRange calcMoveRange;

    // ���������}�b�v���Ǘ�
    TileObj[,] tileObjs;

    private void Start()
    {
        // �}�b�v�𐶐�
        tileObjs = mapGenerator.Generator();
    }

    // �N���b�N�����^�C�����擾����
    public TileObj GetClickTileObj()
    {
        Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.Raycast(clickPosition, Vector2.down);

        // Ray���΂��ăq�b�g�����^�C�����擾
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
                //Debug.Log($"character.Position+{character.Position}");  // �G���[

                if (tileObjs[i, j].positionInt == character.Position)
                {
                    return tileObjs[i, j];
                }
            }
        }
        return null;
    }

    // �ړ��͈͂�\������
    public void ShowMovablePanels(Character character,List<TileObj> movableTiles)
    {
        // �G���[�̌���:Player��Position�����Ă��邩��A�}�C�i�X�����Ă��܂�
        // ���Ԗڂ̃^�C���Ȃ̂�(index)������

        // character������Ă���^�C����Index���擾����
        Vector2Int index = GetTileOn(character).Index;
        calcMoveRange.SetMoveCost(tileObjs,true);
        int[,] result = calcMoveRange.StartSearch(index.x, index.y, character.MoveRange);

        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
                // 0�ȏ�Ȃ�ړ��͈͂Ƃ��Ēǉ�����
                if(result[i,j] >= 0)
                {
                    movableTiles.Add(tileObjs[i, j]);
                }
            }
        }

        //// character����㉺���E�̃^�C����T��
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

    // �ړ��͈͂����Z�b�g����
    public void ResetMovablePanels(List<TileObj> movableTiles)
    {
        foreach (var tile in movableTiles)
        {
            tile.ShowMovablePanel(false);
        }
        movableTiles.Clear();
    }

    //�U���͈͂�\������
    public void ShowAttackablePanels(Character character, List<TileObj> tiles)
    {
        // character������Ă���^�C����Index���擾����
        Vector2Int index = GetTileOn(character).Index;
        calcMoveRange.SetMoveCost(tileObjs, false);
        int[,] result = calcMoveRange.StartSearch(index.x, index.y, character.AttackRange);

        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
                // 0�ȏ�Ȃ�U���͈͂Ƃ��Ēǉ�����
                if (result[i, j] >= 0)
                {
                    tiles.Add(tileObjs[i, j]);
                }
            }
        }

        //// character����㉺���E�̃^�C����T��  // �}�b�v�̊O�̓o�O����
        //TileObj currentTile = GetTileOn(character);
        //tiles.Add(tileObjs[currentTile.Index.x, currentTile.Index.y+1]);
        //tiles.Add(tileObjs[currentTile.Index.x, currentTile.Index.y-1]);
        //tiles.Add(tileObjs[currentTile.Index.x+1, currentTile.Index.y]);
        //tiles.Add(tileObjs[currentTile.Index.x-1, currentTile.Index.y]);

        tiles.RemoveAll(tile => tile == null);
        foreach (var tile in tiles)
        {
            // �U���p�ɕ\��
            tile.ShowAttackablePanel(true);
        }
    }

    // �U���͈͂����Z�b�g����
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
