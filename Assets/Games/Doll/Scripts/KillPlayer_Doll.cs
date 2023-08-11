using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class KillPlayer_Doll : NetworkBehaviour
{
    [SerializeField] private float _time;
    [SerializeField] private GameObject _lineWin;
    [SerializeField] private ClientLogic_Doll ClientLogic;
    //[SerializeField] private GameObject _player;
    //private Animator _animator;
    private float _timeLeft = 0f;
    private int _animIDDeath;
    private int _animIDSpeed;
    private bool[] _playerDead;
    private bool _scan = false;
    private Sound_Doll _soundDool;


    IEnumerator Start()
    {
        if (isServer)
        {
            _soundDool = gameObject.GetComponent<Sound_Doll>();
            _playerDead = new bool[MyNetworkManager.clientObjects.Count];
            Array.Fill(_playerDead, false);
            //Animator animator = GetComponent<Animator>();

            _animIDDeath = Animator.StringToHash("Kill");
            _animIDSpeed = Animator.StringToHash("Speed");

            _soundDool.SubscribeToScanning(() =>
            {
                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    _scan = true;
                }
                );
            });

            _soundDool.SubscribeToRepeatSound(() => _scan = false);

            while (!MyNetworkManager.allClientsReady)
            {
                yield return new WaitForEndOfFrame();
            }
            _timeLeft = _time;
            StartCoroutine(StartTimer());
        }
    }
   /* private void Start()
    {

    }*/

    private IEnumerator StartTimer()
    {
        _timeLeft -= 1;
        ClientLogic.RpcCountdown(_timeLeft);
        yield return new WaitForSeconds(1);
        
        if (_timeLeft > 0)
        {
            StartCoroutine(StartTimer());
        }
        else
        {
            for (int i = 0; i < MyNetworkManager.clientObjects.Count; i++)
            {
                if (!_playerDead[i] & MyNetworkManager.clientObjects[i].transform.position.x < _lineWin.transform.position.x)
                {
                    _playerDead[i] = true;
                    ClientLogic.TargetChangeAnimationDead(MyNetworkManager.clientObjects[i].GetComponent<NetworkIdentity>().connectionToClient, _animIDDeath, _playerDead[i]);
                    ClientLogic.RpcShootSound();
                }
            }
            Invoke(nameof(GameOver), 4f);
        }
        
    }

    private void ChangeScene()
    {
        NetworkManager.singleton.ServerChangeScene("Lobby");
    }

    private void ScanningMotion()
    {
        if (_scan)
        {
            for (int i = 0; i < MyNetworkManager.clientObjects.Count; i++)
            {
                if ((MyNetworkManager.clientObjects[i].GetComponent<Animator>().GetFloat(_animIDSpeed) > 0.001f |
                    MyNetworkManager.clientObjects[i].transform.position.y > 0.1) & !_playerDead[i] &
                    (MyNetworkManager.clientObjects[i].transform.position.x < _lineWin.transform.position.x))
                {
                    _playerDead[i] = true;
                    ClientLogic.TargetChangeAnimationDead(MyNetworkManager.clientObjects[i].GetComponent<NetworkIdentity>().connectionToClient, _animIDDeath, _playerDead[i]);
                    ClientLogic.RpcShootSound();
                }
            }
        }
    }

    private void GameOver()
    {
        for (int i = 0; i < MyNetworkManager.clientObjects.Count; i++)
        {
            if (!_playerDead[i])
            {
                PlayerProperties playerProperties = MyNetworkManager.clientObjects[i].GetComponent<PlayerProperties>();
                playerProperties.countOfWins++;
                playerProperties.UpdatePlayerNameFromServer();
            }
            ClientLogic.TargetChangeAnimationDead(MyNetworkManager.clientObjects[i].GetComponent<NetworkIdentity>().connectionToClient, _animIDDeath, false);
        }
        Invoke(nameof(ChangeScene), 0.6f);
    }

    void Update()
    {
        if (isServer)
        {
            ScanningMotion();
        }
    }
}
