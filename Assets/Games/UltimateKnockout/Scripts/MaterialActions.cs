using Cinemachine;
using Mirror;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class MaterialActions : NetworkBehaviour
{
    [SerializeField] private KeyEvents_UltimateKnockout _keyEvents;
    [SerializeField] private ClientLogic_UltimateKnockout ClientLogic;
    [SerializeField] private GameObject[] _screens;
    [HideInInspector] public int _correctIdMaterial;


    IEnumerator Start()
    {
        if (isServer)
        {
            _keyEvents.SubscribeToChoosePlatformForPlayer(() =>
            {
                _correctIdMaterial = Random.Range(0, ClientLogic./*_pictures*/_materialForPlatforms.Length);
                ClientLogic.RpcTurnOnScreens(_screens, _correctIdMaterial);
            });


            _keyEvents.SubscribeToRevising(() =>
            {
                Shuffle();
                ClientLogic.RpcResetScreens(_screens);
            });

            while (!MyNetworkManager.allClientsReady)
            {
                yield return new WaitForEndOfFrame();
            }

            ClientLogic.RpcSetPropertyMaterials();
        }

    }


    private void Shuffle()
    {
        for (int i = ClientLogic._materialForPlatforms.Length - 1; i >= 1; i--)
        {
            int j = Random.Range(0, i);
            ClientLogic.RpcExchangeElementOfMaterialList(i, j);
        }
    }

}
