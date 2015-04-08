using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Popup Handler (Attach to Player)")]
public class PopupHandler : MonoBehaviour
{
    #region Fields

    public GameObject SpeedBuffPopup;
    public GameObject SpeedDebuffPopup;
    public GameObject PowerBuffPopup;
    public GameObject PowerDebuffPopup;
    public GameObject GreatPopup;
    public GameObject PerfectPopup;
    public GameObject HeartsPopup;

    public float LeftRightOffset = 1.0f;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        Controller_Player.OnSpeedBuff += Controller_Player_OnSpeedBuff;
        Controller_Player.OnSpeedDebuff += Controller_Player_OnSpeedDebuff;
        Controller_Player.OnPowerBuff += Controller_Player_OnPowerBuff;
        Controller_Player.OnPowerDebuff += Controller_Player_OnPowerDebuff;
        Controller_Player.OnGreatThrow += Controller_Player_OnGreatThrow;
        Controller_Player.OnPerfectThrow += Controller_Player_OnPerfectThrow;
        Controller_Player.OnBookStun += Controller_Player_OnBookStun;
    }

	private void OnDestroy(){
		Controller_Player.OnSpeedBuff -= Controller_Player_OnSpeedBuff;
		Controller_Player.OnSpeedDebuff -= Controller_Player_OnSpeedDebuff;
		Controller_Player.OnPowerBuff -= Controller_Player_OnPowerBuff;
		Controller_Player.OnPowerDebuff -= Controller_Player_OnPowerDebuff;
		Controller_Player.OnGreatThrow -= Controller_Player_OnGreatThrow;
		Controller_Player.OnPerfectThrow -= Controller_Player_OnPerfectThrow;
		Controller_Player.OnBookStun -= Controller_Player_OnBookStun;
	}

    #endregion

    #region Callbacks

    private void Controller_Player_OnSpeedBuff(object sender, System.EventArgs e)
    {
        Controller_Player player = (Controller_Player)sender;

        Vector3 position = Vector3.left * LeftRightOffset;
        GameObject gObj = Instantiate(SpeedBuffPopup, position, Quaternion.identity) as GameObject;
        gObj.GetComponent<StatusPopup>().ParentTransform = player.transform;
    }

    private void Controller_Player_OnSpeedDebuff(object sender, System.EventArgs e)
    {
        Controller_Player player = (Controller_Player)sender;

        Vector3 position = Vector3.left * LeftRightOffset;
        GameObject gObj = Instantiate(SpeedDebuffPopup, position, Quaternion.identity) as GameObject;
        gObj.GetComponent<StatusPopup>().ParentTransform = player.transform;
    }

    private void Controller_Player_OnPowerBuff(object sender, System.EventArgs e)
    {
        Controller_Player player = (Controller_Player)sender;

        Vector3 position = Vector3.right * LeftRightOffset;
        GameObject gObj = Instantiate(PowerBuffPopup, position, Quaternion.identity) as GameObject;
        gObj.GetComponent<StatusPopup>().ParentTransform = player.transform;
    }

    private void Controller_Player_OnPowerDebuff(object sender, System.EventArgs e)
    {
        Controller_Player player = (Controller_Player)sender;

        Vector3 position = Vector3.right * LeftRightOffset;
        GameObject gObj = Instantiate(PowerDebuffPopup, position, Quaternion.identity) as GameObject;
        gObj.GetComponent<StatusPopup>().ParentTransform = player.transform;
    }

    private void Controller_Player_OnGreatThrow(object sender, System.EventArgs e)
    {
        Controller_Player player = (Controller_Player)sender;
        GameObject gObj = Instantiate(GreatPopup, Vector3.zero, Quaternion.identity) as GameObject;
        gObj.GetComponent<StatusPopup>().ParentTransform = player.transform;
    }

    private void Controller_Player_OnPerfectThrow(object sender, System.EventArgs e)
    {
        Controller_Player player = (Controller_Player)sender;
        GameObject gObj = Instantiate(PerfectPopup, Vector3.zero, Quaternion.identity) as GameObject;
        gObj.GetComponent<StatusPopup>().ParentTransform = player.transform;
    }

    private void Controller_Player_OnBookStun(object sender, System.EventArgs e)
    {
        Controller_Player player = (Controller_Player)sender;
        GameObject gObj = Instantiate(HeartsPopup, Vector3.up * 0.5f, Quaternion.identity) as GameObject;
        gObj.GetComponent<StatusPopup>().ParentTransform = player.transform;
    }

    #endregion
}