using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem.OnScreen;
using MyBox;

public class UIVirtualTouchZone : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [System.Serializable]
    public class Event : UnityEvent<Vector2> { }

    [Header("Rect References")]
    public RectTransform containerRect;
    public RectTransform handleRect;

    [Header("Settings")]
    public bool clampToMagnitude;
    public float magnitudeMultiplier = 1f;
    public bool invertXOutputValue;
    public bool invertYOutputValue;

    [SerializeField] private bool workWithCamera;

    /*[ConditionalField("workWithCamera")] */[SerializeField] private GameObject cinemachineCameraTarget;

    //Stored Pointer Values
    private Vector2 pointerDownPosition;
    private Vector2 currentPointerPosition;

    [Header("Output")]
    public Event touchZoneOutputEvent;

    /*[ConditionalField("workWithCamera")] */private float _cinemachineTargetYaw;
    /*[ConditionalField("workWithCamera")] */private float _cinemachineTargetPitch;

    void Start()
    {
        SetupHandle();

        if (workWithCamera)
        {
            _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
        }
    }

    private void SetupHandle()
    {
        if(handleRect)
        {
            SetObjectActiveState(handleRect.gameObject, false); 
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(containerRect, eventData.position, eventData.pressEventCamera, out pointerDownPosition);

        if(handleRect)
        {
            SetObjectActiveState(handleRect.gameObject, true);
            UpdateHandleRectPosition(pointerDownPosition);

            if (!workWithCamera)
            {
                handleRect.gameObject.GetComponent<VirtualJoystick>().HandleStickController.OnPointerDown(eventData);
            }

        }

        
    }



    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(containerRect, eventData.position, eventData.pressEventCamera, out currentPointerPosition);
        

/*        OutputPointerEventValue(outputPosition * magnitudeMultiplier);*/

        if (!workWithCamera)
        {
            Vector2 positionDelta = GetDeltaBetweenPositions(pointerDownPosition, currentPointerPosition);

            Vector2 clampedPosition = ClampValuesToMagnitude(positionDelta);

            Vector2 outputPosition = ApplyInversionFilter(clampedPosition);

            handleRect.gameObject.GetComponent<VirtualJoystick>().HandleStickController.OnDrag(eventData);

            OutputPointerEventValue(outputPosition * magnitudeMultiplier);

            //CameraRotation(outputPosition * magnitudeMultiplier);

        } else
        {
            Vector2 positionDelta = GetDeltaBetweenPositions(pointerDownPosition, currentPointerPosition);

            handleRect.anchoredPosition = currentPointerPosition;

            pointerDownPosition = currentPointerPosition;

            //Vector2 clampedPosition = ClampValuesToMagnitude(positionDelta);

            Vector2 outputPosition = ApplyInversionFilter(positionDelta);

            CameraRotation(outputPosition * magnitudeMultiplier);
        }
    }

    private void CameraRotation(Vector2 outputPosition)
    {
        //Debug.Log(_input.look.sqrMagnitude);


        _cinemachineTargetYaw = cinemachineCameraTarget.transform.eulerAngles.y;
        _cinemachineTargetPitch = cinemachineCameraTarget.transform.eulerAngles.x;
        if (outputPosition.sqrMagnitude >= 0.01)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = Time.deltaTime;

          

            _cinemachineTargetYaw += outputPosition.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += outputPosition.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        float TopClamp = 60.0f;
        float BottomClamp = -60.0f;

       // _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
        // Cinemachine will follow this target
        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle > 270f) lfAngle -= 360f;
        if (lfAngle < -360f) lfAngle += 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);        
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDownPosition = Vector2.zero;
        currentPointerPosition = Vector2.zero;

        OutputPointerEventValue(Vector2.zero);

        if(handleRect)
        {
            SetObjectActiveState(handleRect.gameObject, false);
            //UpdateHandleRectPosition(Vector2.zero);
        }

        if (!workWithCamera)
        {
            handleRect.gameObject.GetComponent<VirtualJoystick>().HandleStickController.OnPointerUp(eventData);
        }
    }

    void OutputPointerEventValue(Vector2 pointerPosition)
    {
        touchZoneOutputEvent.Invoke(pointerPosition);
    }

    void UpdateHandleRectPosition(Vector2 newPosition)
    {
        handleRect.anchoredPosition = newPosition;
    }

    void SetObjectActiveState(GameObject targetObject, bool newState)
    {
        targetObject.SetActive(newState);
    }

    Vector2 GetDeltaBetweenPositions(Vector2 firstPosition, Vector2 secondPosition)
    {
        return secondPosition - firstPosition;
    }

    Vector2 ClampValuesToMagnitude(Vector2 position)
    {
        return Vector2.ClampMagnitude(position, 1);
    }

    Vector2 ApplyInversionFilter(Vector2 position)
    {
        if(invertXOutputValue)
        {
            position.x = InvertValue(position.x);
        }

        if(invertYOutputValue)
        {
            position.y = InvertValue(position.y);
        }

        return position;
    }

    float InvertValue(float value)
    {
        return -value;
    }
    
}
