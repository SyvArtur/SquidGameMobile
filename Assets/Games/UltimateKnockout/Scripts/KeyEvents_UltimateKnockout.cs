using Cinemachine;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class KeyEvents_UltimateKnockout : NetworkBehaviour
{
    [SerializeField] private float _timeToRemember = 13f;
    [SerializeField] private GameObject _player;
    [SerializeField] private ClientLogic_UltimateKnockout ClientLogic;
    [SerializeField] private PlatformsActions PlatformsActions;
    private float _timeLeft;

    private delegate void NewRound();
    private delegate void ChoosePlatformForPlayer();
    private delegate void Revising();

    private NewRound newRound;
    private ChoosePlatformForPlayer choosePlatformForPlayer;
    private Revising revising;

    public void SubscribeToRevising(Action action)
    {
        revising += new Revising(action);
    }

    public void SubscribeToNewRound(Action action)
    {
        newRound += new NewRound(action);
    }

    public void SubscribeToChoosePlatformForPlayer(Action action)
    {
        choosePlatformForPlayer += new ChoosePlatformForPlayer(action);
    }

    private void Awake()
    {
        SubscribeToChoosePlatformForPlayer(() =>
        {
            StartCoroutine(WaitSecondsAndRun(PlatformsActions._timeBeforeDestroy + 4f, StartNewRound));
        });

        SubscribeToNewRound(() =>
        {
            _timeLeft = _timeToRemember;
            if (_timeToRemember > 2) _timeToRemember--;
            StartCoroutine(CountdownToChooseRightMaterial());
        });
    }

    IEnumerator Start()
    {
        if (isServer)
        {
            while (!MyNetworkManager.allClientsReady)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(0.05f);

            StartNewRound();
        }
        
    }

    private void StartNewRound()
    {
        revising();
        //Invoke(nameof(newRound), 3.7f);
        StartCoroutine(WaitSecondsAndRun(3.7f, () => newRound()));
    }

    
    private IEnumerator CountdownToChooseRightMaterial()
    {
        if (_timeLeft < 0)
        {
            choosePlatformForPlayer();
            //StartCoroutine(WaitSecondsAndRun(4, () => StartNewRound()));
        }
        else
        {
            string seconds = string.Format("{0:0}", Mathf.FloorToInt(_timeLeft % 60));
            ClientLogic.RpcSetTimerText(seconds);
            _timeLeft -= 1;
            yield return new WaitForSeconds(1);
            StartCoroutine(CountdownToChooseRightMaterial());
        }
    }


    private IEnumerator WaitSecondsAndRun(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }
}
