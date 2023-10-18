using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Test
{
    public class Player : MonoBehaviour
    {
        public float speed;

        PhotonView pv;

        float hAxis;
        float vAxis;

        Vector3 moveVec = Vector3.zero;

        private void Awake()
        {
            pv = GetComponent<PhotonView>();
        }

        void Update()
        {
            if (!pv.IsMine)
                return;

            GetInput();
            Move();
        }

        void GetInput()
        {
            hAxis = Input.GetAxisRaw("Horizontal");
            vAxis = Input.GetAxisRaw("Vertical");
        }

        void Move()
        {
            moveVec = new Vector3(hAxis, 0, vAxis).normalized;

            this.transform.position += moveVec * speed * Time.deltaTime;
        }
    }
}


