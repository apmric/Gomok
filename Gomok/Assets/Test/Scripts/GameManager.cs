using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;


namespace Test
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager instance;

        PhotonView pv;

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
            pv = GetComponent<PhotonView>();
        }

        void Start()
        {
            Screen.SetResolution(1920, 1080, false);

            // 서버 접속하기
            PhotonNetwork.ConnectUsingSettings();
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 서버에 연결시
        /// </summary>
        public override void OnConnectedToMaster()
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 3;
            PhotonNetwork.JoinOrCreateRoom("Room1", options, null);
        }

        /// <summary>
        /// 방 만들었을때 실행
        /// </summary>
        public override void OnCreatedRoom()
        {

        }

        /// <summary>
        /// 방에 입장했을 때
        /// </summary>
        public override void OnJoinedRoom()
        {
            Vector3 pos = new Vector3 (Random.Range(-7f, 7f), 1, Random.Range(-7f, 7f));

            PhotonNetwork.Instantiate("Player", pos, Quaternion.identity);
        }
    }
}
