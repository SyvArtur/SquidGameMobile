using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ClientLogic_BlockParty : NetworkBehaviour
{
    [SerializeField] private GameObject _timerText;
    [SerializeField] private GameObject _imageUI;
    [SerializeField] private GameObject _observationPlace;

    [HideInInspector] public readonly SyncList<Color> _syncListColors = new SyncList<Color>();
    [HideInInspector] public readonly SyncList<GameObject> _platforms = new SyncList<GameObject>();
    //[HideInInspector] public readonly SyncList<Vector3> _playerStartedPositins = new SyncList<Vector3>();

    [SyncVar(hook = nameof(RightColorChanged))]
    [HideInInspector]
    public Color _rightColor;

    private MaterialPropertyBlock _materialPropertyBlock;

    [SyncVar(hook = nameof(TextTimerChanged))]
    [HideInInspector]
    public string textTimer;



    private void Awake()
    {
        _platforms.Callback += OnAddColorToplatform;
        _materialPropertyBlock = new MaterialPropertyBlock();
    }

    public void SetPlayersPositions(int _sizeMatrixPlatform)
    {
        for (int i = 1; i < _sizeMatrixPlatform - 1; i++)
        {
            for (int j = 1; j < _sizeMatrixPlatform - 1; j++)
            {
                if ((i - 1) * (_sizeMatrixPlatform - 1) + j - 1 < MyNetworkManager.clientObjects.Count)
                {
                    //ClientLogic._playerStartedPositins.Add(new Vector3(i * 3, 8, j * 3));
                    //LobbyNetworkManager.clientObjects[(i - 1) * (_sizeMatrixPlatform - 1) + j - 1].transform.position = new Vector3(i * 3, 8, j * 3);
                    TargetSetPlayerPosition(MyNetworkManager.clientObjects[(i - 1) * (_sizeMatrixPlatform - 1) + j - 1].GetComponent<NetworkIdentity>().connectionToClient, new Vector3(i * 3, 8, j * 3));
                }
            }
        }
    }

    private void RightColorChanged(Color oldColor, Color newColor)
    {
        _imageUI.GetComponent<UnityEngine.UI.Image>().color = newColor;
    }

    private void TextTimerChanged(string oldText, string newText)
    {
        _timerText.GetComponent<Text>().text = newText;
    }


    public void OnAddColorToplatform(SyncList<GameObject>.Operation op, int index, GameObject oldObject, GameObject newObject)
    {
        if (op == SyncList<GameObject>.Operation.OP_ADD)
        {
            //Debug.Log(newObject.transform.position + "  " + _syncListColors[_platforms.Count - 1]);
            //RpcUpdateObjectMaterial(newObject, _syncListColors[_platforms.Count - 1]);
            newObject.GetComponent<Renderer>().GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetColor("_Color", _syncListColors[_platforms.Count - 1]);
            //Debug.Log(newObject.GetComponent<Renderer>().("Color"));
            newObject.GetComponent<Renderer>().SetPropertyBlock(_materialPropertyBlock);
            //newObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);//_materialPropertyBlock.GetColor("Color"));//_syncListColors[_platforms.Count - 1];
        }
        /*else if (op == SyncList<GameObject>.Operation.OP_CLEAR)
        {
            RpcClearObjectsList();
        }*/
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
