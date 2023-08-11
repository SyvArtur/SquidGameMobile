using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticCamera : MonoBehaviour, ICameraOperation
{
    private GameObject _car;

    public void Initialize(GameObject car)
    {
        _car = car;
        Camera.main.GetComponent<Transform>().position = _car.GetComponent<Transform>().position + GetShiftPointTroughAngle(_car);
    }

    void FixedUpdate()
    {
        if (_car != null)
            CameraWork();
    }

    private float _cameraStickiness = 17f;

    public void CameraWork()
    {
        Vector3 shiftPoint = GetShiftPointTroughAngle(_car);
        Transform mainCameraTransform = Camera.main.GetComponent<Transform>();
        Transform carTransform = _car.GetComponent<Transform>();

        mainCameraTransform.position = Vector3.Lerp(mainCameraTransform.position, (carTransform.position + shiftPoint), _cameraStickiness * Time.deltaTime);

        mainCameraTransform.LookAt(carTransform);
    }

    private Vector3 GetShiftPointTroughAngle(GameObject someGameObject)
    {
        Vector3 shiftPoint;
        Transform objectTransform = someGameObject.GetComponent<Transform>();

        if (objectTransform.eulerAngles.y > 45 && objectTransform.eulerAngles.y < 135)
        {
            shiftPoint = new Vector3(5, 3f, 0);
        }
        else

        if (objectTransform.eulerAngles.y > 135 && objectTransform.eulerAngles.y < 225)
        {
            shiftPoint = new Vector3(0, 3f, -5f);
        }
        else

        if (objectTransform.eulerAngles.y > 225 && objectTransform.eulerAngles.y < 315)
        {
            shiftPoint = new Vector3(-5, 3, 0);
        }
        else
        {
            shiftPoint = new Vector3(0, 3, 5f);
        }
        return shiftPoint;
    }
}
