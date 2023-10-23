using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using Test;
using System.ComponentModel;

public class Atk : MonoBehaviour
{
    PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    void OnEnable()
    {
        Debug.Log("È°¼ºÈ­");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!pv.IsMine && other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            if (other != null)
            {
                other.GetComponent<PhotonView>().RPC("Hit", RpcTarget.All);
            }
        }
    }
}
