using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaKills : NetworkBehaviour
{
    [SerializeField] private ClientLogic_UltimateKnockout ClientLogic;
    [SerializeField] private CheckForWin CheckForWin;
    //private int inactivePlayers = 0;
    //private object lockObject = new object();

    void OnTriggerEnter(Collider other)
    {
        //Destroy(other.gameObject);
        if (other.gameObject.CompareTag("Player"))
        {
            ClientLogic.TargetCameraObservation(other.gameObject.GetComponent<NetworkIdentity>().connectionToClient);
            other.gameObject.GetComponent<PlayerProperties>().playerActive = false;
            CheckForWin.CountOfInnactivePLayers++;
        }
        
    }

}
