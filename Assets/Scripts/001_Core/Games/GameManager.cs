using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // �t�F�[�Y�̊Ǘ�
    enum Phase
    {
        PlayerCharacterSelection,       // �L�����I��
        PlayerCharacterMoveSelection,   // �L�����ړ�
        PlayerCharacterCommandSelection,// �R�}���h�I��
        PlayerCharacterTargetSelection, // �U���ΏۑI��
        EnemyCharacterSelection,        // �G�I��
        EnemyCharacterMoveSelection,    // �G�ړ�
        EnemyCharacterTargetSelection,  // �G�̍U��
    }
    [SerializeField] Phase phase;

    // �I�������L�����̕ێ�
    Character selectedCharacter;

    // �I���L�����̈ړ��\�͈͂�ێ�
    List<TileObj> movableTiles = new List<TileObj>();

    // �I���L�����̍U���\�͈͂�ێ�
    List<TileObj> attackableTiles = new List<TileObj>();

    // �L�������擾����֐����Ă�
    [SerializeField] CharactersManager charactersManager;

    // �N���b�N�����^�C�����擾
    [SerializeField] MapManager mapManager;

    // �R�}���hUI�̕\����\��
    [SerializeField] ActionCommandUI actionCommandUI;

    // TurnUI�̕\����\��
    [SerializeField] PhasePanelUI phasePanelUI;

    // �X�e�[�^�XUI�\��
    [SerializeField] StatusUI statusUI;

    // �_���[�WUI�\��
    [SerializeField] DamageUI damageUI;

    // �^�[���I���{�^���̕\��/��\��
    [SerializeField] GameObject turnEndButton;

    private void Start()
    {
        damageUI.OnEndPlayerAnim += OnAttacked;
        phase = Phase.PlayerCharacterSelection;
        actionCommandUI.Show(false);
        StartCoroutine(phasePanelUI.PhasePanelAnim("PLAYER TURN"));// �t�F�[�Y�A�j��
        turnEndButton.SetActive(true);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            PlayerClickAction();
        }
    }

    void SetMoveInfomation()
    {
        // �擾�����q�I�u�W�F�N�g����������
        // �ړ��͈͂̕\��
        mapManager.ShowMovablePanels(selectedCharacter, movableTiles);  /*2023//5/31 null�`�F�b�N�̕ύX*/
    }

    void SetAttackInfomation()
    {
        // �擾�����q�I�u�W�F�N�g����������
        // �U���͈͂̕\��
        mapManager.ShowAttackablePanels(selectedCharacter, attackableTiles);  /*2023//5/31 null�`�F�b�N�̕ύX*/
    }

    void PlayerClickAction()
    {
        switch(phase)
        {
            case Phase.PlayerCharacterSelection:
                PlayerCharacterSelection();
                break;
            case Phase.PlayerCharacterMoveSelection:
                PlayerCharacterMoveSelection();
                break;
            case Phase.PlayerCharacterCommandSelection:
                PlayerCharacterCommandSelection();
                break;
            case Phase.PlayerCharacterTargetSelection:
                PlayerCharacterTargetSelection();
                break;
            case Phase.EnemyCharacterSelection:
                EnemyCharacterSelection();
                break;
            case Phase.EnemyCharacterMoveSelection:
                EnemyCharacterMoveSelection();
                break;
            case Phase.EnemyCharacterTargetSelection:
                EnemyCharacterTargetSelection();
                break;
        }
    }

    bool IsClickCharacter(TileObj clickTileObj)
    {
        if (clickTileObj==null)
        {
            return false;
        }

        Character character = charactersManager.GetCharacter(clickTileObj.positionInt);
        if(character)
        {
            // �I���L�����̕ێ�
            selectedCharacter = character;

            // �L�����̃X�e�[�^�X��\��
            statusUI.Show(selectedCharacter);

            // ���������̃L�����������Ă��Ȃ��Ȃ�A�ړ��͈͕\��
            if (character.IsMoved == false && character.IsEnemy == false)
            {
                // �ړ��͈͂����Z�b�g
                mapManager.ResetMovablePanels(movableTiles);

                SetMoveInfomation();

                return true;
            }
        }
        return false;
    }

    // PlayerCharacterSelection�t�F�[�Y�ŃN���b�N�����Ƃ��ɂ�肽������
    void PlayerCharacterSelection()
    {
        // �N���b�N�����^�C�����擾����
        TileObj clickTileObj = mapManager.GetClickTileObj();

        // ���̏�ɃL����������Ȃ�
        if (IsClickCharacter(clickTileObj))
        {
            phase = Phase.PlayerCharacterMoveSelection;
        }
    }

    // PlayerCharacterMoveSelection�t�F�[�Y�ŃN���b�N�����Ƃ��ɂ�肽������
    void PlayerCharacterMoveSelection()
    {
        // �N���b�N�����ꏊ���ړ��͈͂Ȃ�ړ������āA�G�̃t�F�[�Y��
        TileObj clickTileObj = mapManager.GetClickTileObj();

        if (IsClickCharacter(clickTileObj)) return;

        // �L������ێ����Ă���Ȃ�A�I�������^�C���̏ꏊ�Ɉړ�������
        if (selectedCharacter)
        {
            // �N���b�N����tileObj���ړ��͈͂Ɋ܂܂��Ȃ�
            if (movableTiles.Contains(clickTileObj))
            {
                // selectedCharacter��tileObj�܂ňړ�������
                // �o�H���擾���Ĉړ�����
                selectedCharacter.Move(clickTileObj.positionInt,mapManager.GetRoot(selectedCharacter,clickTileObj),null);  /*2023//5/31 �֐����o�^*/

                phase = Phase.PlayerCharacterCommandSelection;

                // �R�}���h�̕\��
                actionCommandUI.Show(true);
            }
            mapManager.ResetMovablePanels(movableTiles);  // false
        }
    }

    void PlayerCharacterCommandSelection()
    {
        // �����v���C���[���G�ɍU��������v���C���[�^�[���I��
        TileObj clickTileObj = mapManager.GetClickTileObj();

        // �U���͈͓̔����N���b�N������
        if (attackableTiles.Contains(clickTileObj))
        {
            // �L����������Ȃ�
            Character targetChara = charactersManager.GetCharacter(clickTileObj.positionInt);

            if (targetChara && targetChara.IsEnemy)
            {
                phase = Phase.PlayerCharacterTargetSelection;
            }
        }
    }

    // �U���͈͓��̓G���N���b�N������U������
    // �E���Ȃ��ꍇ�͑ҋ@�{�^���������ă^�[���I��
    void PlayerCharacterTargetSelection()
    {
        TileObj clickTileObj = mapManager.GetClickTileObj();

        // �U���͈͓̔����N���b�N������
        if (attackableTiles.Contains(clickTileObj))
        {
            // �L����������Ȃ�
            Character targetChara = charactersManager.GetCharacter(clickTileObj.positionInt);
            if (targetChara && targetChara.IsEnemy)
            {
                // �X�e�[�^�X�̎���
                // �_���[�W���󂯂�֐�
                // �U��������֐�
                Debug.Log("�U������");
                int damage = selectedCharacter.Attack(targetChara);
                mapManager.ResetAttackablePanels(attackableTiles);
                actionCommandUI.Show(false);
                damageUI.Show(targetChara, damage);
            }
        }
    }

    public void OnAttackButton()
    {
        Debug.Log("�U���I��");
        phase = Phase.PlayerCharacterTargetSelection;
        // �U���͈͂����Z�b�g
        mapManager.ResetAttackablePanels(attackableTiles);

        SetAttackInfomation();

        actionCommandUI.ShowAttackButton(false);
    }

    public void OnWaitButton()
    {
        Debug.Log("�ҋ@�I��");
        //OnPlayerTurnEnd();
        actionCommandUI.Show(false);
        selectedCharacter = null;
        mapManager.ResetAttackablePanels(attackableTiles);
        phase = Phase.PlayerCharacterSelection;
    }

    // �U�����I�������
    void OnAttacked()
    {
        if (phase == Phase.PlayerCharacterTargetSelection)
        {
            actionCommandUI.Show(false);
            selectedCharacter = null;
            mapManager.ResetAttackablePanels(attackableTiles);
            phase = Phase.PlayerCharacterSelection;
        }

        if (phase == Phase.EnemyCharacterTargetSelection)  /*2023//5/31 �ǉ�*/
        {
            actionCommandUI.Show(false);
            selectedCharacter = null;
            mapManager.ResetAttackablePanels(attackableTiles);
            OnEnemyTurnEnd();
        }
    }

    void OnPlayerTurnEnd()
    {
        Debug.Log("����^�[��");
        phase = Phase.EnemyCharacterSelection;
        actionCommandUI.Show(false);
        selectedCharacter = null;
        mapManager.ResetAttackablePanels(attackableTiles);
        StartCoroutine(phasePanelUI.PhasePanelAnim("ENEMY TURN"));// �t�F�[�Y�A�j��
        StartCoroutine(EnemyCharacterSelection());  // �t�F�[�Y�A�j�����I����Ă�����s������

        foreach (var chara in charactersManager.characters)
        {
            if (chara.IsEnemy)
            {
                chara.OnBeginTurn();
            }
        }
    }

    IEnumerator EnemyCharacterSelection()
    {
        yield return new WaitForSeconds(1.3f);

        if (IsEnemyCharacter())
        {
            Debug.Log("�G�̃L�����I��");
            phase = Phase.EnemyCharacterMoveSelection;
        }
    }

    bool IsEnemyCharacter()
    {
        TileObj tileObj = mapManager.GetClickTileObj();  /*2023//5/31 �ǉ�*/

        Character randomEnemy = charactersManager.GetRandomEnemy();
        if (randomEnemy)
        {
            // �I���L�����̕ێ�
            selectedCharacter = randomEnemy;

            // �L�����̃X�e�[�^�X��\��
            statusUI.Show(selectedCharacter);

            // ���������̃L�����������Ă��Ȃ��Ȃ�A�ړ��͈͕\��
            if (randomEnemy.IsMoved == false && randomEnemy.IsEnemy)
            {
                // �ړ��͈͂����Z�b�g
                mapManager.ResetMovablePanels(movableTiles);
                // �ړ��͈͂�\��
                SetMoveInfomation();

                Invoke("EnemyCharacterMoveSelection", 2f);
                return true;
            }
        }
        return false;
    }

    void EnemyCharacterMoveSelection()
    {
        // �菇
        // �E�^�[�Q�b�g�ƂȂ�Player��������=> ��ԋ߂�Player
        Character target = charactersManager.GetClosestCharacter(selectedCharacter);

        // �E�ړ��͈͂̒��ŁAPlayer�ɋ߂��ꏊ��T��
        TileObj targetTile = movableTiles
            .OrderBy(tile => Vector2.Distance(target.Position, tile.positionInt))  // ���������ɕ��ׂ�
            .FirstOrDefault();// �ŏ��̃^�C����n��

        if (target == null) return;
        if (targetTile == null) return;

        // �G�L�����ȊO�̃^�C�����N���b�N����ƃG���[�A2�̖ڂ̓G�L�����̈ړ����ɃG���[
        selectedCharacter.Move(targetTile.positionInt, mapManager.GetRoot(selectedCharacter, targetTile),EnemyCharacterTargetSelection);/*2023//5/31 �֐��o�^*/
        mapManager.ResetMovablePanels(movableTiles);
    }

    // �G�̍U��
    void EnemyCharacterTargetSelection()
    {
        phase = Phase.EnemyCharacterTargetSelection;/*2023//5/31 �ǉ�*/

        TileObj tileObj = mapManager.GetClickTileObj();/*2023//5/31 �ǉ�*/

        // �U���͈͂����Z�b�g
        mapManager.ResetAttackablePanels(attackableTiles);

        // �U���͈͂̕\��
        SetAttackInfomation();

        // �͈͓���Player�L����������Ȃ�擾
        Character targetChara = null;

        foreach (var tile in attackableTiles)
        {
            Character character = charactersManager.GetCharacter(tile.positionInt);
            if (character && character.IsEnemy == false)
            {
                targetChara = character;
            }
        }

        // �^�[�Q�b�g������Ȃ�U�������s
        if (targetChara)
        {
            phase = Phase.EnemyCharacterTargetSelection;
            int damage = selectedCharacter.Attack(targetChara);
            actionCommandUI.Show(false);
            selectedCharacter = null;
            damageUI.Show(targetChara, damage);
            /*2023//5/31 �폜*/
        }
        else
        {
            Invoke("OnEnemyTurnEnd", 2f);
        }
    }

    void OnEnemyTurnEnd()
    {
        //Debug.Log("�G�^�[���I��");
        selectedCharacter = null;
        phase = Phase.PlayerCharacterSelection;
        mapManager.ResetAttackablePanels(attackableTiles);           // �U���͈͔�\��  /*2023//5/31 �ǉ�*/
        StartCoroutine(phasePanelUI.PhasePanelAnim("PLAYER TURN"));  // �t�F�[�Y�A�j��
        OnEnemyTurnEndButton();
        foreach (var chara in charactersManager.characters)
        {
            if(chara.IsEnemy == false)
            {
                chara.OnBeginTurn();
            }
        }
    }

    public void OnEnemyTurnEndButton()
    {
        turnEndButton.SetActive(true);
    }

    public void OnTurnEndButton()
    {
        OnPlayerTurnEnd();
        turnEndButton.SetActive(false);
    }
}

// �G���[����
// �A�����ăL������I�ׂȂ�

// ��x�s�������L�����͍s���ł��Ȃ�
// �U�������ꍇ����ɑ���^�[���ɂȂ��Ă��܂�
// => �ړ��������ǂ����̃t���O(bool)������Ă��΂悢

// �E�U�������ꍇ�Ƀ^�[���̐؂�ւ�������Ȃ�  =>  phase�؂�ւ�����5/31�ǉ�
// �E�ړ��Ɠ����ɍU�������Ă��܂�
// �E�G���A�̒[���ƃG���[���o��
// �EPlayer�̕��ɋ߂Â��Ȃ�
// �E�l��  => ����Map��null��������s�����Ȃ�
// �E�G�L�������U��������̓G�̃^�[���Ŏ~�܂�(EnemyCharacterSelection)
