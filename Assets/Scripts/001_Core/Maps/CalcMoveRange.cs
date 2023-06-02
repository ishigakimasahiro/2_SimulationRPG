using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalcMoveRange : MonoBehaviour
{
    // �ړ��R�X�g�̃}�b�v�f�[�^
    int[,] _originalMapList=new int[MapGenerator.WIDTH, MapGenerator.HEIGHT];
    // �ړ��v�Z���ʂ̃f�[�^�i�[�p
    int[,] _resultMoveRangeList = new int[MapGenerator.WIDTH, MapGenerator.HEIGHT];

    // �}�b�v���x,z�ʒu
    int _x;
    int _z;
    // �ړ���
    int _m;

    // �}�b�v�̑傫��
    int _xLength = MapGenerator.WIDTH;
    int _zLength = MapGenerator.HEIGHT;

    [SerializeField] CharactersManager charactersManager;

    public void SetMoveCost(TileObj[,] tileObjs,bool attack)
    {
        // �ړ��R�X�g�̃}�b�v�f�[�^(_originalMapList)���쐬 (�{���͂����ł͍쐬�����O������n��)
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
    /// �T���J�n
    /// �v�Z���ʂ̃}�b�v�f�[�^��Ԃ�
    /// </summary>
    public int[,] StartSearch(int currentX, int currentZ, int movePower)
    {
        // _originalMapList�̃R�s�[�쐬
        Copy();

        _xLength = _resultMoveRangeList.GetLength(0);
        _zLength = _resultMoveRangeList.GetLength(1);

        _x = currentX;
        _z = currentZ;
        _m = movePower;

        //Debug.Log($"_x:{_x},_z:{_z} : _xLength:{_xLength},_zLength{_zLength}");
        // ���݈ʒu�Ɍ��݂̈ړ��͂���
        _resultMoveRangeList[_x,_z] = _m;
        Search4(_x, _z, _m);

        return _resultMoveRangeList;
    }

    /// <summary>
    /// �ړ��\�Ȕ͈͂�4�����𒲂ׂ�
    /// </summary>
    void Search4(int x, int z, int m)
    {

        if (0 <= x && x < _xLength && 0 <= z && z < _zLength)
        {
            // �����
            Search(x, z - 1, m);
            // ������
            Search(x, z + 1, m);
            // ������
            Search(x - 1, z, m);
            // �E����
            Search(x + 1, z, m);
        }
    }

    /// <summary>
    /// �ړ���̃Z���̒���
    /// </summary>
    void Search(int x, int z, int m)
    {
        // �T��������Cell���}�b�v�G���A�̈�����`�F�b�N
        if (x < 0 || _xLength <= x) return;
        if (z < 0 || _zLength <= z) return;

        // ���łɌv�Z�ς݂�Cell���`�F�b�N
        if ((m - 1) <= _resultMoveRangeList[x,z]) return;

        // ���݂̈ړ��\�� + �n�`�R�X�g
        m = m + _originalMapList[x,z];

        if (m > 0)
        {
            // �i�񂾈ʒu�Ɍ��݂̈ړ��͂���
            _resultMoveRangeList[x,z] = m;
            // �ړ��ʂ�����̂�Search4���ċA�Ăт���
            Search4(x, z, m);
        }
        else
        {
            m = 0;
        }
    }

    // �o�H�T��
    // ���݂̈ʒu����A�N���b�N�����ꏊ�܂ł̃}�X���擾�������B
    // �N���b�N�����}�X(�S�[���}�X)����ړ��R�X�g��߂��āA�t���Ōo�H���o��
    public List<TileObj> GetRoot(Vector2Int startIndex,Vector2Int goalIndex,TileObj[,] tileObjs)
    {
        List<TileObj> root = new List<TileObj>();

        root.Add(tileObjs[goalIndex.x, goalIndex.y]);

        Search4Root(root,startIndex,goalIndex,tileObjs);
        root.Reverse();
        return root;
    }

    // �㉺���E�ŁA�ړ��R�X�g����v������̂������root�ɒǉ����āAstartIndex�ƈ�v����܂Œ��ׂ�
    void Search4Root(List<TileObj> root,Vector2Int startIndex, Vector2Int searchIndex, TileObj[,] tileObjs)
    {
        if(startIndex == searchIndex)
        {
            // �����I��
            return;
        }
        // �ړ��R�X�g��߂��Ĉ�v������̂������root�ɒǉ����Ă��̏ꏊ�𒲂ׂ�
        // ���݂̈ړ��R�X�g
        int currentMovePower = _resultMoveRangeList[searchIndex.x, searchIndex.y];
        // �ړ��O�̈ړ��R�X�g�ɖ߂�
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
                // root�ɒǉ�
                root.Add(tileObjs[arroundIndex.x, arroundIndex.y]);
                // ����ɁA���̏ꏊ�ɂ��Ē��ׂ�
                Search4Root(root, startIndex, arroundIndex, tileObjs);
                break;
            }
        }
    }

    // �͈͊O�G���[����
    bool IsMatch(int currentMovePower,Vector2Int arroundIndex)
    {
        // �͈͊O�Ȃ�false
        if (arroundIndex.x < 0 || arroundIndex.x >= _resultMoveRangeList.GetLength(0))
        {
            return false;
        }
        if (arroundIndex.y < 0 || arroundIndex.y >= _resultMoveRangeList.GetLength(1))
        {
            return false;
        }

        // ��v����Ȃ�true
        if (currentMovePower == _resultMoveRangeList[arroundIndex.x, arroundIndex.y])
        {
            return true;
        }

        return false;
    }

    // �E�o�O�̏C��
    // �Z�E�o�H�T���ŕςȓ���������o�O�̏C�� => break������
    
    // �Z�E�����̐����ʒu�ɂ���
    // => ���������Ȃ畽���̏�ɂ���
    // => �L�����̈ʒu���擾���āA���̏ꍇ�}�X�͕����ɂ���

    // �Z�E�L������ʉ߂ł��Ȃ��悤�ɂ���
    // => �L����������ꏊ���R�X�g-99�ɂ��Ă��
    // => ����:�L�����͈ړ�����̂Ŗ��񏑂�����

    // �E�S�ẴL�����̈ړ����I����Ă���^�[�����I������
    //   �E�^�[���I���̃{�^�����������瑊��̃^�[���ɂȂ�
    // �E�G�̍U���̎���
}
