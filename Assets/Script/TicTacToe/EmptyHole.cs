using Photon.Pun;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class EmptyHole : MonoBehaviour
{
    private int _id;
    private Board _board;
    private GameObject _mySymbol;
    private PhotonView _mySymbolPV;
    public bool IsEmpty
    {
        get; private set;
    }

    public void Initialize(int id, Board board)
    {
        _id = id;
        _board = board;
        IsEmpty = true;
    }

    public void ResetHole()
    {
        // NB: _mySymbol could be null because the symbol has not been
        // instantiated by this player! So the gameobject exists,
        // but the _mySymbol variable has never been assigned.
        // So that means automatically that the symbol is not mine
        // and I can't cancel it.
        if (transform.childCount > 0 && _mySymbol != null)
        {
            if (_mySymbolPV.IsMine)
                PhotonNetwork.Destroy(_mySymbol);
        }
        IsEmpty = true;
    }

    public void SetSymbol(string symbolName)
    {
        PhotonStatusCanvas.instance.AppendMessage("istanzio " +symbolName + " in " + transform.TransformPoint(Vector3.zero));
        object[] data = new object[1];
        data[0] = transform.name;
        _mySymbol = PhotonNetwork.Instantiate(
            Path.Combine("TicTacToe", symbolName.ToUpper()),
            transform.TransformPoint(Vector3.zero), Quaternion.identity, 0, data);
        _mySymbol.transform.SetParent(transform, true);
        _mySymbolPV = _mySymbol.GetComponent<PhotonView>();
        IsEmpty = false;
    }

    private void OnMouseUp()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
