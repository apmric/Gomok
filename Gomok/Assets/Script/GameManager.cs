using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum eKanInfo
{
    none,
    black,
    white
}

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
    [SerializeField]
    GameObject restartBt;

    public static GameManager instance;

    bool isBlack = false;
    bool isMyTurn = false;
    bool otherPlayerWantRestart = false;

    // 왼쪽 아래의 x 50, y 85
    const float START_X = 50 / 2f;
    const float START_Y = 85 / 2f;

    // 선과 선 사이 거리 65
    // 선과 선 사이 절반 거리 32.5
    // 맨처음 시작점 - 32.5 = 첫 번째 지점의 가장 왼쪽
    const float GAP = 65 / 2f;
    const float HALF_GAP = 65 / 4f;

    const int MAXINDEX = 15;

    eKanInfo[,] stoneArr = new eKanInfo[MAXINDEX, MAXINDEX];

    eKanInfo myStone;
    eKanInfo otherStone;

    int xIndex;
    int yIndex;

    PhotonView pv;

    // 내 돌 리스트
    List<GameObject> myStoneList = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        pv = GetComponent<PhotonView>();

        for (int i = 0; i < MAXINDEX; i++)
        {
            for (int j = 0; j < MAXINDEX; j++)
            {
                stoneArr[i, j] = eKanInfo.none;
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

            xIndex = (int) ((mousePos.x - HALF_GAP) / GAP);
            yIndex = (int) ((mousePos.y - HALF_GAP) / GAP);

            if (xIndex < 0 || xIndex > MAXINDEX - 1 || yIndex < 0 || yIndex > MAXINDEX - 1)
                return;

            if (stoneArr[yIndex, xIndex] != eKanInfo.none)
            {
                return;
            }

            isMyTurn = false;
            pv.RPC("NowMyTurn", RpcTarget.Others, xIndex, yIndex);

            stoneArr[yIndex, xIndex] = myStone;

            mousePos.x = (xIndex * GAP) + START_X;
            mousePos.y = (yIndex * GAP) + START_Y;

            if (isBlack)
                myStoneList.Add(PhotonNetwork.Instantiate("Black", mousePos, Quaternion.identity));
            else
                myStoneList.Add(PhotonNetwork.Instantiate("White", mousePos, Quaternion.identity));

            if(OnCheckWin(xIndex, yIndex))
            {
                pv.RPC("GameOver", RpcTarget.All, myStone + "Win\n");
            }
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
        myStone = eKanInfo.black;
    }

    /// <summary>
    /// 방에 입장했을 때
    /// </summary>
    public override void OnJoinedRoom()
    {
        text.text += "Room Joined!\n";

        isBlack = PhotonNetwork.IsMasterClient;

        if (isBlack)
        {
            myStone = eKanInfo.black;
            otherStone = eKanInfo.white;
        }
        else
        {
            myStone = eKanInfo.white;
            otherStone = eKanInfo.black;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        text.text = "Other Player Left The Game";

        restartBt.SetActive(false);
    }

    /// <summary>
    /// [PunRPC] 있으면 원격으로 함수 호출 기능
    /// </summary>
    [PunRPC]
    void NowMyTurn(int x, int y)
    {
        isMyTurn = true;
        stoneArr[y, x] = otherStone;
    }

    [PunRPC]
    void GameOver(string text)
    {
        isMyTurn = false;
        this.text.text = text;
        restartBt.SetActive(true);
    }

    [PunRPC]
    void ReStartRecv(string text)
    {
        this.text.text = "Other Player want Restart Game";
        otherPlayerWantRestart = true;
    }

    [PunRPC]
    public void ReStartPush()
    {
        // 상대방이 먼저 재시작 하자고 할 때
        if(otherPlayerWantRestart)
        {
            // 게임 재시작 하는 코드
            pv.RPC("ReStartGame", RpcTarget.All);
        }
        // 내가 먼저 재시작 하자고 할 때
        else
        {
            this.text.text = "ReStart Game\nWating other Player";
            pv.RPC("ReStartRecv", RpcTarget.Others);
        }

        restartBt.SetActive(false);
    }

    [PunRPC]
    void ReStartGame()
    {
        // 돌제거
        int count = myStoneList.Count;
        for (int i = 0; i < count; i++) 
        {
            PhotonNetwork.DestroyAll(myStoneList[0]);
            myStoneList.RemoveAt(0);
        }

        // 돌 배열 초기화
        stoneArr = new eKanInfo[15, 15];

        text.text = "";

        isMyTurn = isBlack;

        otherPlayerWantRestart = false;
    }

    struct FindAndX
    {
        public int find;
        public int x;
    }

    bool OnCheckWin(int x, int y)
    {
        int findCount = 0;

        int tempX;
        int tempY;

        // 가로 계산
        for(int i = -4; i < 1 ; i++)
        {
            findCount = 0;

            for(int j = 0; j < 5 ; j++)
            {
                tempX = x + i + j;

                if (tempX < 0 || tempX > 14)
                    break;

                if (stoneArr[y, tempX] == myStone)
                {
                    findCount++;
                    if(findCount == 5)
                        return true;
                }
            }
        }

        // 세로 계산
        for (int i = -4; i < 1; i++)
        {
            findCount = 0;

            for (int j = 0; j < 5; j++)
            {
                tempY = y + i + j;

                if (tempY < 0 || tempY > 14)
                    break;

                if (stoneArr[tempY, x] == myStone)
                {
                    findCount++;
                    if (findCount == 5)
                        return true;
                }
            }
        }

        // 오른쪽 위
        for (int i = -4; i < 1; i++)
        {
            findCount = 0;

            for (int j = 0; j < 5; j++)
            {
                tempX = x + i + j;
                tempY = y + i + j;

                if (tempX < 0 || tempX > 14 || tempY < 0 || tempY > 14)
                    break;

                if (stoneArr[tempY, tempX] == myStone)
                {
                    findCount++;
                    if (findCount == 5)
                        return true;
                }
            }
        }

        // 오른쪽 아래
        for (int i = -4; i < 1; i++)
        {
            findCount = 0;

            for (int j = 0; j < 5; j++)
            {
                tempX = x + i + j;
                tempY = y - i - j;

                if (tempX < 0 || tempX > 14 || tempY < 0 || tempY > 14)
                    break;

                if (stoneArr[tempY, tempX] == myStone)
                {
                    findCount++;
                    if (findCount == 5)
                        return true;
                }
            }
        }

        return false;
    }
}