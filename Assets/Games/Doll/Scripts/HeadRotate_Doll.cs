
using Mirror;
using UnityEngine;

public class HeadRotate_Doll : NetworkBehaviour
{

    [SerializeField] private GameObject _head;
    //[SerializeField] private ClientLogic_Doll ClientLogic;

    [HideInInspector] private Vector3 _rotateTo = new Vector3();
    [HideInInspector] private bool _rotating = false;

    private void Start()
    {
/*        if (isServer)
        {*/
            Sound_Doll soundDool = gameObject.GetComponent<Sound_Doll>();
            //_head = gameObject;
            soundDool.SubscribeToRepeatSound(RotateAwayFromPlayer);
            soundDool.SubscribeToScanning(RotateTOPlayer);
        //}
    }
    private void RotateAwayFromPlayer()
    {
        _rotateTo.y = 180;
        _rotating = true;
    }

    private void RotateTOPlayer()
    {
        _rotateTo.y = 0;
        _rotating = true;
    }

    private void HeadRotate()
    {
        if (_rotating)
        {
            if (Vector3.SqrMagnitude(_head.transform.eulerAngles - _rotateTo) > 0.01f)
            {
                _head.transform.eulerAngles = Vector3.Lerp(_head.transform.rotation.eulerAngles, _rotateTo, Time.deltaTime * 4);
            }
            else
            {
                _head.transform.eulerAngles = _rotateTo;
                _rotating = false;
            }
        }
    }


    public void Update()
    {
        /*if (isServer)
        {*/
            HeadRotate();
        //}
    }
}
