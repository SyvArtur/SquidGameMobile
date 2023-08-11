using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class ClientLogic_UltimateKnockout : NetworkBehaviour
{
    [SerializeField] private GameObject _observationPlace;
    [SerializeField] private Material[] _pictures;
    [SerializeField] private Text _timerText;
    [SerializeField] private MeshRenderer _prefabMaterial;

    [HideInInspector] public Material[] _materialForPlatforms;



    [ClientRpc]
    public void RpcExchangeElementOfMaterialList(int i, int j)
    {
        Material tmp = _materialForPlatforms[j];
        _materialForPlatforms[j] = _materialForPlatforms[i];
        _materialForPlatforms[i] = tmp;
    }


    [ClientRpc]
    public void RpcSetPropertyMaterials()
    {
        _materialForPlatforms = new Material[PlatformsActions._sizeMatrixPlatform * PlatformsActions._sizeMatrixPlatform];

        for (int i = 0; i < PlatformsActions._sizeMatrixPlatform * PlatformsActions._sizeMatrixPlatform; i++)
        {
            _materialForPlatforms[i] = _pictures[i % _pictures.Length];
        }
    }

    [ClientRpc]
    public void RpcResetScreens(GameObject[] _screens)
    {
        for (int i = 0; i < _screens.Length; i++)
        {
            _screens[i].GetComponent<MeshRenderer>().material = null;
            _screens[i].GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(221f, 221f, 221f);
        }
    }

    [ClientRpc]
    public void RpcTurnOnScreens(GameObject[] _screens, int newIdMaterial)
    {
        float tillingX = 1.6f;
        for (int i = 0; i < _screens.Length; i++)
        {
            _screens[i].GetComponent<MeshRenderer>().material = new Material(_materialForPlatforms[newIdMaterial]);
            _screens[i].GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(transform.localScale.x * tillingX, transform.localScale.y * 1);
            _screens[i].GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2((1 - tillingX) / 2, 0));
        }
    }

    [ClientRpc]
    public void RpcSetMaterialToObject(GameObject myObject, int i, int j)
    {
        myObject.GetComponent<MeshRenderer>().material = _materialForPlatforms[j + i * PlatformsActions._sizeMatrixPlatform];
    }

    [ClientRpc]
    public void RpcResetMaterialToObject(GameObject myObject)
    {
        myObject.GetComponent<MeshRenderer>().material = _prefabMaterial.sharedMaterial;
    }

    [ClientRpc]
    public void RpcDestroyObject(GameObject myObject)
    {
        Destroy(myObject);
    }

    [ClientRpc]
    public void RpcSetTimerText(string myText)
    {
        _timerText.text = myText;
    }

    [TargetRpc]
    public void TargetCameraObservation(NetworkConnectionToClient conn)
    {
        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        virtualCamera.Follow = _observationPlace.transform;
    }

    [TargetRpc]
    public void TargetSetPlayerPosition(NetworkConnectionToClient conn, Vector3 newPosition)
    {
        conn.identity.gameObject.transform.position = newPosition;
    }
}

