using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SpeedCalculator : MonoBehaviour
{
    public static float Speed;

    [SerializeField] private Rigidbody _car;

    [SerializeField] private Text _speedText;


    void FixedUpdate()
    {
        Speed = _car.velocity.magnitude * 2.23693629f;

        _speedText.text = Speed.ToString("0");
    }

}
