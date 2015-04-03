using System;
using UnityEngine;

public class SuperEventArgs : EventArgs
{
    public Vector2 InputVector;

    public SuperEventArgs(Vector2 _inputVector)
    {
        InputVector = _inputVector;
    }
}
