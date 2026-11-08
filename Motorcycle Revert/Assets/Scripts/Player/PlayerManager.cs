using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public float speedUsing { get; private set; }
    public float distance { get; private set; }

    public LayerMask breakMask;



    public CameraManager cameraManager { get; private set; }

    [Header("Values")]
    [SerializeField] float baseSpeed = 5f;
    [SerializeField] float speedIncreasing;

    [Header("Level")] 
    [SerializeField] GameObject[] loadedSections;
    [SerializeField] GameObject[] sectionAssets;
    [SerializeField] GameObject deathFog;


    private PlayerMovement movement;


    GameObject breaking;
    SpriteRenderer breakSprite;
    Collider2D breakCollider;
    ParticleSystem breakingParticle;
    ParticleSystem.Particle[] arrayBreakingParticle;

    void Start()
    {
        Application.targetFrameRate = 60;
        speedUsing = baseSpeed;
        cameraManager = FindAnyObjectByType<CameraManager>();
        int ran = UnityEngine.Random.Range(0, sectionAssets.Length);
        loadedSections[1] = Instantiate(sectionAssets[ran]);
        ran = UnityEngine.Random.Range(0, sectionAssets.Length);
        loadedSections[2] = Instantiate(sectionAssets[ran]);
        Vector3 newPosOfSection = loadedSections[1].transform.GetChild(0).transform.position;
        loadedSections[2].transform.position = newPosOfSection;
    }

    // Update is called once per frame
    void Update()
    {
        speedUsing += speedIncreasing * Time.deltaTime;
        BreakParticleHandling();
        distance = Mathf.Max(distance, Mathf.Round(transform.position.x/4));
        deathFog.transform.position += Vector3.right * Time.deltaTime * speedUsing;
        deathFog.transform.GetChild(0).transform.localPosition = transform.position.y * Vector3.up;

            
        
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
            breakingParticle = breaking.GetComponent<ParticleSystem>();
            breakSprite = breakingParticle.GetComponent<SpriteRenderer>();
            breakCollider = breaking.GetComponent<Collider2D>();
            if (breakSprite.enabled == true)
            {
                breakingParticle.randomSeed = (uint)UnityEngine.Random.Range(0, 1024);
                breakingParticle.Emit(20);
            }
            breakSprite.enabled = false;
            breakCollider.enabled = false;
            
        }
        else
        {
            breaking = null;
        }
    }

    private void BreakParticleHandling()
    {
       if (breaking != null)
       {
            var mainPart = breakingParticle.main;
            if (arrayBreakingParticle == null)
            {
                mainPart.simulationSpeed = 1;
                
                arrayBreakingParticle = new ParticleSystem.Particle[breakingParticle.main.maxParticles];
            }


            breakingParticle.GetParticles(arrayBreakingParticle);

            if (arrayBreakingParticle[0].remainingLifetime != 0)
                mainPart.simulationSpeed = Mathf.Sqrt((arrayBreakingParticle[0].remainingLifetime - 0.225f) / 2);
            

       }
       else if (breakingParticle != null)
       {
            var mainPart = breakingParticle.main;
            var velocityOverLifeTime = breakingParticle.velocityOverLifetime;
            mainPart.simulationSpeed = 1;
            breakingParticle.GetParticles(arrayBreakingParticle);
            float simTime = arrayBreakingParticle[0].remainingLifetime;
            velocityOverLifeTime.radial = -40; 
            if (simTime < 0.025f)
            {
                breakingParticle.Stop();
                breakingParticle.Clear();
                breakSprite.enabled = true;
                breakCollider.enabled = true;
                breakingParticle = null;
                arrayBreakingParticle = null;
                breakSprite = null;
            }
            
        }
    }

    private void LoadNewSection()
    {
        if (loadedSections[0] != null) 
        {
            Destroy(loadedSections[0]);
        }
        for (int i = 0; i < loadedSections.Length -1; i++)
        {
            loadedSections[i] = loadedSections[i + 1];
        }
        int ran = UnityEngine.Random.Range(0,sectionAssets.Length);
        loadedSections[2] = Instantiate(sectionAssets[ran]);
        Vector3 newPosOfSection = loadedSections[1].transform.GetChild(0).transform.position;
        loadedSections[2].transform.position = newPosOfSection;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Section"))
        {
            LoadNewSection();
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Death"))
        {
            SceneManager.LoadScene("Title Screen");
        }
    }
}
