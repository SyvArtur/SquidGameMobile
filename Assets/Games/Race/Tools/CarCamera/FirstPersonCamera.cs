using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour, ICameraOperation
{
    private GameObject _car;
    private float _camRotationY;
    private float _camRotationX;
    private StarterAssetsInputs _input;
    public void Initialize(GameObject car)
    {
        _car = car;
        _camRotationY = Camera.main.transform.parent.transform.rotation.eulerAngles.y;
        _camRotationX = _car.transform.rotation.eulerAngles.x;
        _input = GetComponent<StarterAssetsInputs>();
        //car.AddComponent<Camera.main>();
        //Camera.main.transform.position = new Vector3(Camera.main.transform.parent.position.x + 6, Camera.main.transform.parent.position.y + 3, Camera.main.transform.parent.position.z);
        //_input.look.x = 180;
        

        //Camera.main.transform.parent.rotation = Quaternion.Euler(0, 0, 0);
        Camera.main.transform.SetParent(_car.transform);
        Camera.main.transform.position = new Vector3(_car.transform.position.x + 0.4f, _car.transform.position.y + 0.85f, _car.transform.position.z + 0.65f);
        Camera.main.transform.rotation = Quaternion.Euler(_camRotationX, _car.transform.rotation.eulerAngles.y + 180, 0);

    }

    void FixedUpdate()
    {
        if (_car != null)
            CameraWork();
    }

    public void CameraWork()
    {
        //Debug.Log(_input.look.sqrMagnitude);
        float threshold = 0.01f;
        //Camera.main.transform.parent.position = new Vector3(_car.transform.position.x+0.35f, _car.transform.position.y+1f, _car.transform.position.z+0.65f);
        if (_input.look.sqrMagnitude >= threshold)
        {
            //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            float deltaTimeMultiplier = 5f;
            _camRotationY += _input.look.x * deltaTimeMultiplier;
            _camRotationX += _input.look.y * deltaTimeMultiplier;

            float TopClamp = 80.0f;
            float BottomClamp = -50.0f;
            _camRotationX = ClampAngle(_camRotationX, BottomClamp, TopClamp);
            _camRotationY = ClampAngle(_camRotationY, float.MinValue, float.MaxValue);

            Camera.main.transform.rotation = Quaternion.Euler(_camRotationX, _car.transform.rotation.eulerAngles.y + 180 + _camRotationY, 0);
        }
        //_camRotationX += _car.transform.rotation.eulerAngles.y + _input.look.y * deltaTimeMultiplier;



        //Camera.main.transform.LookAt(Camera.main.transform.parent.transform);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
