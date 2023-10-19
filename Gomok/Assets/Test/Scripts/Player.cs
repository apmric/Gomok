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
        public GameObject[] itempPrefabs;
        [Header("# PlayerInfo")]
        public float speed;

        PhotonView pv;

        GameObject[] item;

        float hAxis;
        float vAxis;

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
        }

        void Move()
        {
            moveVec = new Vector3(hAxis, 0, vAxis).normalized;

            this.transform.position += moveVec * speed * Time.deltaTime;
        }

        void EquipmentPrefabs()
        {
            item = new GameObject[itempPrefabs.Length];

            for (int i = 0; i < itempPrefabs.Length; i++)
            {
                item[i] = Instantiate(itempPrefabs[i], this.transform.position, this.transform.rotation);
                item[i].transform.parent = this.transform;

                item[i].SetActive(false);
            }
        }

        public int myItemNow = 0;

        void Equipment()
        {
            KeyCode startKey = KeyCode.Alpha1;
            for(int i = 0; i < itempPrefabs.Length; i++)
            {
                if(Input.GetKeyDown(startKey + i))
                {
                    item[i].SetActive(item[i].activeSelf);

                    myItemNow += item[i].activeSelf ? 1 << i : -(1 << i);
                }
            }
        }

        void ChangeItem(int n)
        {

        }
    }
}