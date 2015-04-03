using System;
using UnityEngine;

public class EXEventArgs : EventArgs
{
    public Vector2 InputVector;

    public EXEventArgs(Vector2 _inputVector)
    {
        InputVector = _inputVector;
    }
}
