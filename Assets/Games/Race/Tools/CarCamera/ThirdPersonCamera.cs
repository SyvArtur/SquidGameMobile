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
    private StarterAssetsInputs _input;

    public void Initialize(GameObject car)
    {
        _car = car;
        _camRotationY = Camera.main.transform.parent.transform.rotation.eulerAngles.y;
        _camRotationZ = _car.transform.rotation.eulerAngles.x;
        _input = GetComponent<StarterAssetsInputs>();
        //car.AddComponent<Camera.main>();
        Camera.main.transform.position = new Vector3(Camera.main.transform.parent.position.x + 6, Camera.main.transform.parent.position.y + 3, Camera.main.transform.parent.position.z);
        Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
        Camera.main.transform.parent.position = new Vector3(_car.transform.position.x, _car.transform.position.y, _car.transform.position.z);
        Camera.main.transform.parent.rotation = Quaternion.Euler(0, 0, 135f);

        

    }

    void FixedUpdate()
    {
        if (_car != null)
            CameraWork();
    }

    public void CameraWork()
    {

        float threshold = 0.01f;
        Camera.main.transform.parent.position = new Vector3 (_car.transform.position.x+1, _car.transform.position.y, _car.transform.position.z);
        if (_input.look.sqrMagnitude >= threshold)
        {
            //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            float deltaTimeMultiplier = 5f;
            _camRotationY += _input.look.x * deltaTimeMultiplier;
            _camRotationZ += -_input.look.y * deltaTimeMultiplier;
        }

        float TopClamp = 155.0f; 
        float BottomClamp = 70.0f; 

        _camRotationZ = ClampAngle(_camRotationZ, BottomClamp, TopClamp);
        _camRotationY = ClampAngle(_camRotationY, float.MinValue, float.MaxValue);

        Camera.main.transform.parent.rotation = Quaternion.Euler(0, _camRotationY, _camRotationZ);

        Camera.main.transform.LookAt(Camera.main.transform.parent.transform);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
