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
        public GameObject atk;
        [Header("# PlayerInfo")]
        public float speed;

        PhotonView pv;

        GameObject[] myItem;

        float hAxis;
        float vAxis;
        bool fDown;
        bool qDown;

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
            Turn();
            Attack();
            Equipment();
        }

        void GetInput()
        {
            hAxis = Input.GetAxisRaw("Horizontal");
            vAxis = Input.GetAxisRaw("Vertical");
            fDown = Input.GetMouseButtonDown(0);
        }

        void Move()
        {
            moveVec = new Vector3(hAxis, 0, vAxis).normalized;

            this.transform.position += moveVec * speed * Time.deltaTime;
        }

        void Turn()
        {
            this.transform.LookAt(this.transform.position + moveVec);
        }

        void Attack()
        {
            if(fDown)
            {
                if (atk.activeSelf)
                    return;

                StartCoroutine(AttackCoru());
            }
        }

        [PunRPC]
        void AtkSetActive(bool b)
        {
            atk.SetActive(b);
        }

        IEnumerator AttackCoru()
        {
            atk.SetActive(true);
            pv.RPC("AtkSetActive", RpcTarget.Others, atk.activeSelf);

            yield return new WaitForSeconds(0.2f);

            atk.SetActive(false);
            pv.RPC("AtkSetActive", RpcTarget.Others, atk.activeSelf);
        }

        void EquipmentPrefabs()
        {
            myItem = new GameObject[itempPrefabs.Length];

            for (int i = 0; i < itempPrefabs.Length; i++)
            {
                myItem[i] = Instantiate(itempPrefabs[i], this.transform.position, Quaternion.identity, this.transform);

                myItem[i].SetActive(false);
            }
        }

        int myItemNow = 0;

        void Equipment()
        {
            KeyCode startKey = KeyCode.Alpha1;
            for(int i = 0; i < itempPrefabs.Length; i++)
            {
                if(Input.GetKeyDown(startKey + i))
                {
                    myItem[i].SetActive(!myItem[i].activeSelf);

                    myItemNow += myItem[i].activeSelf ? 1 << i : -(1 << i);

                    pv.RPC("ChangeItem", RpcTarget.Others, myItemNow);
                }
            }
        }

        [PunRPC]
        void ChangeItem(int n)
        {
            for(int i = 0; i < myItem.Length; i++)
            {
                myItem[i].SetActive((n >> i) % 2 == 1);
            }
        }
    }
}

// atk가 충돌시 rpc로 함수 부르기 hit