using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Linq;

// �L�����N�^�[�Ǘ�
public class Character : MonoBehaviour
{
    [SerializeField] new string name;
    [SerializeField] int hp;
    [SerializeField] int maxHp;
    [SerializeField] int at;
    [SerializeField] int df;
    [SerializeField] bool isEnemy;
    [SerializeField] Vector2Int positionInt;
    [SerializeField] int moveRange;
    [SerializeField] int attackRange;
    [SerializeField] bool isMoved;

    public Vector2Int Position { get => positionInt; }
    public bool IsEnemy { get => isEnemy; }
    public string Name { get => name; }
    public int Hp { get => hp; }
    public int At { get => at; }
    public int Df { get => df; }
    public int MaxHp { get => maxHp; }
    public int MoveRange { get => moveRange; }
    public int AttackRange { get => attackRange; }
    public bool IsMoved { get => isMoved; }

    void Start()
    {
        transform.position = (Vector2)positionInt;
    }

    // �L�������ړ�
    public void Move(Vector2Int pos,List<TileObj> root,UnityAction movedAction)
    {
        // Select���g���āA���X�g�̒��̓���̗v�f�������擾�������X�g�����
        Vector3[] path = root.Select(tile => tile.transform.position).ToArray();
          
        // �o�H�ɉ����Ĉړ�����(�o�H,�ړ�����)
        transform.DOPath(path, 0.3f).SetEase(Ease.Linear).OnComplete(() =>movedAction?.Invoke());
        
        positionInt = pos;
        isMoved = true;
    }

    public int Damage(int value)
    {
        hp -= value;
        if(hp<=0)
        {
            hp = 0;
        }
        return value;
    }

    public int Attack(Character target)
    {
        return target.Damage(at);
    }

    public void OnBeginTurn()
    {
        isMoved = false;
    }
}
