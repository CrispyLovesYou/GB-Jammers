using UnityEngine;
using System.Collections;

public abstract class Controller_Base : MonoBehaviour
{
    #region Fields

    protected Transform cTransform;
    protected Rigidbody2D cRigidbody2D;
    protected PhotonView cPhotonView;

    #endregion

    #region Unity Callbacks

    protected virtual void Awake()
    {
        // Cache Unity lookups
        cTransform = GetComponent<Transform>();
        cRigidbody2D = GetComponent<Rigidbody2D>();
        cPhotonView = GetComponent<PhotonView>();
    }

    #endregion

    #region Methods

    public void Stop() { cRigidbody2D.velocity = Vector2.zero; }
    public virtual void Move(Vector2 _inputVector) { }
    public virtual void Action(Vector2 _inputVector) { }
    public virtual void Lob(Vector2 _inputVector) { }

    #endregion
}