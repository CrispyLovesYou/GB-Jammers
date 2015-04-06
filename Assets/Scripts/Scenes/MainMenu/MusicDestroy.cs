using UnityEngine;
using System.Collections;

public class MusicDestroy : MonoBehaviour
{
    public static MusicDestroy Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    public static void DestroyObject()
    {
        Destroy(Instance.gameObject);
    }
}