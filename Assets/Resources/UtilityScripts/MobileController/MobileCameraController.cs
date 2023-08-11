using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileCameraController : NetworkBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject cinemachineCameraTarget;
    private MobileCameraPanel mobileCameraPanel;
    public CinemachineVirtualCamera cvc;
    [SerializeField] private float sensitivity;

    void Start()
    {
        if (isLocalPlayer)
        {
            mobileCameraPanel = panel.AddComponent<MobileCameraPanel>();
            BindCameraToPlayer();
            mobileCameraPanel.Cvc = cvc;
        }
    }

    private void BindCameraToPlayer()
    {
        cvc = FindObjectOfType<CinemachineVirtualCamera>();
        cvc.Follow = cinemachineCameraTarget.transform;
        cvc.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = 1;
        cvc.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = 1;
        cvc.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisName = "";
        cvc.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisName = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
