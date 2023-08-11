using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPlayerPosition_Doll : NetworkBehaviour
{
    [SerializeField] private ClientLogic_Doll ClientLogic;

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
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                Vector3 playerPosition = new Vector3(62 - (i * 2), 2, -7 + j * 2);
                if (i * 16 + j  < MyNetworkManager.clientObjects.Count)
                {
                    if (Vector3.Distance(MyNetworkManager.clientObjects[i * 16 + j].transform.position, playerPosition) > 7)
                    {
                        ClientLogic.TargetSetPlayerPosition(MyNetworkManager.clientObjects[i * 16 + j].GetComponent<NetworkIdentity>().connectionToClient, playerPosition);
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
