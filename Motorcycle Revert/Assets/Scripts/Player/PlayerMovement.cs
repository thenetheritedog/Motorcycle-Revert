using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] LayerMask groundLayer;
    [SerializeField] float acceleration;
    [SerializeField] float friction;
    [SerializeField] float drag;
    [SerializeField] float slopeFactor;

    [SerializeField] float fallGravity;
    [SerializeField] float jumpGravity;
    [SerializeField] float jumpPower;


    float gravityFactor;
    float gravity;
    bool grounded = false;
    ParticleSystem smokeParticles;
    Vector2 inputDirection;
    Rigidbody2D rb;
    RaycastHit2D groundHit;
    PlayerManager player;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerManager>();
        rb.linearVelocityX = player.speedUsing;
        gravityFactor = fallGravity;
        smokeParticles = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 velocity = rb.linearVelocity;
        if (CheckGround())
        {
            Vector2 forwardDir = new Vector2(transform.right.x, transform.right.y);
            velocity += ((inputDirection.x * acceleration) + player.speedUsing) * Time.deltaTime * forwardDir;
            velocity.y += gravity * Time.deltaTime;

            Vector2 groundTangent = new Vector2 (groundHit.normal.y, -groundHit.normal.x);
            float slopeEffect = Vector3.Dot(velocity, groundTangent);
            Debug.DrawRay(transform.position, groundTangent);
            if (Mathf.Abs(slopeEffect) < Vector3.Dot(Vector3.up * gravity, groundHit.normal))
            {
                grounded=false;
                velocity = slopeEffect * groundTangent;
                velocity.y += gravity * Time.deltaTime;
                gravity -= gravityFactor * Time.deltaTime;
                transform.right = Vector3.Lerp(transform.right, Vector3.right, Time.deltaTime);
            }
            else
            {
                velocity = slopeEffect * groundTangent;
                velocity -= velocity * (1 - friction) * Time.deltaTime;
                gravity = -jumpGravity;
            }

            

            
            transform.right = groundTangent.normalized;

        }
        else
        {
            velocity.x += inputDirection.x * acceleration/5 * Time.deltaTime;
            velocity.x -= (velocity.x * (1-drag)) * Time.deltaTime;
            velocity.y += gravity * Time.deltaTime;
            transform.right = Vector3.Lerp(transform.right, Vector3.right, Time.deltaTime);
            gravity -= gravityFactor * Time.deltaTime;
        }
        rb.linearVelocity = velocity;
        HandleSmokeParticles();
    }

    public void MoveChange(Vector2 input)
    {
        inputDirection = input;
    }

    bool CheckGround() 
    {
        Vector3 downVec = grounded ? -transform.up : Vector3.down;
        groundHit = Physics2D.BoxCast(transform.position, new Vector2(0.5f,0.1f), transform.eulerAngles.z, downVec, 1f, groundLayer);
        if (groundHit && grounded)
        {
            transform.position = (transform.position + (0.6f - groundHit.distance) * -downVec);
        }
        else if (groundHit.distance > 0.4f)
        {
            grounded = false;
            return false;
        }
        
        grounded = groundHit;
        return groundHit;
    }

    public void Jump(bool pressing)
    {
        if (CheckGround() && pressing)
        {
            StartCoroutine(JumpHold());
            rb.linearVelocity += groundHit.normal * jumpPower;
            gravity = rb.linearVelocity.y;
            grounded = false;
        }
        else
        {
            StopCoroutine(JumpHold());
            gravityFactor = fallGravity;
        }
            
    }

    void HandleSmokeParticles()
    {
        var emission = smokeParticles.emission;
        var particleMain = smokeParticles.main;
        if (CheckGround()) 
        {
            emission.rateOverTimeMultiplier = rb.linearVelocity.magnitude;


            
        }
        else
        {
            emission.rateOverTimeMultiplier = 0;


        }

        
    }

    IEnumerator JumpHold()
    {
        gravityFactor = jumpGravity;
        yield return new WaitForSeconds(0.1f);
        gravityFactor = fallGravity;
    }
}

