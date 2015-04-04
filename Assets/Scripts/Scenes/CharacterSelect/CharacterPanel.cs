using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterPanel : MonoBehaviour {

	public Image ReadyImage;
	public Image[] PowerStars;
	public Image[] SpeedStars;

	public Text PassiveText;
	public Text ExText;
	public Text SuperText;
	public CharNameSwapper CharNameSwapper;

	public void OnCharacterSelect(int id){
		// Change UI to match character
		CharacterID charID = (CharacterID) id;
		Debug.Log ("Char id is " + charID);
		PassiveText.text = Globals.CharacterDict[charID].AbilityPassive;
		ExText.text = Globals.CharacterDict[charID].AbilityEX;
		SuperText.text = Globals.CharacterDict[charID].AbilitySuper;
		for(int i = 0; i < PowerStars.Length && i < Globals.CharacterDict[charID].ThrowPower; i++){
			PowerStars[i].GetComponent<SpriteSwapper>().SpriteSwap();
		}

		for(int i = 0; i < SpeedStars.Length && i < Globals.CharacterDict[charID].MoveSpeed; i++){
			SpeedStars[i].GetComponent<SpriteSwapper>().SpriteSwap();
		}

		CharNameSwapper.CharNameSwap(id);
	}

	public void OnCharacterConfirm(){
		ReadyImage.enabled = true;
	}

	public void OnCharacterCancel(){
		ReadyImage.enabled = false;
	}
}
