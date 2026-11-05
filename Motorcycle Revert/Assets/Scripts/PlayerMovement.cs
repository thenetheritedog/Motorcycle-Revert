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
    }

    // Update is called once per frame
    void Update()
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
            velocity = slopeEffect * groundTangent;
            velocity -= velocity * (1 - friction) * Time.deltaTime;

            gravity = -fallGravity;
            transform.right = groundTangent.normalized;

        }
        else
        {
            velocity.x += inputDirection.x * acceleration/5 * Time.deltaTime;
            velocity.x -= (velocity.x * (1-drag)) * Time.deltaTime;
            velocity.x += player.speedUsing/5 * Time.deltaTime;
            velocity.y += gravity * Time.deltaTime;
            transform.right = Vector3.Lerp(transform.right, Vector3.right, Time.deltaTime);
            gravity -= gravityFactor * Time.deltaTime;
        }
        player.cameraManager.AlignCameraY(grounded);
        rb.linearVelocity = velocity;
    }

    public void MoveChange(Vector2 input)
    {
        inputDirection = input;
    }

    bool CheckGround() 
    {
        if (rb.linearVelocityY > 0 && !grounded)
            return false;
        groundHit = Physics2D.Raycast(transform.position, -transform.up, 1f, groundLayer);
        if (grounded)
        {
            //transform.position = groundHit.point + Vector2.up/2;
        }
        else if (groundHit.distance > 0.55f)
        {
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

    IEnumerator JumpHold()
    {
        gravityFactor = jumpGravity;
        yield return new WaitForSeconds(0.2f);
        gravityFactor = fallGravity;
    }
}

