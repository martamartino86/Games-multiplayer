using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameSetup : MonoBehaviourPunCallbacks, IInRoomCallbacks, IOnEventCallback
{
    public const int NEWPLAYERLOGIN_EVTCODE = 0;
    public const int UPDATELASTMOVE_EVTCODE = 1;
    public const int LASTMOVENOTCOMPLETED   = 2;

    public static GameSetup instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameSetup>();
            return _instance;
        }
    }
    private static GameSetup _instance;

    public Transform[] SpawnPoints;
    private Dictionary<Player, Board.BoardSymbol> _playerSymbol;
    private Dictionary<Board.BoardSymbol, Player> _symbolPlayer;
    private Board.BoardSymbol _nextBoardSymbol = Board.BoardSymbol.O;
    private int _lastMoveActorNumber;
    private bool _gameEnded;

    public Board Board
    {
        get
        {
            if (_board == null)
                _board = GetComponent<Board>();
            return _board;
        }
    }
    private Board _board;
    private bool _twoPlayersInRoom;
    private int[] _layers;


    private void Awake()
    {
        _playerSymbol = new Dictionary<Player, Board.BoardSymbol>();
        _symbolPlayer = new Dictionary<Board.BoardSymbol, Player>();
        _symbolPlayer[Board.BoardSymbol.O] = null;
        _symbolPlayer[Board.BoardSymbol.X] = null;
        _layers = new int[9];
        for (int i = 0; i < 9; i++)
        {
            string l = "EO_" + i;
            _layers[i] = LayerMask.NameToLayer(l);
        }
        // siccome questo gameobject è creato quando si avvia la scena multiplayer,
        // ci sta che il primo player sia già loggato e quindi io non riceverò l'evento del new player.
        if (photonView.IsMine)
        {
            UpdateSymbol(PhotonNetwork.LocalPlayer);
        }
        _twoPlayersInRoom = PhotonNetwork.PlayerList.Length >= 2;
        _gameEnded = false;
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    private void Update()
    {
        // handle mouse events
        if (!_gameEnded && Input.GetMouseButtonUp(0) && EventSystem.current.currentSelectedGameObject != null)
        {
            if (!_twoPlayersInRoom)
            {
                PhotonStatusCanvas.instance.AppendMessage("Not enough players to start playing!");
                return;
            }
            int selectedObjLayer = EventSystem.current.currentSelectedGameObject.layer;
            if (_layers[0] <= selectedObjLayer && selectedObjLayer <= _layers[_layers.Length - 1])
            {
                int playerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                if (_lastMoveActorNumber != -1 && playerActorNumber == _lastMoveActorNumber)
                {
                    PhotonStatusCanvas.instance.AppendMessage("You can't play, it's your opponent turn!");
                }
                else
                {
                    _lastMoveActorNumber = playerActorNumber;
                    int emptyHoleId = selectedObjLayer % (_layers.Length + 1);
                    photonView.RPC("RPC_FillHole", RpcTarget.All, PhotonNetwork.LocalPlayer, emptyHoleId);
                }
            }
        }
    }

    public bool HoleClicked(int holeId, Player player)
    {
        print("Hole with layer " + holeId + " clicked ");

        //if (_playerSymbol[player] != Board.BoardSymbol.None)
        //{
        Board.BoardSymbol s = _playerSymbol[player];
        return Board.FillWithSymbol(s, holeId);
        //}
    }

    //public void HoleClicked(int holeId, Player player)
    //{
    //    //if (!_twoPlayersInRoom)
    //    //{
    //    //    PhotonStatusCanvas.instance.AppendMessage("Not enough players to start playing!");
    //    //}
    //    //else
    //    {
    //        //int playerActorNumber = player.ActorNumber;
    //        //if (_lastMoveActorNumber != -1 && playerActorNumber == _lastMoveActorNumber)
    //        //{
    //        //    PhotonStatusCanvas.instance.AppendMessage("You can't play, it's your opponent turn!");
    //        //    return;
    //        //}
    //        print("Hole with layer " + holeId + " clicked ");
    //        // a seconda di chi ha cliccato, devo settare un certo prefab nell'EmptyHole
    //        if (_playerSymbol[player] != Board.BoardSymbol.None)
    //        {
    //            Board.BoardSymbol s = _playerSymbol[player];
    //            Board.FillWithSymbol(s, holeId);
    //        }
    //        //_lastMoveActorNumber = playerActorNumber;
    //        //photonView.RPC("RPC_UpdateLastMoveActorNumber", RpcTarget.AllBuffered, _lastMoveActorNumber);
    //    }
    //}

    public void NotifyWinner(Board.BoardSymbol symbol)
    {
        // the winner is the last one that made a move
        // but check (just in case)
        if (symbol == _playerSymbol[PhotonNetwork.CurrentRoom.GetPlayer(_lastMoveActorNumber)])
        {
            photonView.RPC("RPC_AndTheWinnerIs", RpcTarget.All, _symbolPlayer[symbol]);
        }
    }

    public void NotifyEndOfGame()
    {
        photonView.RPC("RPC_AndTheWinnerIs", RpcTarget.All, null);
    }

    public void RestartGame()
    {
        photonView.RPC("RPC_RestartGame", RpcTarget.All, null);
    }

    private void SendMessageToPlayer(Player player, string msg)
    {
        object[] content = new object[] { "GameSetup", msg };
        print("Msg sent: "+PhotonNetwork.RaiseEvent(NEWPLAYERLOGIN_EVTCODE, content, new RaiseEventOptions { TargetActors = new int[] { player.ActorNumber } }, SendOptions.SendReliable));
    }

    [PunRPC]
    private void RPC_FillHole(Player playerWhoClicked, int holeId)
    {
        // è solo il "main" GameSetup a dover istanziare le cose nella Board
        if (photonView.IsMine)
        {
            if (HoleClicked(holeId, playerWhoClicked))
            {
                photonView.RPC("LastMoveCompletedCorrectly", RpcTarget.All, playerWhoClicked.ActorNumber);
            }
            else
            {
                SendMessageToPlayer(playerWhoClicked, "You must click on an empty cell!");
            }
        }
    }

    [PunRPC]
    private void LastMoveCompletedCorrectly(int actorNumber)
    {
        _lastMoveActorNumber = actorNumber;
    }

    private void UpdatePlayerSymbol(Player player, Board.BoardSymbol symbol, bool add)
    {
        if (add)
        {
            _playerSymbol.Add(player, symbol);
            _symbolPlayer[symbol] = player;
            PhotonStatusCanvas.instance.AppendMessage("Adding " + player.ActorNumber + " with symbol " + symbol.ToString());
        }
        else
        {
            _playerSymbol.Remove(player);
            _symbolPlayer[symbol] = null;
            PhotonStatusCanvas.instance.AppendMessage("Removing " + player.ActorNumber + " with symbol " + symbol.ToString());
        }
        _twoPlayersInRoom = PhotonNetwork.PlayerList.Length >= 2;
    }

    [PunRPC]
    private void RPC_AndTheWinnerIs(Player winnerPlayer)
    {
        _gameEnded = true;
        if (winnerPlayer != null)
        {
            PhotonStatusCanvas.instance.AppendMessage("<b>" + winnerPlayer.NickName + " wins the match!</b>");
        }
        else
        {
            PhotonStatusCanvas.instance.AppendMessage("<b>Game ended without a winner!</b>");
        }
    }

    [PunRPC]
    private void RPC_RestartGame()
    {
        _gameEnded = false;
        if (photonView.IsMine)
        {
            Board.ResetBoard();
            _lastMoveActorNumber = -1;
        }
    }

    private void SetNextBoardSymbol()
    {
        if (_symbolPlayer[Board.BoardSymbol.O] == null)
        {
            _nextBoardSymbol = Board.BoardSymbol.O;
        }
        else
        {
            _nextBoardSymbol = Board.BoardSymbol.X;
        }
    }

    private void UpdateSymbol(Player newPlayer)
    {
        if (PhotonNetwork.PlayerList.Length > 2)
        {
            string msg = "Game already has 2 players. You can watch, but not play!";
            SendMessageToPlayer(newPlayer, msg);
            return;
        }
        else
        {
            SetNextBoardSymbol();
            UpdatePlayerSymbol(newPlayer, _nextBoardSymbol, true);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PhotonStatusCanvas.instance.AppendMessage("PLAYER ENTER ROOM " + newPlayer.ActorNumber);
        if (photonView.IsMine)
        {
            UpdateSymbol(newPlayer);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonStatusCanvas.instance.AppendMessage("A player (ActorNumber " + otherPlayer.ActorNumber + ") has left.");
        if (photonView.IsMine)
        {
            UpdatePlayerSymbol(otherPlayer, _playerSymbol[otherPlayer], false);
        }
    }

    // Quando loggo ricevo un messaggio dal gioco
    public void OnEvent(EventData photonEvent)
    {
        object[] data = null;
        try
        {
            data = (object[])photonEvent.CustomData;
        }
        catch (Exception)
        {
            Debug.Log("No Custom Data for this type of event: " + photonEvent.Code);
        }
        switch (photonEvent.Code)
        {
            case NEWPLAYERLOGIN_EVTCODE:
            case LASTMOVENOTCOMPLETED:
                string sender = (string)data[0];
                string msg = (string)data[1];
                PhotonStatusCanvas.instance.AppendMessage("[" + sender + "] -> " + msg);
                break;
            case UPDATELASTMOVE_EVTCODE:
                int lastMoveActorNumber = (int)data[1];
                _lastMoveActorNumber = lastMoveActorNumber;
                break;
            default:
                break;
        }
    }

}
