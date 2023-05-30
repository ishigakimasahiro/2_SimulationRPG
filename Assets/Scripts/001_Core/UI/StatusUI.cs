using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    // Status�̕\��
    // �n���ꂽCharacter�f�[�^�̕\��

    [SerializeField] Text nameText;
    [SerializeField] Text hpText;
    [SerializeField] Text atText;
    [SerializeField] Text dfText;
    [SerializeField] Image hpBar;

    // Status�̕\��
    public void Show(Character character)
    {
        gameObject.SetActive(true);
        nameText.text = character.Name;
        hpText.text = $"{ character.Hp}/{character.MaxHp}";
        atText.text = $"AT:{character.At}";
        dfText.text = $"DF:{character.Df}";
        hpBar.fillAmount = (float)character.Hp / (float)character.MaxHp;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
