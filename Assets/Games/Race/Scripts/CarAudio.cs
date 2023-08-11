using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAudio : MonoBehaviour
{
    public AudioSource Engine;
    public AudioSource GearChangeSound;

    public float pitchOffSet1;
    public float pitchOffSet2;
    public float pitchOffSet3;
    public float pitchOffSet4;

    private float gearChange;

    void Update()
    {
        PitchControl();
        EngineVolume();
    }


    public void EngineVolume()
    {
        if (CarController.carInput.Car.WASD.ReadValue<Vector2>().y == 1)
        {
            Engine.volume += Time.deltaTime;
        }
        else
        {
            if (Engine.volume > 0.1f)
            {
                Engine.volume -= Time.deltaTime;
            }
        }      
    }


    private void GearChange(float gearNumber)
    {
        if (gearChange == gearNumber)
        {
            return;
        }
        gearChange = gearNumber;
        GearChangeSound.Play();
    }

    public void PitchControl()
    {
        if (SpeedCalculator.Speed > 0 & SpeedCalculator.Speed < 60)
        {
            GearChange(1);
            Engine.pitch = SpeedCalculator.Speed * pitchOffSet1;
        }

        /*if (SpeedCalculator.Speed > 30 & SpeedCalculator.Speed < 60)
        {
            Engine.pitch = SpeedCalculator.Speed * PitchOffSet2;
        }*/

        if (SpeedCalculator.Speed > 60 & SpeedCalculator.Speed < 120)
        {
            GearChange(2);
            Engine.pitch = SpeedCalculator.Speed * pitchOffSet2;
        }

        /*if (SpeedCalculator.Speed > 90 & SpeedCalculator.Speed < 120)
        {
            Engine.pitch = SpeedCalculator.Speed * PitchOffSet4;
        }*/

        if (SpeedCalculator.Speed > 120 & SpeedCalculator.Speed < 180)
        {
            GearChange(3);
            Engine.pitch = SpeedCalculator.Speed * pitchOffSet3;
        }

        if (SpeedCalculator.Speed > 180)
        {
            GearChange(4);
            Engine.pitch = SpeedCalculator.Speed * pitchOffSet4;
        }
    }

}
