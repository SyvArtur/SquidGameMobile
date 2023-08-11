using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{

    [SerializeField] private WheelAlignment[] _steerableWheels;

    [SerializeField] private float _breakPower;

    [SerializeField] private float _wheelRotateSpeed;
    [SerializeField] private float _wheelSteeringAngle;

    [SerializeField] private float _wheelAcceleration;
    [SerializeField] private float _wheelMaxSpeed;


    [SerializeField] private Rigidbody _carRB;

    // Update is called once per frame
    void Update()
    {
        wheelControl();
    }


    void wheelControl()
    {
        for (int i = 0; i < _steerableWheels.Length; i++)
        {
            _steerableWheels[i].SteeringAngle = Mathf.LerpAngle(_steerableWheels[i].SteeringAngle, 0, Time.deltaTime * _wheelRotateSpeed);
            _steerableWheels[i].WheelCol.motorTorque = -Mathf.Lerp(_steerableWheels[i].WheelCol.motorTorque, 0, Time.deltaTime * _wheelAcceleration);


            float horizontal = -CarController.carInput.Car.WASD.ReadValue<Vector2>().x;
            float vertical = CarController.carInput.Car.WASD.ReadValue<Vector2>().y;

            if (vertical > 0.1)
            {
                _steerableWheels[i].WheelCol.motorTorque = -Mathf.Lerp(_steerableWheels[i].WheelCol.motorTorque, _wheelMaxSpeed, Time.deltaTime * _wheelAcceleration);
            }

            if (vertical < -0.1)
            {
                _steerableWheels[i].WheelCol.motorTorque = Mathf.Lerp(_steerableWheels[i].WheelCol.motorTorque, _wheelMaxSpeed, Time.deltaTime * _wheelAcceleration * _breakPower);
                _carRB.drag = 0.3f;
            }
            else
            {
                _carRB.drag = 0;
            }


            if (horizontal > 0.1)
            {
                _steerableWheels[i].SteeringAngle = Mathf.LerpAngle(_steerableWheels[i].SteeringAngle, -_wheelSteeringAngle, Time.deltaTime * _wheelRotateSpeed);
            }

            if (horizontal < -0.1)
            {
                _steerableWheels[i].SteeringAngle = Mathf.LerpAngle(_steerableWheels[i].SteeringAngle, _wheelSteeringAngle, Time.deltaTime * _wheelRotateSpeed);
            }
        }
    }

}
