using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // フェーズの管理
    enum Phase
    {
        PlayerCharacterSelection,       // キャラ選択
        PlayerCharacterMoveSelection,   // キャラ移動
        PlayerCharacterCommandSelection,// コマンド選択
        PlayerCharacterTargetSelection, // 攻撃対象選択
        EnemyCharacterSelection,        // 敵選択
        EnemyCharacterMoveSelection,    // 敵移動
        EnemyCharacterTargetSelection,  // 敵の攻撃
    }
    [SerializeField] Phase phase;

    // 選択したキャラの保持
    Character selectedCharacter;

    // 選択キャラの移動可能範囲を保持
    List<TileObj> movableTiles = new List<TileObj>();

    // 選択キャラの攻撃可能範囲を保持
    List<TileObj> attackableTiles = new List<TileObj>();

    // キャラを取得する関数を呼ぶ
    [SerializeField] CharactersManager charactersManager;

    // クリックしたタイルを取得
    [SerializeField] MapManager mapManager;

    // コマンドUIの表示非表示
    [SerializeField] ActionCommandUI actionCommandUI;

    // TurnUIの表示非表示
    [SerializeField] PhasePanelUI phasePanelUI;

    // ステータスUI表示
    [SerializeField] StatusUI statusUI;

    // ダメージUI表示
    [SerializeField] DamageUI damageUI;

    // 
    [SerializeField] GameObject turnEndButton;

    private void Start()
    {
        damageUI.OnEndPlayerAnim += OnAttacked;
        phase = Phase.PlayerCharacterSelection;
        actionCommandUI.Show(false);
        StartCoroutine(phasePanelUI.PhasePanelAnim("PLAYER TURN"));// フェーズアニメ
        turnEndButton.SetActive(true);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            PlayerClickAction();
        }
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
            // 選択キャラの保持
            selectedCharacter = character;

            // キャラのステータスを表示
            statusUI.Show(selectedCharacter);

            // もし自分のキャラが動いていないなら、移動範囲表示
            if (character.IsMoved == false && character.IsEnemy == false)
            {
                mapManager.ResetMovablePanels(movableTiles);
                // 移動範囲を表示
                mapManager.ShowMovablePanels(selectedCharacter, movableTiles);
                return true;
            }
        }
        return false;
    }

    // PlayerCharacterSelectionフェーズでクリックしたときにやりたいこと
    void PlayerCharacterSelection()
    {
        // クリックしたタイルを取得して
        TileObj clickTileObj = mapManager.GetClickTileObj();

        // その上にキャラがいるなら
        if (IsClickCharacter(clickTileObj))
        {
            phase = Phase.PlayerCharacterMoveSelection;
        }
    }

    // PlayerCharacterMoveSelectionフェーズでクリックしたときにやりたいこと
    void PlayerCharacterMoveSelection()
    {
        // クリックした場所が移動範囲なら移動させて、敵のフェーズへ
        TileObj clickTileObj = mapManager.GetClickTileObj();

        if (IsClickCharacter(clickTileObj)) return;

        // キャラを保持しているなら、選択したタイルの場所に移動させる
        if (selectedCharacter)
        {
            // クリックしたtileObjが移動範囲に含まれるなら
            if (movableTiles.Contains(clickTileObj))
            {
                // selectedCharacterをtileObjまで移動させる
                // 経路を取得して移動する
                selectedCharacter.Move(clickTileObj.positionInt,mapManager.GetRoot(selectedCharacter,clickTileObj));

                phase = Phase.PlayerCharacterCommandSelection;

                // コマンドの表示
                actionCommandUI.Show(true);
            }
            mapManager.ResetMovablePanels(movableTiles);  // false
        }
    }

    void PlayerCharacterCommandSelection()
    {
        // もしプレイヤーが敵に攻撃したらプレイヤーターン終了
        TileObj clickTileObj = mapManager.GetClickTileObj();

        // 攻撃の範囲内をクリックしたら
        if (attackableTiles.Contains(clickTileObj))
        {
            // キャラがいるなら
            Character targetChara = charactersManager.GetCharacter(clickTileObj.positionInt);

            if (targetChara && targetChara.IsEnemy)
            {
                phase = Phase.PlayerCharacterTargetSelection;
            }
        }
    }

    // 攻撃範囲内の敵をクリックしたら攻撃する
    // ・いない場合は待機ボタンを押してターン終了
    void PlayerCharacterTargetSelection()
    {
        TileObj clickTileObj = mapManager.GetClickTileObj();

        // 攻撃の範囲内をクリックしたら
        if (attackableTiles.Contains(clickTileObj))
        {
            // キャラがいるなら
            Character targetChara = charactersManager.GetCharacter(clickTileObj.positionInt);
            if (targetChara && targetChara.IsEnemy)
            {
                // ステータスの実装
                // ダメージを受ける関数
                // 攻撃をする関数
                Debug.Log("攻撃処理");
                int damage = selectedCharacter.Attack(targetChara);
                mapManager.ResetAttackablePanels(attackableTiles);
                actionCommandUI.Show(false);
                damageUI.Show(targetChara, damage);
            }
        }
    }

    public void OnAttackButton()
    {
        Debug.Log("攻撃選択");
        phase = Phase.PlayerCharacterTargetSelection;
        // 攻撃範囲を表示
        mapManager.ResetAttackablePanels(attackableTiles);
        // 移動範囲を表示
        mapManager.ShowAttackablePanels(selectedCharacter, attackableTiles);

        actionCommandUI.ShowAttackButton(false);
    }

    public void OnWaitButton()
    {
        Debug.Log("待機選択");
        //OnPlayerTurnEnd();
        actionCommandUI.Show(false);
        selectedCharacter = null;
        mapManager.ResetAttackablePanels(attackableTiles);
        phase = Phase.PlayerCharacterSelection;
    }

    // 攻撃が終わったよ
    void OnAttacked()
    {
        if (phase == Phase.PlayerCharacterTargetSelection)
        {
            actionCommandUI.Show(false);
            selectedCharacter = null;
            mapManager.ResetAttackablePanels(attackableTiles);
            phase = Phase.PlayerCharacterSelection;
            StartCoroutine(phasePanelUI.PhasePanelAnim("PLAYER TURN"));// フェーズアニメ
        }
    }

    void OnPlayerTurnEnd()
    {
        Debug.Log("相手ターン");
        phase = Phase.EnemyCharacterSelection;
        actionCommandUI.Show(false);
        selectedCharacter = null;
        mapManager.ResetAttackablePanels(attackableTiles);
        StartCoroutine(phasePanelUI.PhasePanelAnim("ENEMY TURN"));// フェーズアニメ
        StartCoroutine(EnemyCharacterSelection());  // フェーズアニメが終わってから実行したい

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
            Debug.Log("敵のキャラ選択");
            phase = Phase.EnemyCharacterMoveSelection;
        }
    }

    bool IsEnemyCharacter()
    {
        Character randomEnemy = charactersManager.GetRandomEnemy();
        if (randomEnemy)
        {
            // 選択キャラの保持
            selectedCharacter = randomEnemy;

            // キャラのステータスを表示
            statusUI.Show(selectedCharacter);

            // もし自分のキャラが動いていないなら、移動範囲表示
            if (randomEnemy.IsMoved == false && randomEnemy.IsEnemy)
            {
                mapManager.ResetMovablePanels(movableTiles);
                // 移動範囲を表示
                mapManager.ShowMovablePanels(selectedCharacter, movableTiles);
                Invoke("EnemyCharacterMoveSelection", 2f);
                return true;
            }
        }
        return false;
    }

    void EnemyCharacterMoveSelection()
    {
        // 手順
        // ・ターゲットとなるPlayerを見つける=> 一番近いPlayer
        Character target = charactersManager.GetClosestCharacter(selectedCharacter);

        // ・移動範囲の中で、Playerに近い場所を探す
        TileObj targetTile = movableTiles
            .OrderBy(tile => Vector2.Distance(target.Position, tile.positionInt))// 小さい順に並べる
            .FirstOrDefault();// 最初のタイルを渡す

        // 敵キャラ以外のタイルをクリックするとエラー、2体目の敵キャラの移動時にエラー
        selectedCharacter.Move(targetTile.positionInt, mapManager.GetRoot(selectedCharacter, targetTile));
        mapManager.ResetMovablePanels(movableTiles);
        StartCoroutine(EnemyCharacterTargetSelection());
    }

    // 敵の攻撃
    IEnumerator EnemyCharacterTargetSelection()
    {
        // 攻撃範囲の取得
        mapManager.ResetAttackablePanels(attackableTiles);

        yield return new WaitForSeconds(2f);

        mapManager.ShowAttackablePanels(selectedCharacter, attackableTiles);
        // 範囲内にPlayerキャラがいるなら取得
        Character targetChara = null;

        foreach (var tile in attackableTiles)
        {
            Character character = charactersManager.GetCharacter(tile.positionInt);
            if (character && character.IsEnemy == false)
            {
                targetChara = character;
            }
        }

        // ターゲットがいるなら攻撃を実行
        if (targetChara)
        {
            phase = Phase.EnemyCharacterTargetSelection;
            int damage = selectedCharacter.Attack(targetChara);
            actionCommandUI.Show(false);
            selectedCharacter = null;
            damageUI.Show(targetChara, damage);
            StartCoroutine(OnEnemyAttacked());
        }
        else
        {
            Invoke("OnEnemyTurnEnd", 2f);
        }
    }

    // 攻撃が終わったよ
    IEnumerator OnEnemyAttacked()
    {
        Character randomEnemy = charactersManager.GetRandomEnemy();

        // 選択キャラの保持
        selectedCharacter = randomEnemy;

        // キャラのステータスを表示
        statusUI.Show(selectedCharacter);

        // 全ての敵キャラが移動、攻撃完了していなかったら
        if (randomEnemy.IsMoved == false && randomEnemy.IsEnemy)
        {
            actionCommandUI.Show(false);
            selectedCharacter = null;
            mapManager.ResetAttackablePanels(attackableTiles);
            /*TODO */
            yield return new WaitForSeconds(2f);
            phase = Phase.PlayerCharacterSelection;
        }
        else
        {
            Invoke("OnEnemyTurnEnd", 2f);
        }
    }

    void OnEnemyTurnEnd()
    {
        //Debug.Log("敵ターン終了");
        selectedCharacter = null;
        // 攻撃範囲非表示
        mapManager.ResetAttackablePanels(attackableTiles);
        phase = Phase.PlayerCharacterSelection;
        StartCoroutine(phasePanelUI.PhasePanelAnim("PLAYER TURN"));// フェーズアニメ
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

// エラー解消
// 連続してキャラを選べない

// 一度行動したキャラは行動できない
// 攻撃した場合勝手に相手ターンになってしまう
// => 移動したかどうかのフラグ(bool)を作ってやればよい

// ・攻撃した場合にターンの切り替えがされない=>もし全ての敵が行動終了していなかったらEnemyTurn
// ・移動と同時に攻撃をしてしまう
// ・エリアの端だとエラーが出る
// ・Playerの方に近づかない
// ・詰む
// ・敵キャラが攻撃した後の敵のターンで止まる(EnemyCharacterSelection)
