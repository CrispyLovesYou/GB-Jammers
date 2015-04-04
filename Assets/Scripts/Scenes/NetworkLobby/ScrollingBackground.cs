using UnityEngine;
using System.Collections;

public class ScrollingBackground : MonoBehaviour {

	MeshRenderer mr;
	public float speed = 1;
	public Vector2 dir = new Vector2(1,1);
	// Use this for initialization
	void Start () {
		mr = GetComponent <MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		mr.material.mainTextureOffset += dir * speed * Time.deltaTime;;
	}
}
