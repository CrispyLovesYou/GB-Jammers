using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharNameSwapper : MonoBehaviour {

	public Sprite[] sprites;
	Image image;
	void Awake(){
		image = GetComponent<Image>();
	}

	public void CharNameSwap(int _index){
		image.sprite = sprites[_index];
		image.SetNativeSize();
	}
}
