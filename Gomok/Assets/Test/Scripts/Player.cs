using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Test
{
    public class Player : MonoBehaviour
    {
        [Header("# Equipment")]
        public GameObject hat1Prefab;
        public GameObject hat2Prefab;
        public GameObject weapon1Prefab;
        public GameObject weapon2Prefab;
        [Header("# PlayerInfo")]
        public float speed;

        PhotonView pv;

        GameObject hat1;
        GameObject hat2;
        GameObject weapon1;
        GameObject weapon2;

        float hAxis;
        float vAxis;
        bool e1Down;
        bool e2Down;
        bool e3Down;
        bool e4Down;

        bool isE1;
        bool isE2;
        bool isE3;
        bool isE4;

        Vector3 moveVec = Vector3.zero;

        private void Awake()
        {
            pv = GetComponent<PhotonView>();

            EquipmentPrefabs();
        }

        void Update()
        {
            if (!pv.IsMine)
                return;

            GetInput();
            Move();
            Equipment();
        }

        void GetInput()
        {
            hAxis = Input.GetAxisRaw("Horizontal");
            vAxis = Input.GetAxisRaw("Vertical");
            e1Down = Input.GetButtonDown("Equipment1");
            e2Down = Input.GetButtonDown("Equipment2");
            e3Down = Input.GetButtonDown("Equipment3");
            e4Down = Input.GetButtonDown("Equipment4");
        }

        void Move()
        {
            moveVec = new Vector3(hAxis, 0, vAxis).normalized;

            this.transform.position += moveVec * speed * Time.deltaTime;
        }

        void EquipmentPrefabs()
        {
            hat1 = Instantiate(hat1Prefab, this.transform.position + Vector3.up, this.transform.rotation);
            hat1.transform.parent = this.transform;
            hat2 = Instantiate(hat2Prefab, this.transform.position + Vector3.up, this.transform.rotation);
            hat2.transform.parent = this.transform;
            weapon1 = Instantiate(weapon1Prefab, this.transform.position + Vector3.right, this.transform.rotation);
            weapon1.transform.parent = this.transform;
            weapon2 = Instantiate(weapon2Prefab, this.transform.position + Vector3.right, this.transform.rotation);
            weapon2.transform.parent = this.transform;

            hat1.SetActive(false);
            hat2.SetActive(false);
            weapon1.SetActive(false);
            weapon2.SetActive(false);

            isE1 = false;
            isE2 = false;
            isE3 = false;
            isE4 = false;
        }

        void Equipment()
        {
            if (e1Down)
            {
                if (!isE1)
                {
                    hat1.SetActive(true);
                    isE1 = !isE1;
                }
                else
                {
                    hat1.SetActive(false);
                    isE1 = !isE1;
                }
            }

            if (e2Down)
            {
                if (!isE2)
                {
                    hat2.SetActive(true);
                    isE2 = !isE2;
                }
                else
                {
                    hat2.SetActive(false);
                    isE2 = !isE2;
                }
            }

            if (e3Down)
            {
                if (!isE3)
                {
                    weapon1.SetActive(true);
                    isE3 = !isE3;
                }
                else
                {
                    weapon1.SetActive(false);
                    isE3 = !isE3;
                }
            }

            if (e4Down)
            {
                if (!isE4)
                {
                    weapon2.SetActive(true);
                    isE4 = !isE4;
                }
                else
                {
                    weapon2.SetActive(false);
                    isE4 = !isE4;
                }
            }
        }
    }
}