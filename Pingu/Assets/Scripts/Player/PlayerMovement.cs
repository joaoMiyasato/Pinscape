using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    public Animator anim;
    WallJump wj;
    public Collider normal1, normal2, slide1;

    [Header("Movement")]
    public float acceleration;
    public float normalSpeed;
    public float sprintSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;
    bool sprinting = false;
    public bool sliding = false;
    public bool jumping = false;
    public bool cantSlide = false;

    [Header("Colliders")]
    public Collider foot;
    public PhysicMaterial normalMaterial;
    public PhysicMaterial slipperyMaterial;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode slideKey = KeyCode.C;

    [Header("Ground Check")]
    public float playerHeight;
    public float adjust;
    public LayerMask whatIsGround;
    public bool grounded;

    public Transform orientation;
    
    float horizontaInput;
    float verticalInput;

    Vector3 moveDir;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        wj = GetComponent<WallJump>();
    }

    void Update()
    {
        if(rb.velocity.y < -0.1f)
        {
            anim.SetBool("caindo", true);
        }
        else
        {
            anim.SetBool("caindo", false);
        }

        grounded = Physics.Raycast(new Vector3(transform.position.x, transform.position.y - adjust, transform.position.z), Vector3.down, playerHeight * 0.5f + 0.1f, whatIsGround); 
        if(grounded){anim.SetBool("Grounded", true);}else{anim.SetBool("Grounded", false);}

        MyInput();
        SpeedControl();

        if(grounded)
        {
            if(!sliding)
            {
                normal1.enabled = true;
                normal2.enabled = true;
                slide1.enabled = false;
                rb.drag = groundDrag;
                foot.material = normalMaterial;
            }
            else
            {
                normal1.enabled = false;
                normal2.enabled = false;
                slide1.enabled = true;
                rb.drag = 0.45f;
                foot.material = slipperyMaterial;
            }
        }
        else
        {
            rb.drag = 0;
        }
    }
    void FixedUpdate()
    {
        if(!sliding)
            MovePlayer();
    }

    void MyInput()
    {
        horizontaInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(horizontaInput != 0f || verticalInput != 0f)
        {
            if(grounded && !sliding)
            {
                anim.SetBool("Walk", true);
            }
            else
            {
                anim.SetBool("Walk", false);
            }
        }
        else
        {
            anim.SetBool("Walk", false);
        }

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            anim.SetTrigger("Jump");
            
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if(Input.GetKey(sprintKey))
        {
            sprinting = true;
            anim.SetBool("Run", true);
        }
        else
        {
            sprinting = false;
            anim.SetBool("Run", false);
        }

        if(Input.GetKey(slideKey) && grounded)
        {
            if(new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude > 2f && !cantSlide)
            {
                sliding = true;
                anim.SetBool("Slide", true);
            }
        }
        else
        {
            sliding = false;
            anim.SetBool("Slide", false);
        }
    }

    void MovePlayer()
    {
        moveDir = orientation.forward * verticalInput + orientation.right * horizontaInput;

        if(grounded && !wj.startingWallJump && !wj.afterBump)
        {
            rb.AddForce(moveDir.normalized * acceleration * 10f, ForceMode.Force);
        }
        else if(!grounded && !wj.startingWallJump && !wj.afterBump)
        {
            rb.AddForce(moveDir.normalized * acceleration * 10f * airMultiplier, ForceMode.Force);
        }
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(!sliding)
        {
            if(flatVel.magnitude > normalSpeed && !sprinting && !wj.wallJumping && grounded)
            {
                Vector3 limitedVel = flatVel.normalized * normalSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
            else if(flatVel.magnitude > sprintSpeed && sprinting && !wj.wallJumping && grounded)
            {
                Vector3 limitedVel = flatVel.normalized * sprintSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
            else if(!grounded && !wj.wallJumping)
            {
                Vector3 limitedVel = flatVel.normalized * sprintSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
            else if(wj.wallJumping)
            {
                Vector3 limitedVel = flatVel.normalized * wj.wallJumpBackForce;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

        }
    }

    void Jump()
    {
        // Debug.Log("Jumping");
        jumping = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        jumping = false;
        readyToJump = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + adjust - playerHeight * 0.5f + 0.1f, transform.position.z));
    }
}
