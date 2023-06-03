using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// �L�������ׂĂ��Ǘ�����
public class CharactersManager : MonoBehaviour
{
    public List<Character> characters = new List<Character>();

    //void Start()
    //{
    //    GetComponentsInChildren(characters);
    //}

    // ���W����v����L������n��
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

    // �G�L�����������_���Ɉ�̎擾����
    public Character GetRandomEnemy()
    {
        // �S�Ă̓G���W�߂�
        List<Character> enemies = characters.FindAll(characters => characters.IsEnemy);
        // �����_���Ɉ�n��
        int r = Random.Range(0, enemies.Count);
        return enemies[r];
    }

    // �ړ��ł���G�L�������擾
    public Character GetMovableEnemy()
    {
        return characters.Find(enemy => enemy.IsEnemy && !enemy.IsMoved);
    }


    // �����ɍł��߂��L����(�G�L����)��T��
    // �G�L�����FPlayer���炷���Enemy,Enemy���炷���Player

    public Character GetClosestCharacter(Character self)
    {
        return characters
            .Where(chara => chara.IsEnemy != self.IsEnemy)// �G��T��
            .OrderBy(chara => Vector2.Distance(self.Position, chara.Position))// self���狗�����߂����ɕ��ׂ�
            .FirstOrDefault();// �ł��߂��L������n��
    }
}
