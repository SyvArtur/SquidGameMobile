using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using MyBox;
using UnityEngine.UI;

public class UIVirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    [System.Serializable]
    public class Event : UnityEvent { }

    [Header("Output")]
    public BoolEvent buttonStateOutputEvent;
    public Event buttonClickOutputEvent;

    [SerializeField] private bool mayBeActivated;

    [ConditionalField("mayBeActivated")] [SerializeField] private bool stateOfActivation;

    private Color buttonPressedColor;

    private Color buttonNormalColor;

    private Button button;

    /*    private void OnValidate()
        {
            if (mayBeActivated)
            {


            }
        }*/

    void Start()
    {
        if (mayBeActivated)
        {
            button = GetComponent<Button>();
            if (button != null)
            {
                buttonPressedColor = button.colors.pressedColor;
                buttonNormalColor = button.colors.normalColor;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (mayBeActivated)
        {
            stateOfActivation = !stateOfActivation;
            OutputButtonStateValue(stateOfActivation);

            ColorBlock colors = button.colors;
            if (stateOfActivation)
            {
                colors.normalColor = buttonPressedColor;
            } else
            {
                colors.normalColor = buttonNormalColor;
            }
            button.colors = colors;
        }
        else
        {
            OutputButtonStateValue(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!mayBeActivated)
        {
            OutputButtonStateValue(false);
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OutputButtonClickEvent();
    }

    void OutputButtonStateValue(bool buttonState)
    {
        buttonStateOutputEvent.Invoke(buttonState);
    }

    void OutputButtonClickEvent()
    {
        buttonClickOutputEvent.Invoke();
    }

}
