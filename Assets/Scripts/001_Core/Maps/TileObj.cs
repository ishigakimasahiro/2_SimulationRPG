using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 番号をつける
public class TileObj : MonoBehaviour
{
    public int x;
    public int y;

    public Vector2Int positionInt;
    Vector2Int index;  // 何番目のタイルなのか
    [SerializeField] int cost;    // 移動コスト

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
