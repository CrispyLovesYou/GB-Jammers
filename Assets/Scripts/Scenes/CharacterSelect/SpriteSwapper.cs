using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpriteSwapper : MonoBehaviour {

	public Sprite AltSprite;
	Sprite defaultSprite;
	Image image;

	void Awake(){
		image = GetComponent<Image>();
	}

	public void ShowAlt(){
		image.overrideSprite = AltSprite;
	}

	public void ShowDefault(){
		image.overrideSprite = null;
	}
}
