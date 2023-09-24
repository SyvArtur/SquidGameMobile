using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

class ThirdPersonCamera : MonoBehaviour, ICameraOperation
{
    private GameObject _car;
    private float _camRotationY;
    private float _camRotationZ;
    //private StarterAssetsInputs _input;
    private Camera _cameraMain;

    public void Initialize(GameObject car)
    {
        _car = car;
        _camRotationY = Camera.main.transform.parent.transform.rotation.eulerAngles.y;
        _camRotationZ = _car.transform.rotation.eulerAngles.x;
        //_input = GetComponent<StarterAssetsInputs>();
        //car.AddComponent<Camera.main>();
        _cameraMain = Camera.main;
        _cameraMain.transform.position = new Vector3(Camera.main.transform.parent.position.x + 6, Camera.main.transform.parent.position.y + 3, Camera.main.transform.parent.position.z);
        _cameraMain.transform.rotation = Quaternion.Euler(0, 0, 0);
        _cameraMain.transform.parent.position = new Vector3(_car.transform.position.x, _car.transform.position.y, _car.transform.position.z);
        _cameraMain.transform.parent.rotation = Quaternion.Euler(0, 0, 135f);

        

    }

    void FixedUpdate()
    {
        if (_car != null)
            CameraWork();
    }

    public void CameraWork()
    {

        float threshold = 0.01f;
        _cameraMain.transform.parent.position = new Vector3 (_car.transform.position.x+1, _car.transform.position.y, _car.transform.position.z);
        if (UITouchZoneRace._deltaPosition.sqrMagnitude >= threshold)
        {
            //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            float deltaTimeMultiplier = 1f;
            _camRotationY += UITouchZoneRace._deltaPosition.x * deltaTimeMultiplier;
            _camRotationZ += -UITouchZoneRace._deltaPosition.y * deltaTimeMultiplier;
        }

        float TopClamp = 155.0f; 
        float BottomClamp = 70.0f; 

        _camRotationZ = ClampAngle(_camRotationZ, BottomClamp, TopClamp);
        _camRotationY = ClampAngle(_camRotationY, float.MinValue, float.MaxValue);

        _cameraMain.transform.parent.rotation = Quaternion.Euler(0, _camRotationY, _camRotationZ);

        _cameraMain.transform.LookAt(_cameraMain.transform.parent.transform);
        UITouchZoneRace._deltaPosition = new Vector2(0, 0);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
