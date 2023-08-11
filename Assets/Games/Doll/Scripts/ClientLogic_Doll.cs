using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ClientLogic_Doll : NetworkBehaviour
{
    [SerializeField] private Text _timerText;
    [SerializeField] public AudioSource _kukulaku;
    [SerializeField] private AudioSource _shootSound;

    [TargetRpc]
    public void TargetSetPlayerPosition(NetworkConnectionToClient conn, Vector3 newPosition)
    {
        conn.identity.gameObject.transform.position = newPosition;
    }

    [ClientRpc]
    public void RpcPlaySound()
    {
        _kukulaku.Play();
    }

    [TargetRpc]
    public void TargetChangeAnimationDead(NetworkConnectionToClient conn, int animationDeathID, bool dead)
    {
        conn.identity.gameObject.GetComponent<Animator>().SetBool(animationDeathID, dead);

    }

    [ClientRpc]
    public void RpcShootSound()
    {
        _shootSound.PlayOneShot(_shootSound.clip);
    }

    [ClientRpc]
    public void RpcCountdown(float timeLeft)
    {
        //float minutes = Mathf.FloorToInt(_timeLeft / 60);
        float seconds = Mathf.FloorToInt(timeLeft % 60);
        _timerText.text = string.Format("{0:00}", /*minutes,*/ seconds);
    }

    [ClientRpc]
    public void HeadRotate(GameObject head, Vector3 rotateTo)
    {
        if (Vector3.SqrMagnitude(head.transform.eulerAngles - rotateTo) > 0.01f)
        {
            head.transform.eulerAngles = Vector3.Lerp(head.transform.rotation.eulerAngles, rotateTo, Time.deltaTime * 4);
        }
        else
        {
            head.transform.eulerAngles = rotateTo;
        }
    }
}
