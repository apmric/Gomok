using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using Test;

public class Atk : MonoBehaviour
{
    PhotonView pv;

    void Awake()
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
            pv.RPC("Damage", RpcTarget.All, 10f);
        }
    }

    [PunRPC]
    void Damage(float damage)
    {
        Debug.Log("Damage: " + damage);
    }
}
