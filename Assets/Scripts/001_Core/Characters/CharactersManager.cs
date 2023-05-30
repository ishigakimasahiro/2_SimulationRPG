using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// キャラすべてを管理する
public class CharactersManager : MonoBehaviour
{
    public List<Character> characters = new List<Character>();

    //void Start()
    //{
    //    GetComponentsInChildren(characters);
    //}

    // 座標が一致するキャラを渡す
    public Character GetCharacter(Vector2Int pos)
    {
        foreach(var character in characters)
        {
            if(character.Position==pos)
            {
                return character;
            }
        }
        return null;
    }

    public Character GetCharacter(Vector2 pos)
    {
        foreach (var character in characters)
        {
            if (character.Position == pos)
            {
                return character;
            }
        }
        return null;
    }

    public Character GetRandomEnemy()
    {
        List<Character> enemies = characters.FindAll(characters => characters.IsEnemy);
        int r = Random.Range(0, enemies.Count);
        return enemies[r];
    }

    // 自分に最も近いキャラ(敵キャラ)を探す
    // 敵キャラ：PlayerからするとEnemy,EnemyからするとPlayer

    public Character GetClosestCharacter(Character self)
    {
        return characters
            .Where(chara => chara.IsEnemy != self.IsEnemy)// 敵を探す
            .OrderBy(chara => Vector2.Distance(self.Position, chara.Position))// selfから距離が近い順に並べる
            .FirstOrDefault();// 最も近いキャラを渡す
    }
}
