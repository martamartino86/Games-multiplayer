using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private Flying _mosquitoPrefab;
    [SerializeField]
    private Flying _handPrefab;
    [SerializeField]
    private Transform _flyersContainer;
    // Can spawn?
    private bool _spawn;
    // After this _timer, try to spawn a hand
    [SerializeField]
    [Range(.5f, 10f)]
    private float _timerHand;
    private float _initTimerHand;

    private void Start()
    {
        _initTimerHand = _timerHand;
    }

    public void StartSpawn()
    {
        _spawn = true;
    }

    public void StopSpawn()
    {
        _spawn = false;
        foreach (Transform flyer in _flyersContainer)
        {
            Destroy(flyer.gameObject);
        }
    }

    private Vector2 SetInitialPosition()
    {
        float r = Random.value;

        if (r >= 0 && r <= .25f)
        {
            float x = Random.Range(0, Screen.width);
            return new Vector2(x, 0f);
        }
        if (r > .25f && r <= .5f)
        {
            float x = Random.Range(0, Screen.width);
            return new Vector2(x, Screen.height);
        }
        if (r > .5f && r <= .75f)
        {
            float y = Random.Range(0, Screen.height);
            return new Vector2(0f, y);
        }
        else
        {
            float y = Random.Range(0, Screen.height);
            return new Vector2(Screen.width, y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_spawn)
        {
            float r = Random.value;
            if (MosquitoGameManager.instance.FlyingMosquitoes < 2 && r <= .5f)
            {
                Flying mosquito = Instantiate(_mosquitoPrefab, _flyersContainer) as Flying;
                Vector2 initialPosition = SetInitialPosition();
            
                mosquito.Initialize(initialPosition);

                MosquitoGameManager.instance.FlyingMosquitoes++;
            }

            if (_timerHand <= 0)
            {
                if (MosquitoGameManager.instance.FlyingHands < 1 && r <= .01f)
                {
                    Flying hand = Instantiate(_handPrefab, _flyersContainer) as Flying;
                    Vector2 initialPosition = SetInitialPosition();

                    hand.Initialize(initialPosition);

                    MosquitoGameManager.instance.FlyingHands++;

                    _timerHand = _initTimerHand;
                }
            }
        }
        _timerHand -= Time.deltaTime;
    }
}
