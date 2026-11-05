using System;
using Unity.Mathematics;
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


    private PlayerMovement movement;


    GameObject breaking;
    SpriteRenderer breakSprite;
    Collider2D breakCollider;
    ParticleSystem breakingParticle;
    ParticleSystem.Particle[] arrayBreakingParticle;

    void Start()
    {
        speedUsing = baseSpeed;
        cameraManager = FindAnyObjectByType<CameraManager>();
    }

    // Update is called once per frame
    void Update()
    {
        speedUsing += speedIncreasing * Time.deltaTime;
        BreakParticleHandling();
        
            
        
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
                mainPart.simulationSpeed = Mathf.Sqrt(arrayBreakingParticle[0].remainingLifetime - 0.025f / 2);
            

       }
       else if (breakingParticle != null)
       {
            var mainPart = breakingParticle.main;
            mainPart.simulationSpeed = 1;
            breakingParticle.GetParticles(arrayBreakingParticle);
            float simTime = arrayBreakingParticle[0].startLifetime - arrayBreakingParticle[0].remainingLifetime - Time.deltaTime/6;
            breakingParticle.Simulate(simTime, false, true);
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



}
