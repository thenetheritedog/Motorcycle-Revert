using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public CameraManager cam;
    [SerializeField] float multFOV;

    Material mat;
    void Start()
    {
        transform.localPosition += (transform.position.z - 5) * Vector3.up;
        mat = GetComponent<SpriteRenderer>().material;
        mat.SetFloat("_Rand", Random.Range(0f,32f));
        mat.SetFloat("_Darken", 0.5f +transform.localPosition.y/2);
    }

    void Update()
    {
        transform.localPosition -= (cam.changedPos * Time.deltaTime) * (transform.localPosition.z - 6)/2 * multFOV;
        if (transform.localPosition.x < -15)
        {
            Destroy(gameObject);
        }
    }
}
