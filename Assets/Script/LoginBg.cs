using UnityEngine;

public class LoginBg : MonoBehaviour
{
    Camera camera;
    float H, S, V;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        Color.RGBToHSV(camera.backgroundColor, out H, out S, out V);
    }

    // Update is called once per frame
    void Update()
    {
        H = (H + 1) % 360;
        float H_norm = H / 360f;
        camera.backgroundColor = Color.HSVToRGB(H_norm, S, V);
    }

    
}
