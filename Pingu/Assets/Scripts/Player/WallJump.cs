using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJump : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public LayerMask whatIsWall;
    public PlayerMovement pm;
    public Animator anim;

    [Header("WallBump")]
    public float maxBumpTime;
    private float bumpTimer;

    public bool bumping = false;
    public bool afterBump = false;

    [Header("WallJumping")]
    public float wallJumpUpForce;
    public float wallJumpBackForce;
    public float wallMaxJumps;
    private float wallJumpCounter;

    public KeyCode jumpKey = KeyCode.Space;
    public bool wallJumping = false;
    public bool startingWallJump = false;

    [Header("Detection")]
    public float detectionLenght;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;


    public RaycastHit frontWallHit;
    private bool wallFront;

    private Transform lastWall;
    public Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    float timer = 0.25f;

    private void Update()
    {
        WallCheck();
        StateMachine();

        if(bumping)
        {
            WallBumping();
        }
        if(wallJumping)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                EndWallJumping();
            }
        }
        else
        {
            timer = 0.25f;
        }

        if(afterBump)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            if(pm.grounded)
            {
                afterBump = false;
            }
        }
    }

    private void StateMachine()
    {
        if(wallFront && wallLookAngle < maxWallLookAngle && !pm.grounded)
        {
            if(!bumping && bumpTimer > 0 && new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude > pm.normalSpeed)
            {
                StartWallBump();
            }

            if(bumpTimer > 0 && bumping)
            {
                bumpTimer -= Time.deltaTime;
            }
            if(bumpTimer < 0 && !wallJumping)
            {
                StopWallBump();
                afterBump = true;
            }
        }
        else
        {
            if(!wallJumping)
                StopWallBump();
        }

        if(bumping && Input.GetKeyDown(jumpKey) && wallJumpCounter > 0)
        {
            bumping = false;
            StartWallJump();
        }
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLenght, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        if(Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, 4f, whatIsWall))
        {
            pm.cantSlide = true;
        }
        else
        {
            pm.cantSlide = false;
        }

        bool newWall = /*frontWallHit.transform != lastWall || */Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        if((wallFront && newWall) || pm.grounded)
        {
            bumpTimer = maxBumpTime;
            wallJumpCounter = wallMaxJumps;
        }
    }

    private void StartWallJump()
    {
        anim.SetBool("WallJumping", true);
        rb.velocity = Vector3.zero;
        startingWallJump = true;
        pm.jumping = true;
        anim.SetTrigger("WallJump");
        Invoke(nameof(WallJumping), 0.5f);
    }

    private void WallJumping()
    {
        anim.SetBool("WallJumping", false);
        rb.useGravity = true;
        startingWallJump = false;
        wallJumping = true;
        wallJumpCounter--;
        Vector3 forceToApply = transform.up * wallJumpUpForce + frontWallHit.normal * wallJumpBackForce;

        rb.velocity = Vector3.zero;
        rb.AddForce(forceToApply, ForceMode.Impulse);

        //orientation.forward = Vector3.Slerp(orientation.forward, frontWallHit.normal, Time.deltaTime * 300f);
    }

    private void EndWallJumping()
    {
        wallJumping = false;
    }

    private void StartWallBump()
    {
        bumping = true;
        anim.SetTrigger("Bumping");

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
    }

    private void WallBumping()
    {
        orientation.forward = Vector3.Slerp(orientation.forward, -frontWallHit.normal, Time.deltaTime * 100f);
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }

    private void StopWallBump()
    {
        rb.useGravity = true;
        bumping = false;
    }
}
