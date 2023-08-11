using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformsActions : NetworkBehaviour
{
    [SerializeField] private GameObject _prefabBlock;
    [SerializeField] public float _timeBeforeDestroy = 8f;
    [SerializeField] private KeyEvents_UltimateKnockout _keyEvents;
    [SerializeField] private MaterialActions _materialActions;
    [SerializeField] private ClientLogic_UltimateKnockout ClientLogic;

    private GameObject[,] _platforms;
    public static int _sizeMatrixPlatform = 5;


    private void Awake()
    {
        _keyEvents.SubscribeToChoosePlatformForPlayer(() =>
        {
           ResetMaterialsFromPlatforms();
           StartCoroutine(CountdownToDestroyPlatforms());
        });

        _keyEvents.SubscribeToRevising(() =>
        {
            DestroyAllPlatforms();
            CreatePlatforms();
        });

        _keyEvents.SubscribeToNewRound(() =>
        {
            PaintPlatforms();
        });
    }

/*    IEnumerator Start()
    {
        if (isServer)
        {
            while (!MyNetworkManager.allClientsReady) {
                yield return new WaitForEndOfFrame();
            }

        }
    }*/


    private void ResetMaterialsFromPlatforms()
    {
        for (int i = 0; i < _sizeMatrixPlatform; i++)
        {
            for (int j = 0; j < _sizeMatrixPlatform; j++)
            {
                ClientLogic.RpcResetMaterialToObject(_platforms[i, j]);
                //_platforms[i, j].GetComponent<MeshRenderer>().material = _prefabBlock.GetComponent<MeshRenderer>().sharedMaterial;
                //platform.AddComponent<MyCollisionDetector>();
            }
        }
    }


    private void CreatePlatforms()
    {
        _platforms = new GameObject[_sizeMatrixPlatform, _sizeMatrixPlatform];
        for (int i = 0; i < _sizeMatrixPlatform; i++)
        {
            for (int j = 0; j < _sizeMatrixPlatform; j++)
            {
                _platforms[i, j] = Instantiate(_prefabBlock, new Vector3(i * 3.4f, -29f, j * 3.4f), Quaternion.Euler(0, 0, 0));
                NetworkServer.Spawn(_platforms[i, j]);
                //platform.AddComponent<MyCollisionDetector>();
            }
        }
    }

    private void PaintPlatforms()
    {
        for (int i = 0; i < _sizeMatrixPlatform; i++)
        {
            for (int j = 0; j < _sizeMatrixPlatform; j++)
            {
                ClientLogic.RpcSetMaterialToObject(_platforms[i, j], i, j);
                //_platforms[i, j].GetComponent<MeshRenderer>().material = _materialActions._materials[j + i * _sizeMatrixPlatform];
            }
        }
    }


    private void DestroyPlatformsByMaterial()
    {
        for (int i = 0; i < _sizeMatrixPlatform; i++)
        {
            for (int j = 0; j < _sizeMatrixPlatform; j++)
            {
                if (!Equals(ClientLogic._materialForPlatforms[i * _sizeMatrixPlatform + j], ClientLogic._materialForPlatforms[_materialActions._correctIdMaterial]))
                {
                    ClientLogic.RpcDestroyObject(_platforms[i, j]);
                    //Destroy(_platforms[i, j]);
                }
            }
        }
    }

    private void DestroyAllPlatforms()
    {
        if (_platforms != null)
        {
            for (int i = 0; i < _sizeMatrixPlatform; i++)
            {
                for (int j = 0; j < _sizeMatrixPlatform; j++)
                {
                    Destroy(_platforms[i, j]);
                }
            }
        }
    }

    private IEnumerator CountdownToDestroyPlatforms()
    {
        yield return new WaitForSeconds(_timeBeforeDestroy);
        DestroyPlatformsByMaterial();
        PaintPlatforms();
    }

}
