using UnityEngine;

public class Flying : MonoBehaviour
{
    [SerializeField]
    private float _velocity;

    // Bug rect transform
    private RectTransform _rt;
    // Bug initial position
    private Vector2 _initialPosition;
    // A new center of rotation is computer at the end of each _timer sec
    private float _timer;
    // Bug does not start moving until has been initialized
    private bool _initialized = false;
    // Center point of the rotation
    private Vector2 _center;
    // Ray (from center to bug position)
    private float _ray;
    // Angle of rotation
    private float _angle;
    // Direction of rotation
    private float _direction;

    // Debug object for checking the center position
    public RectTransform _centerGO;

    public void Initialize(Vector2 initialPosition)
    {
        _rt = GetComponent<RectTransform>();
        _rt.anchoredPosition = initialPosition;
        _centerGO = GameObject.Find("Spawner/centerRotationDebug").GetComponent<RectTransform>();
        GenerateTrajectory();
        _initialized = true;
    }

    private void GenerateTrajectory()
    {
        _timer = Random.Range(0.5f, 3f);
        _initialPosition = _rt.anchoredPosition;
        // center point is randomly generated (also outside the screen)
        _center = new Vector2(Random.Range(-50, Screen.width - 50), Random.Range(-50, Screen.height + 50));
        _centerGO.anchoredPosition = _center;
        _ray = Vector2.Distance(_center, _initialPosition);
        //print(_initialPosition + " " + _center + " " + _ray + " " + _adjacent + " " + _adjacent.magnitude);

        _adjacent = new Vector2(_initialPosition.x, _center.y);

        if (_initialPosition.y > _center.y)
        {
            _angle = Mathf.Acos((_adjacent - _center).x / _ray);
        }
        else
        {
            _angle = -Mathf.Acos((_adjacent - _center).x / _ray);
        }

        _direction = 1;

        // test new pos: se va fuori, cambia direzione
        float fakeAlpha = _angle + Time.deltaTime * _direction;
        float fakeX = _ray * Mathf.Cos(fakeAlpha);
        float fakeY = _ray * Mathf.Sin(fakeAlpha);
        Vector2 fakeNewPos = _center + new Vector2(fakeX, fakeY);
        if (fakeNewPos.x < 0 || fakeNewPos.x > Screen.width || fakeNewPos.y < 0 || fakeNewPos.y > Screen.height)
            _direction *= -1;
    }

    Vector2 _adjacent;
    private void ChangeTrajectory()
    {
        Vector2 newCenter = Vector2.zero;

        // mosquito on vertical border
        if (_rt.anchoredPosition.x <= 0 || _rt.anchoredPosition.x >= Screen.width) 
        {
            Debug.Log("On vertical border, reflecting center point horizontally");
            newCenter = new Vector2(_center.x, _rt.anchoredPosition.y - (_center.y - _rt.anchoredPosition.y));
            _center = newCenter;
        }
        // mosquito on horizontal border
        else if (_rt.anchoredPosition.y <= 0 || _rt.anchoredPosition.y >= Screen.height)
        {
            Debug.Log("On horizontal border, reflecting center point vertically");
            newCenter = new Vector2(_rt.anchoredPosition.x - (_center.x - _rt.anchoredPosition.x), _center.y);
            _center = newCenter;
        }

        _centerGO.anchoredPosition = _center;

        _initialPosition = _rt.anchoredPosition;
        _ray = Vector2.Distance(_initialPosition, _center);
        _adjacent = new Vector2(_initialPosition.x, _center.y);

        if (_initialPosition.y > _center.y)
        {
            _angle = Mathf.Acos((_adjacent - _center).x / _ray);
        }
        else
        {
            _angle = -Mathf.Acos((_adjacent - _center).x / _ray);
        }

        //print(_initialPosition + " " + _center + " " + _ray + " " + _adjacent + " " + _adjacent.magnitude);

        // test new pos: if it goes outside screen, change direction
        float fakeAlpha = _angle + Time.deltaTime * _direction;
        float fakeX = _ray * Mathf.Cos(fakeAlpha);
        float fakeY = _ray * Mathf.Sin(fakeAlpha);
        Vector2 fakeNewPos = _center + new Vector2(fakeX, fakeY);
        if (fakeNewPos.x < 0 || fakeNewPos.x > Screen.width || fakeNewPos.y < 0 || fakeNewPos.y > Screen.height)
            _direction *= -1;
    }


    private void Update()
    {
        if (_initialized)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                GenerateTrajectory();
            }

            _angle += Time.deltaTime * _direction /*/ _ray * _velocity*/;
            float x = _ray * Mathf.Cos(_angle);
            float y = _ray * Mathf.Sin(_angle);
            Vector2 newPos = _center + new Vector2(x, y);
            _rt.anchoredPosition = newPos;
            if (newPos.x <= 0 || newPos.x >= Screen.width || newPos.y <= 0 || newPos.y >= Screen.height)
            {
                print("Change");
                ChangeTrajectory();
            }
        }
    }

    public void Die(bool catched)
    {
        if (catched)
        {
            MosquitoGameManager.instance.FlyingCatched(gameObject.layer);
        }
        else
        {
            MosquitoGameManager.instance.FlyingMosquitoes--;
        }
        print("Dying " + (catched ? "" : "not") + " catched, there are still "
            + MosquitoGameManager.instance.FlyingMosquitoes + " mosquitoes");
        Destroy(gameObject);
    }
}
