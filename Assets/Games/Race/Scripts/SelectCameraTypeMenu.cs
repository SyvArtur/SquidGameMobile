using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SelectCameraTypeMenu : MonoBehaviour
{
    private CreatorCamera _creatorCamera;
    private bool _cameraTypeSelected = false;
    [SerializeField] private GameObject _car;
    [SerializeField] private GameObject _startMenu;
    //[SerializeField] private AudioSource SkidSound;
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        AudioListener.pause = true;
        
        Time.timeScale = 0;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && _startMenu.active)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } 
    }

    public void SelectedFirstPersonCamera()
    {
        _creatorCamera = new CreatorFirstPersonCamera();
        CameraSelected();
    }

    public void SelectedThirdPersonCamera()
    {
        _creatorCamera = new CreatorThirdPersonCamera();
        CameraSelected();
    }

    public void SelectedAutomaticCamera()
    {
        _creatorCamera = new CreatorAutomaticCamera();
        CameraSelected();
    }
    
    private void CameraSelected()
    {
        _creatorCamera.Initialize(_car);
        _cameraTypeSelected = true;
        _startMenu.SetActive(false);
        Time.timeScale = 1;
        AudioListener.pause = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

}
