using UnityEngine;

public class Board : MonoBehaviour
{
    public enum BoardSymbol
    {
        None = -1,
        O,
        X,
    }

    [SerializeField]
    private GameObject[] _symbolsPrefabs;
    private EmptyHole[] _gridElements;
    private BoardSymbol[,] _board;

    private bool _newMove = false;

    private void Awake()
    {
        Transform emptyHoles = transform.Find("EmptyHoles");
        _gridElements = new EmptyHole[emptyHoles.childCount];
        for (int i = 0; i < _gridElements.Length; i++)
        {
            _gridElements[i] = emptyHoles.GetChild(i).GetComponent<EmptyHole>();
            _gridElements[i].Initialize(i, this);
        }
        _board = new BoardSymbol[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                _board[i, j] = BoardSymbol.None;
            }
        }
    }

    public void ResetBoard()
    {
        for (int i = 0; i < _gridElements.Length; i++)
        {
            _gridElements[i].ResetHole();
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                _board[i, j] = BoardSymbol.None;
            }
        }
    }

    private void Update()
    {
        if (_newMove)
        {
            _newMove = false;
            //PrintBoard();
            BoardSymbol winnerOnDiag = CheckDiags();
            if (winnerOnDiag != BoardSymbol.None)
            {
                // BREAK! we have a winner!
                print("Winner!! " + winnerOnDiag);
                GameSetup.instance.NotifyWinner(winnerOnDiag);
                return;
            }
            for (int i = 0; i < 3; i++)
            {
                BoardSymbol winnerOnRow = CheckRow(i);
                if (winnerOnRow != BoardSymbol.None)
                {
                    print("Winner!! " + winnerOnRow);
                    GameSetup.instance.NotifyWinner(winnerOnRow);
                    return;
                }
                BoardSymbol winnerOnCol = CheckColumn(i);
                if (winnerOnCol != BoardSymbol.None)
                {
                    print("Winner!! " + winnerOnCol);
                    GameSetup.instance.NotifyWinner(winnerOnCol);
                    return;
                }
            }
            if (CheckAllFilled())
            {
                GameSetup.instance.NotifyEndOfGame();
            }
        }
    }

    private bool CheckAllFilled()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (_board[i, j] == BoardSymbol.None)
                    return false;
            }
        }
        return true;
    }

    private BoardSymbol CheckRow(int row)
    {
        BoardSymbol r0 = _board[row, 0];
        BoardSymbol r1 = _board[row, 1];
        BoardSymbol r2 = _board[row, 2];
        if (r0 == BoardSymbol.None || r1 == BoardSymbol.None || r2 == BoardSymbol.None)
            return BoardSymbol.None;
        if (r0 == r1 && r1 == r2)
        {
            return r0;
        }
        else
        {
            return BoardSymbol.None;
        }
    }

    private BoardSymbol CheckColumn(int col)
    {
        BoardSymbol r0 = _board[0, col];
        BoardSymbol r1 = _board[1, col];
        BoardSymbol r2 = _board[2, col];
        if (r0 == BoardSymbol.None || r1 == BoardSymbol.None || r2 == BoardSymbol.None)
            return BoardSymbol.None;
        if (r0 == r1 && r1 == r2)
        {
            return r0;
        }
        else
        {
            return BoardSymbol.None;
        }
    }

    private BoardSymbol CheckDiags()
    {
        BoardSymbol b00 = _board[0, 0];
        BoardSymbol b11 = _board[1, 1];
        BoardSymbol b22 = _board[2, 2];
        BoardSymbol b02 = _board[0, 2];
        BoardSymbol b20 = _board[2, 0];
        if (b00 == BoardSymbol.None || b11 == BoardSymbol.None || b22 == BoardSymbol.None)
            return BoardSymbol.None;
        if (b00 == b11 && b11 == b22)
        {
            return b00;
        }
        if (b02 == BoardSymbol.None || b11 == BoardSymbol.None || b20 == BoardSymbol.None)
            return BoardSymbol.None;
        if (b02 == b11 && b11 == b20)
        {
            return b02;
        }
        else
            return BoardSymbol.None;
    }

    private void FillBoardStructure(int layerId, BoardSymbol s)
    {
        // fill board structure with element
        int row = layerId / 3;
        int col = layerId % 3;
        _board[row, col] = s;
    }

    private void PrintBoard()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                print("row " + i + ", col " + j + " symbol: " + _board[i, j]);
            }
        }
    }

    // layerId: [0, ..., 8]
    // Questo metodo lo chiama solo il localplayer (è un RPC) che istanzia via rete l'oggetto O o X 
    public bool FillWithSymbol(BoardSymbol s, int holeId)
    {
        if (_gridElements[holeId].IsEmpty)
        {
            string s_name = s.ToString();
            _gridElements[holeId].SetSymbol(s_name);
            _newMove = true;
            FillBoardStructure(holeId, s);
            return true;
        }
        else
        {
            return false;
        }
    }

}
