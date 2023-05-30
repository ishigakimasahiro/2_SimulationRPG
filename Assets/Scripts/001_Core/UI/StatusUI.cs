using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    // Statusの表示
    // 渡されたCharacterデータの表示

    [SerializeField] Text nameText;
    [SerializeField] Text hpText;
    [SerializeField] Text atText;
    [SerializeField] Text dfText;
    [SerializeField] Image hpBar;

    // Statusの表示
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
