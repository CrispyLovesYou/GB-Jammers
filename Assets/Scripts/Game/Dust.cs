using UnityEngine;
using System.Collections;

public class Dust : MonoBehaviour
{
    public void OnAnimationComplete()
    {
        Destroy(this.gameObject);
    }
}