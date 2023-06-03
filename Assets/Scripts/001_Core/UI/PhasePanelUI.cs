using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;

public class PhasePanelUI : MonoBehaviour
{
    // �Erotation��90�ɂ���
    // �E0�ɋ߂Â���
    // �E90�ɖ߂�

    [SerializeField] Text turnText;

    private void Start()
    {
        // rotation��90�ɂ���
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    // OnEndAnim�͓n���ꂽ�֐� : �����n����Ȃ�������null
    public IEnumerator PhasePanelAnim(string message,UnityAction unityAction = null)
    {
        turnText.text = message;
        yield return new WaitForSeconds(0.3f);
        // �E(0,0,0)�ɋ߂Â���(DOTWeen)  WaitForCompletion�F1�b,�A�j���[�V�����I���܂őҋ@
        yield return transform.DORotate(new Vector3(0, 0, 0), 0.6f).WaitForCompletion();
        // �ҋ@
        yield return new WaitForSeconds(0.7f);
        // 90�ɖ߂�
        yield return transform.DORotate(new Vector3(90, 0, 0), 0.6f).WaitForCompletion();
        yield return new WaitForSeconds(0.2f);
        unityAction?.Invoke();
    }

    // ��邱��
    // �E"ENEMY TURN" "PLAYER TURN"�؂�ւ�
}
