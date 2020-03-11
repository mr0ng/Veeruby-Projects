using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GenericNetSync : MonoBehaviourPun, IPunObservable
{






    public bool isUser;

    public Vector3 startingLocalPosition;
    public Quaternion startingLocalRotation;
    public Vector3 startingScale;

    private Vector3 networkLocalPosition;
    private Quaternion networkLocalRotation;
    private Vector3 networkLocalScale;

    private PhotonView PV;
    private Camera mainCamera;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        mainCamera = Camera.main;

        if (!PV.IsMine && isUser)
        {
            transform.parent = FindObjectOfType<TableAnchor>().transform;
        }
        else if (PV.IsMine && isUser)
        {
            transform.parent = FindObjectOfType<TableAnchor>().transform;
            GenericNetworkManager.instance.localUser = PV;
        }

        startingLocalPosition = transform.localPosition;
        startingLocalRotation = transform.localRotation;
        startingScale = transform.localScale;

        networkLocalPosition = startingLocalPosition;
        networkLocalRotation = startingLocalRotation;
        networkLocalScale = startingScale;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Debug.Log("GenericNetSync.IPunObservable.OnPhotonSerializeView() -> stream.IsWriting");
            stream.SendNext(transform.localPosition);
            stream.SendNext(transform.localRotation);
            //stream.SendNext(transform.localScale);
        }
        else
        {
            //Debug.Log("GenericNetSync.IPunObservable.OnPhotonSerializeView() -> !stream.IsWriting");
            networkLocalPosition = (Vector3)stream.ReceiveNext();
            networkLocalRotation = (Quaternion)stream.ReceiveNext();
            //networkLocalScale = (Vector3)stream.ReceiveNext();
        }
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            transform.localPosition = networkLocalPosition;
            transform.localRotation = networkLocalRotation;
            //transform.localScale = networkLocalScale;
        }

        if (PV.IsMine && isUser)
        {
            transform.position = mainCamera.transform.position;
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}
