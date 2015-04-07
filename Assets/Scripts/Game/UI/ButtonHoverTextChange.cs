using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Button))]
[AddComponentMenu("Game/UI/Button Hover Text Color Change")]
public class ButtonHoverTextChange : MonoBehaviour
{
    #region Fields

    public Color HoverColor = (Color)new Color32(190, 70, 70, 255);

    private Button cButton;
    private Text cText;
    private Color defaultColor;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cButton = GetComponent<Button>();
        cText = GetComponentInChildren<Text>();

        defaultColor = cText.color;
    }

    #endregion

    #region Methods

    public void OnPointerEnter()
    {
        if (!cButton.IsInteractable())
            return;
        
        cText.color = HoverColor;
    }

    public void OnPointerExit()
    {
        if (EventSystem.current.currentSelectedGameObject != this.gameObject)
            cText.color = defaultColor;
    }

    public void OnSelect()
    {
        if (!cButton.IsActive())
            return;
        
        cText.color = HoverColor;
    }

    public void OnDeselect()
    {
        cText.color = defaultColor;
    }

    #endregion
}