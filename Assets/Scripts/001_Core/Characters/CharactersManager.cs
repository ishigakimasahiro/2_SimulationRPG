using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// ƒLƒƒƒ‰‚·‚×‚Ä‚ğŠÇ—‚·‚é
public class CharactersManager : MonoBehaviour
{
    public List<Character> characters = new List<Character>();

    //void Start()
    //{
    //    GetComponentsInChildren(characters);
    //}

    // À•W‚ªˆê’v‚·‚éƒLƒƒƒ‰‚ğ“n‚·
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

    // “GƒLƒƒƒ‰‚ğƒ‰ƒ“ƒ_ƒ€‚Éˆê‘Ìæ“¾‚·‚é
    public Character GetRandomEnemy()
    {
        // ‘S‚Ä‚Ì“G‚ğW‚ß‚é
        List<Character> enemies = characters.FindAll(characters => characters.IsEnemy);
        // ƒ‰ƒ“ƒ_ƒ€‚Éˆê‚Â“n‚·
        int r = Random.Range(0, enemies.Count);
        return enemies[r];
    }

    // ˆÚ“®‚Å‚«‚é“GƒLƒƒƒ‰‚ğæ“¾
    public Character GetMovableEnemy()
    {
        return characters.Find(enemy => enemy.IsEnemy && !enemy.IsMoved);
    }


    // ©•ª‚ÉÅ‚à‹ß‚¢ƒLƒƒƒ‰(“GƒLƒƒƒ‰)‚ğ’T‚·
    // “GƒLƒƒƒ‰FPlayer‚©‚ç‚·‚é‚ÆEnemy,Enemy‚©‚ç‚·‚é‚ÆPlayer

    public Character GetClosestCharacter(Character self)
    {
        return characters
            .Where(chara => chara.IsEnemy != self.IsEnemy)// “G‚ğ’T‚·
            .OrderBy(chara => Vector2.Distance(self.Position, chara.Position))// self‚©‚ç‹——£‚ª‹ß‚¢‡‚É•À‚×‚é
            .FirstOrDefault();// Å‚à‹ß‚¢ƒLƒƒƒ‰‚ğ“n‚·
    }
}
