using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileCameraPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool pressed = false;
    private CinemachineVirtualCamera cvc;
    private int fingerId;

    public CinemachineVirtualCamera Cvc { get => cvc; set => cvc = value; }

    private void Start()
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            pressed = true;
            //Debug.Log("press");
            fingerId = eventData.pointerId;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        //Debug.Log("up");
        Cvc.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisValue = 0;
        Cvc.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisValue = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if (pressed)
        {
            foreach (Touch touch in Input.touches)
            {
                //Debug.Log(touch.fingerId + "   " + fingerId);
                if (RectTransformUtility.RectangleContainsScreenPoint(gameObject.GetComponent<RectTransform>(), touch.position))
                {
                    /*Debug.Log("cond");
                }
                if (touch.fingerId == fingerId)
                {*/
                    Debug.Log("cond");
                    if (touch.phase == TouchPhase.Moved)
                    {
                        Debug.Log("Moved   " + touch.deltaPosition.y + "   " + touch.deltaPosition.x);

/*                        // ¬ращение вокруг оси Y (горизонтальное вращение)
                        transform.RotateAround(target.position, Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);

                        // ¬ращение вокруг оси X (вертикальное вращение)
                        transform.RotateAround(target.position, transform.right, verticalInput * rotationSpeed * Time.deltaTime);*/

                        Cvc.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = Cvc.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value - (touch.deltaPosition.y/2);
                        Cvc.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = Cvc.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value - (touch.deltaPosition.x/2);
                    }
                    if (touch.phase == TouchPhase.Stationary)
                    {
                        Debug.Log("Stationary");
                        Cvc.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisValue = 0;
                        Cvc.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisValue = 0;
                    }
                }
            }
        }
        
    }
}