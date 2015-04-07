using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class VersionNumber : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Text>().text = "v" + Globals.GAME_VERSION;
    }
}