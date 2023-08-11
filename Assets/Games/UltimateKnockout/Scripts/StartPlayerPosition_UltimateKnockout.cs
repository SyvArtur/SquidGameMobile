using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPlayerPosition_UltimateKnockout : NetworkBehaviour
{
    [SerializeField] private ClientLogic_UltimateKnockout ClientLogic;
    
    IEnumerator Start()
    {
        if (isServer)
        {
            while (!MyNetworkManager.allClientsReady)
            {
                yield return new WaitForEndOfFrame();
            }

            SetPlayersPositions();
        }
    }

    private void SetPlayersPositions()
    {
        bool allPlayersInPosition = true;
        for (int i = 0; i <= 7; i++)
        {
            for (int j = 0; j <= 7; j++)
            {
                Vector3 playerPosition = new Vector3(i * 2, -25, j * 2);
                if (i * 8 + j < MyNetworkManager.clientObjects.Count)
                {
                    if (Vector3.Distance(MyNetworkManager.clientObjects[i * 8 + j].transform.position, playerPosition) > 7)
                    {
                        ClientLogic.TargetSetPlayerPosition(MyNetworkManager.clientObjects[i * 8 + j].GetComponent<NetworkIdentity>().connectionToClient, playerPosition);
                        allPlayersInPosition = false;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        if (!allPlayersInPosition)
        {
            Invoke(nameof(SetPlayersPositions), 0.018f);
        }
    }
}
