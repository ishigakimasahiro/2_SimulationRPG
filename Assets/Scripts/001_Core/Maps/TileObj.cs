using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ԍ�������
public class TileObj : MonoBehaviour
{
    public int x;
    public int y;

    public Vector2Int positionInt;
    Vector2Int index;  // ���Ԗڂ̃^�C���Ȃ̂�
    [SerializeField] int cost;    // �ړ��R�X�g

    [SerializeField] GameObject movablePanel;
    [SerializeField] GameObject attackablePanel;

    public int Cost { get => cost; }
    public Vector2Int Index { get => index; }

    public void ShowMovablePanel(bool isActive)
    {
        movablePanel.SetActive(isActive);
    }

    public void ShowAttackablePanel(bool isActive)
    {
        attackablePanel.SetActive(isActive);
    }

    public void SetCost(int cost)
    {
        this.cost = cost;
    }
    public void SetIndex(int x,int y)
    {
        this.index = new Vector2Int(x,y);
    }
}
