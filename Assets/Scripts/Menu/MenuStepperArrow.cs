using UnityEngine;
using UnityEngine.EventSystems;

public class MenuStepperArrow : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MenuStepper _stepper;
    [SerializeField] private int _direction = 1;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || _stepper == null)
            return;

        _stepper.Focus();
        _stepper.Nudge(_direction);
    }
}
