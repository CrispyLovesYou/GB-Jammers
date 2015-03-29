using System;
using System.Collections.Generic;

public static class Globals
{
    public static Dictionary<CharacterID, Character> CharacterDict = new Dictionary<CharacterID, Character>();
    public static CharacterID[] SelectedCharacters = new CharacterID[2];
}