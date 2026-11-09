using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] float speedOfCamera;
    [SerializeField] float yOffset;
    [SerializeField] float yClamp;
    [SerializeField] Vector2 camLimit;

    [Header("Buildings")]
    [SerializeField] BackgroundManager[] backgroundAssets;


    public Vector2 mousePos { get; private set; }
    public Vector3 changedPos { get; private set; }


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
        float dis = 24f;
        for (int i = 0; i < 5; i++)
        {
            int ran = Random.Range(0, backgroundAssets.Length);
            BackgroundManager instance = Instantiate(backgroundAssets[ran]);
            Vector3 pos = Vector3.zero;
            pos.x = dis;
            dis -= Random.Range(2f,3f);
            pos.z = Random.Range(4f, 5f);
            instance.transform.localPosition = pos;
            instance.cam = this;
            instance.transform.parent = transform;
        }
        CreateBuilding();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        
        
        pos.x = Mathf.Lerp(transform.position.x, player.transform.position.x + 5, Time.deltaTime * speedOfCamera);
        pos.y = Mathf.Lerp(pos.y, player.transform.position.y + yOffset, speedOfCamera * Time.deltaTime);
        changedPos = transform.position - pos;
        transform.position = pos;
        cameraOffset = Vector2.Lerp(cameraOffset, cameraOffsetTarget, Time.deltaTime * speedOfCamera);
        changedPos += cameraObject.transform.localPosition - new Vector3(cameraOffset.x, cameraOffset.y, -10f);
        cameraObject.transform.localPosition = new Vector3(cameraOffset.x, cameraOffset.y, -10f);

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
    void CreateBuilding()
    {
        int ran = Random.Range(0, backgroundAssets.Length);
        BackgroundManager instance = Instantiate(backgroundAssets[ran]);
        instance.transform.position = transform.position;
        Vector3 pos = instance.transform.position;
        pos.x += 27;
        pos.z = Random.Range(4f, 5f);
        instance.transform.localPosition = pos;
        instance.cam = this;
        instance.transform.parent = transform;
        StartCoroutine(BackgroundDelay());
        


    }

    IEnumerator BackgroundDelay()
    {
        float posRn = transform.position.x;
        posRn += Random.Range(27f,48f);
        yield return new WaitUntil(() => posRn < transform.position.x);
        CreateBuilding(); 
    }

    
}
