using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    readonly float castingSpeedReduction = 0.35f;
    readonly float rollSpeedMultiplier = 2.3f;
    readonly float rollDuration = 0.8f;
    readonly float rollCooldown = 2.0f;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode rollKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("References")]
    [SerializeField] Transform orientation;
    [SerializeField] Animator playerAnim;
    [SerializeField] PlayerAttack playerAttackScript;

    // attribute maximums
    [HideInInspector]
    public float moveSpeedMax = 15.0f;

    [HideInInspector]
    public bool isRolling;

    // private variables
    float horizontalInput;
    float verticalInput;
    bool readyToJump = true;
    bool readyToRoll = true;
    Vector3 moveDirection;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        MyInput();
        SpeedControl();

        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        // update animator
        playerAnim.SetFloat("vertical", verticalInput);
        playerAnim.SetFloat("horizontal", horizontalInput);
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded && !playerAttackScript.isCasting && !playerAttackScript.damageBuffAnimationActive)
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // when to roll
        if (Input.GetKeyDown(rollKey) && readyToRoll && !isRolling && grounded && !playerAttackScript.isCasting && (horizontalInput != 0 || verticalInput != 0) && !playerAttackScript.damageBuffAnimationActive)
        {
            readyToRoll = false;
            StartCoroutine(ActivateRoll());

            Invoke(nameof(ResetRoll), rollCooldown);
        }
    }
    private void MovePlayer()
    {
        // ignore if rolling
        if (isRolling) return;

        // calc movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
        {
            if (playerAttackScript.isCasting)
            {
                // limit speed if casting
                rb.AddForce(10f * moveSpeed * castingSpeedReduction * moveDirection.normalized, ForceMode.Force);
            } else
            {
                rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);
            }
        } else
        {
            rb.AddForce(10f * airMultiplier * moveSpeed * moveDirection.normalized, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        // ignore if rolling
        if (isRolling) return;

        Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

        // limit general velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);

            // limit max speed if casting
            if (playerAttackScript.isCasting)
            {
                rb.velocity = new Vector3(limitedVel.x * castingSpeedReduction, rb.velocity.y, limitedVel.z * castingSpeedReduction);
            }
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // add jump force
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        playerAnim.SetTrigger("jump");
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    IEnumerator ActivateRoll()
    {
        // set roll variables
        float elapsedRollTime = 0f;
        float rollSpeed = moveSpeed * rollSpeedMultiplier;
        Vector3 rollDirection = moveDirection;

        // start animation
        playerAnim.SetTrigger("roll");

        // wait for animation to get to actual roll
        yield return new WaitForSeconds(0.1f);

        // mark stop of roll to stop player input movement and jumping
        isRolling = true;

        // while loop for gradual movement
        while (elapsedRollTime < rollDuration)
        {
            elapsedRollTime += Time.deltaTime;
            rb.velocity = rollDirection * rollSpeed;
            yield return null;
        }

        // mark end of roll, turning input controls and speed control back on
        isRolling = false;
    }

    private void ResetRoll()
    {
        readyToRoll = true;
    }
}
