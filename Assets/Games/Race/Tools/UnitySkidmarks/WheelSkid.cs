using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(WheelCollider))]
public class WheelSkid : MonoBehaviour
{

    [SerializeField] private AudioSource _SkidSound;
    [SerializeField] private float _SkidSoundMultiplyer;

    [SerializeField] private Rigidbody _carRB;
    //[SerializeField] private Skidmarks _skidmarksController;
    //[SerializeField] private GameObject cub;

    private WheelCollider _wheelCollider;
    private WheelHit _wheelHitInfo;

    private const float SKID_FX_SPEED = 0.5f; // Min side slip speed in m/s to start showing a skid
    private const float MAX_SKID_INTENSITY = 20.0f; // m/s where skid opacity is at full intensity
    private const float WHEEL_SLIP_MULTIPLIER = 10.0f; // For wheelspin. Adjust how much skids show
    private int lastSkid = -1; // Array index for the skidmarks controller. Index of last skidmark piece this wheel used
    private float lastFixedUpdateTime;


    void Awake()
    {
        _wheelCollider = GetComponent<WheelCollider>();
        lastFixedUpdateTime = Time.time;
    }

    void FixedUpdate()
    {
        lastFixedUpdateTime = Time.time;
        _wheelCollider.GetGroundHit(out _wheelHitInfo);
        if (_wheelCollider.GetGroundHit(out _wheelHitInfo))
		{
        // Check sideways speed

        // Gives velocity with +z being the _car's forward axis
        Vector3 localVelocity = transform.InverseTransformDirection(_carRB.velocity);
        float skidTotal = Mathf.Abs(localVelocity.x);
        // Check wheel spin as well

        float wheelAngularVelocity = _wheelCollider.radius * ((2 * Mathf.PI * _wheelCollider.rpm) / 60);
        float carForwardVel = Vector3.Dot(_carRB.velocity, transform.forward);
        float wheelSpin = Mathf.Abs(carForwardVel - wheelAngularVelocity) * WHEEL_SLIP_MULTIPLIER;

        // NOTE: This extra line should not be needed and you can take it out if you have decent wheel physics
        // The built-in Unity demo _car is actually skidding its wheels the ENTIRE timeForTimer you're accelerating,
        // so this fades out the wheelspin-based skid as speed increases to make it look almost OK
        wheelSpin = Mathf.Max(0, wheelSpin * (10 - Mathf.Abs(carForwardVel)));

        skidTotal += wheelSpin;

            // Skid if we should
            if (skidTotal >= SKID_FX_SPEED)
            {
                float intensity;

                if (SpeedCalculator.Speed <= 25)
                {
                    intensity = 0;
                }
                else
                {
                    intensity = Mathf.Clamp01(skidTotal / MAX_SKID_INTENSITY);
                }

                // Account for further movement since the last FixedUpdate
                //_carRB.velocity.Set(_wheelHitInfo.point.x-10, _wheelHitInfo.point.y, _wheelHitInfo.point.z);
                Vector3 skidPoint = _wheelHitInfo.point + (_carRB.velocity * (Time.time - lastFixedUpdateTime));
                //Debug.Log(_carRB.velocity);
                //Vector3 posCar = skidPoint;
                //posCar.x = posCar.x - 30;
                //cub.transform.position = posCar;
                //lastSkid = _skidmarksController.AddSkidMark(new Vector3(50, 50, 50), _wheelHitInfo.normal, 50, lastSkid);
                //Debug.Log(skidPoint + "   " + "   " + _wheelHitInfo.normal + "   " + lastSkid);
                //lastSkid = _skidmarksController.AddSkidMark(skidPoint, _wheelHitInfo.normal, intensity, lastSkid);
                _SkidSound.volume = intensity / 9.5f / _SkidSoundMultiplyer;

            }
            else
            {
                lastSkid = -1;
            }
                }
                else {

                    lastSkid = -1;
                }
        //lastSkid = _skidmarksController.AddSkidMark(new Vector3(50, 50, 50), _wheelHitInfo.normal, 50, lastSkid);
        //}

    }
}
