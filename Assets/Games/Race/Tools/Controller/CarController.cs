using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarController : MonoBehaviour
{
    public static CarInput carInput;

    void Awake()
    {
        carInput = new CarInput();
    }

    private void OnEnable()
    {
        carInput.Enable();
    }

    private void OnDisable()
    {
        carInput.Disable();
    }

    void Update()
    {
        //Debug.Log(carInput.Car.WASD.ReadValue<Vector2>().x + carInput.Car.WASD.ReadValue<Vector2>().y);
    }
}
