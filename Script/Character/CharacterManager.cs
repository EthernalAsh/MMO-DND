using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public List<Character> characters = new List<Character>();


    float time = 6;
    private void Update()
    {
        time -= Time.deltaTime;
        if (time < 0)
        {
            time = 6;
            MoveCharacters();
        }
    }

    private void MoveCharacters()
    {
        foreach (Character character in characters)
        {
            character.Move();
        }
    }
}