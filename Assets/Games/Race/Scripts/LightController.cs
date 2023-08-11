using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Material _lightMeterial;
    public float _brake;


    void Update()
    {
        _brake = CarController.carInput.Car.WASD.ReadValue<Vector2>().y;
        LightManager();
    }

    public void LightManager()
    {
        if(_brake <= -0.1f)
        {
            _lightMeterial.EnableKeyword("_EMISSION");
        }
        else
        {
            _lightMeterial.DisableKeyword("_EMISSION");
        }

    }
}
