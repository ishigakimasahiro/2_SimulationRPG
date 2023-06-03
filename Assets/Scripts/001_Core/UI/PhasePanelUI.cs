using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;

public class PhasePanelUI : MonoBehaviour
{
    // ・rotationを90にする
    // ・0に近づける
    // ・90に戻す

    [SerializeField] Text turnText;

    private void Start()
    {
        // rotationを90にする
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    // OnEndAnimは渡された関数 : 何も渡されなかったらnull
    public IEnumerator PhasePanelAnim(string message,UnityAction unityAction = null)
    {
        turnText.text = message;
        yield return new WaitForSeconds(0.3f);
        // ・(0,0,0)に近づける(DOTWeen)  WaitForCompletion：1秒,アニメーション終了まで待機
        yield return transform.DORotate(new Vector3(0, 0, 0), 0.6f).WaitForCompletion();
        // 待機
        yield return new WaitForSeconds(0.7f);
        // 90に戻す
        yield return transform.DORotate(new Vector3(90, 0, 0), 0.6f).WaitForCompletion();
        yield return new WaitForSeconds(0.2f);
        unityAction?.Invoke();
    }

    // やること
    // ・"ENEMY TURN" "PLAYER TURN"切り替え
}
