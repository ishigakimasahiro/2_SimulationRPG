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

    public Character GetRandomEnemy()
    {
        List<Character> enemies = characters.FindAll(characters => characters.IsEnemy);
        int r = Random.Range(0, enemies.Count);
        return enemies[r];
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
