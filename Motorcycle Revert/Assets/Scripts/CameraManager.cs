using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] float speedOfCamera;
    [SerializeField] float yOffset;
    [SerializeField] float yClamp;
    [SerializeField] Vector2 camLimit;
    public Vector2 mousePos { get; private set; }


    float baseSpeed;

    Vector2 cameraOffset;
    Vector2 cameraOffsetTarget;
    
    PlayerManager player;
    GameObject cameraObject;
    Camera cam;
    void Start()
    {
        player = FindAnyObjectByType<PlayerManager>();
        cam = Camera.main;
        cameraObject = transform.GetChild(0).gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        
        
        pos.x = Mathf.Lerp(transform.position.x, player.transform.position.x + 5, Time.deltaTime * speedOfCamera);
        pos.y = Mathf.Lerp(pos.y, player.transform.position.y + yOffset, speedOfCamera * Time.deltaTime);
        transform.position = pos;
        cameraOffset = Vector2.Lerp(cameraOffset, cameraOffsetTarget, Time.deltaTime * speedOfCamera);
        cameraObject.transform.position = transform.position;
        cameraObject.transform.position += new Vector3(cameraOffset.x, cameraOffset.y, -10f);
    }

    public void ChangeCameraOffset(Vector2 input)
    {
        mousePos = cam.ScreenToWorldPoint(input);


        input.x = input.x / Screen.width;
        input.y = input.y / Screen.height;
        cameraOffsetTarget.x = Mathf.Lerp(-1,1,input.x);
        cameraOffsetTarget.y = Mathf.Lerp(-1, 1, input.y);

        cameraOffsetTarget.x = Mathf.Sqrt(Mathf.Abs(cameraOffsetTarget.x)) 
            * Mathf.Sign(cameraOffsetTarget.x);
        cameraOffsetTarget.y = Mathf.Sqrt(Mathf.Abs(cameraOffsetTarget.y)) 
            * Mathf.Sign(cameraOffsetTarget.y);

        cameraOffsetTarget.x *= camLimit.x;
        cameraOffsetTarget.y *= camLimit.y;


        
    }

    
}
