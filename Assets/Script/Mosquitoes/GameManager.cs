using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MosquitoGameManager : MonoBehaviour
{
    public static MosquitoGameManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<MosquitoGameManager>();
            return _instance;
        }
    }
    private static MosquitoGameManager _instance;

    // How many mosquitoes do I have to kill?
    [SerializeField]
    private int Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            _scoreUI.text = _score.ToString();
            if (_score == 0)
            {
                _spawner.StopSpawn();
                WinScreen.SetActive(true);
            }
        }
    }
    private int _score;

    private int Hands
    {
        get
        {
            return _hands;
        }
        set
        {
            for (int i = 0; i < (value - _hands); i++)
            {
                Instantiate(_handIconPrefab, _handIconsContainer.transform);
            }
            _hands = value;
        }
    }
    private int _hands;

    [SerializeField]
    private int _scoreOnStart;
    [SerializeField]
    private int _handsOnStart;

    [SerializeField]
    private GameObject _handIconPrefab;

    [SerializeField]
    private GameObject _handIconsContainer;

    [SerializeField]
    private TextMeshProUGUI _scoreUI;

    [SerializeField]
    private GameObject WinScreen;

    // How many mosquitoes and hands are flying in the screen?
    public int FlyingMosquitoes;
    public int FlyingHands;

    [SerializeField]
    private Spawner _spawner;

    private Dictionary<string, int> _layers;

    // Start is called before the first frame update
    void Start()
    {
        Score = _scoreOnStart;
        Hands = _handsOnStart;

        _layers = new Dictionary<string, int>();
        _layers.Add("Mosquito", LayerMask.NameToLayer("Mosquito"));
        _layers.Add("Hand", LayerMask.NameToLayer("Hand"));

        _spawner.StartSpawn();
    }

    public void FlyingCatched(int flyingCatchedLayer)
    {
        if (flyingCatchedLayer == _layers["Mosquito"])
        {
            Score = Score - 1;
            FlyingMosquitoes--;
        }
        if (flyingCatchedLayer == _layers["Hand"])
        {
            Hands = Hands + 1;
            FlyingHands--;
        }
    }
}
