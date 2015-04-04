using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpriteSwapper : MonoBehaviour {

	public Sprite AltSprite;
	Image image;

	void Awake(){
		image = GetComponent<Image>();
	}

	public void SpriteSwap(){
		image.sprite = AltSprite;
	}
}
