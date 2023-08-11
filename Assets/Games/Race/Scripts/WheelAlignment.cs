using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelAlignment : MonoBehaviour
{

    [SerializeField] private GameObject _wheelBase;

    [SerializeField] private WheelCollider _wheelCol;

    [SerializeField] private bool _steerable;

    [SerializeField] private float _steeringAngle;

    private RaycastHit hit;

    public WheelCollider WheelCol { get => _wheelCol; set => _wheelCol = value; }
    public float SteeringAngle { get => _steeringAngle; set => _steeringAngle = value; }

    void Update()
    {
        AlignMeshToCollider();
    }

    void AlignMeshToCollider()
    {
        if (Physics.Raycast(WheelCol.transform.position, -WheelCol.transform.up, out hit, WheelCol.suspensionDistance + WheelCol.radius))
        {
            gameObject.transform.position = hit.point + WheelCol.transform.up * WheelCol.radius;
        }
        else
        {
            gameObject.transform.position = WheelCol.transform.position - (WheelCol.transform.up * WheelCol.suspensionDistance);
        }

        if (_steerable)
        {
            WheelCol.steerAngle = SteeringAngle;
        }

        gameObject.transform.eulerAngles = new Vector3(_wheelBase.transform.eulerAngles.x, _wheelBase.transform.eulerAngles.y + WheelCol.steerAngle, _wheelBase.transform.eulerAngles.z);
        gameObject.transform.Rotate(WheelCol.rpm / 60 * 360 * Time.deltaTime, 0, 0);
    }
}
