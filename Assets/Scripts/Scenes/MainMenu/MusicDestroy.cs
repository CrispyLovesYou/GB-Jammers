using UnityEngine;
using System.Collections;

public class MusicDestroy : MonoBehaviour
{
    private static MusicDestroy Instance;

    private void Awake()
    {
        Instance = this;
    }

    public static void DestroyObject()
    {
        Destroy(Instance.gameObject);
    }
}