using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public float speedUsing { get; private set;}
    public float distance { get; private set; }

    public LayerMask breakMask;

    

    public CameraManager cameraManager { get; private set; }

    [SerializeField] float baseSpeed = 5f;
    [SerializeField] float speedIncreasing;
    [SerializeField] GameObject breaking;
    ParticleSystem breakingParticle;

    private PlayerMovement movement;

    void Start()
    {
        speedUsing = baseSpeed;
        cameraManager = FindAnyObjectByType<CameraManager>();
    }

    // Update is called once per frame
    void Update()
    {
        speedUsing += speedIncreasing * Time.deltaTime;
        if (breaking != null)
        {
           var mainPart = breakingParticle.main;
           mainPart.simulationSpeed = 1/(1+breakingParticle.time);
           breakingParticle.time = Mathf.Clamp(breakingParticle.time, 0, 1);
        }
        else if (breakingParticle != null) 
        {
            var mainPart = breakingParticle.main;
            mainPart.simulationSpeed = -1 / (1  -breakingParticle.time);
            if (breakingParticle.time < 0.1f)
            {
                breakingParticle.Stop();
                breakingParticle.GetComponent<SpriteRenderer>().enabled = true;
                breakingParticle = null;
            }
            
        }
    }

    public void BreakObject(bool clicked)
    {
        Vector3 mousePos = cameraManager.mousePos;
        
        mousePos.z = 0;
        Collider2D hit2D = Physics2D.OverlapCircle(mousePos, 1f, breakMask, -10,10);
        Debug.DrawRay(mousePos, Camera.main.transform.position - mousePos);
        if (clicked && hit2D != null)
        {
            breaking = hit2D.gameObject;
            breaking.GetComponent<SpriteRenderer>().enabled = false;
            breakingParticle = breaking.GetComponent<ParticleSystem>();
            breakingParticle.Play();
        }
        else
        {
            breaking = null;
        }
    }
}
