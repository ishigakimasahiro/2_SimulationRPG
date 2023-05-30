using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCommandUI : MonoBehaviour
{
    // ActionCommandÇÃUIÇä«óùÇ∑ÇÈ
    [SerializeField] GameObject attackButton;
    [SerializeField] GameObject waitButton;

    public void Show(bool isActive)
    {
        attackButton.SetActive(isActive);
        waitButton.SetActive(isActive);
    }

    public void ShowAttackButton(bool isActive)
    {
        attackButton.SetActive(isActive);
    }

    public void ShowWaitButton(bool isActive)
    {
        waitButton.SetActive(isActive);
    }
}
