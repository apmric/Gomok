using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("# Transform")]
    public Transform canvous;
    [SerializeField]
    Transform black;
    [SerializeField]
    Transform white;
    [Header("# TextMeshProUGUI")]
    [SerializeField]
    TextMeshProUGUI text;

    public static GameManager instance;

    bool isBlack = false;

    bool isMyTurn = false;

    // 왼쪽 아래의 x 50, y 85
    const float START_X = 50 / 2f;
    const float START_Y = 85 / 2f;

    // 선과 선 사이 거리 65
    // 선과 선 사이 절반 거리 32.5
    // 맨처음 시작점 - 32.5 = 첫 번째 지점의 가장 왼쪽
    const float GAP = 65 / 2f;
    const float HALF_GAP = 65 / 4f;

    const int MAXINDEX = 15;

    bool[,] stoneArr = new bool[MAXINDEX, MAXINDEX];

    int xIndex;
    int yIndex;

    PhotonView pv;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        pv = GetComponent<PhotonView>();

        for (int i = 0; i < MAXINDEX; i++)
        {
            for (int j = 0; j < MAXINDEX; j++)
            {
                stoneArr[i, j] = false;
            }
        }
    }

    void Start()
    {
        Screen.SetResolution(960, 540, false);

        // 서버 접속하기
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMyTurn)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            text.text += mousePos.ToString() + "\n";

            xIndex = (int) ((mousePos.x - HALF_GAP) / GAP);
            yIndex = (int) ((mousePos.y - HALF_GAP) / GAP);

            if (xIndex < 0 || xIndex > MAXINDEX - 1 || yIndex < 0 || yIndex > MAXINDEX - 1)
                return;

            if (stoneArr[yIndex, xIndex] == true)
            {
                Debug.Log("이미 자리가 있음");
                return;
            }

            isMyTurn = false;
            pv.RPC("NowMyTurn", RpcTarget.Others, 123);

            stoneArr[yIndex, xIndex] = true;
            mousePos.x = (xIndex * GAP) + START_X;
            mousePos.y = (yIndex * GAP) + START_Y;

            Transform temp;

            if (isBlack)
                temp = PhotonNetwork.Instantiate("Black", mousePos, Quaternion.identity).transform;
            else
                temp = PhotonNetwork.Instantiate("White", mousePos, Quaternion.identity).transform;
        }
    }

    /// <summary>
    /// 서버에 연결시
    /// </summary>
    public override void OnConnectedToMaster()
    {
        text.text += "Server Connecting..\n";
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom("Room1", options, null);
    }

    /// <summary>
    /// 방 만들었을때 실행
    /// </summary>
    public override void OnCreatedRoom()
    {
        text.text += "Create Room\n";
        isMyTurn = true;
    }

    /// <summary>
    /// 방에 입장했을 때
    /// </summary>
    public override void OnJoinedRoom()
    {
        text.text += "Room Joined!\n";

        isBlack = PhotonNetwork.IsMasterClient;
    }

    /// <summary>
    /// [PunRPC] 있으면 원격으로 함수 호출 기능
    /// </summary>
    [PunRPC]
    void NowMyTurn(int x)
    {
        isMyTurn = true;

        Debug.Log(x);
    }
}
