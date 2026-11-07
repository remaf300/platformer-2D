using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float airSpeed;
     public float maxMoveSpeed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] float _deAcceleration = 30;
    private bool isFacingRight = true;

    private float coyoteTime = 0.1f;
    private float coyoteTimeCounter;

    private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    public Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask jumpPlatformLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private TrailRenderer trailRenderer;

    [Header("Dash")]
    [SerializeField] float dashingVelocity = 14f;
    [SerializeField] float dashingTime = 0.5f;
    private Vector2 dashingDir;
    private bool isDashing;
    private bool isJumping;
    private bool hasJump;
    private bool canDash = true;
    [SerializeField] int jumpCount = 0;
    Dictionary<int, float> initialJumpVelocities = new Dictionary<int, float>();
    Coroutine currentJumpResetRoutine = null;
    bool isCoroutineStarted;
    float basedeAcceleration;

    [HideInInspector] public bool isJumpPlatformRightChecked;
    [HideInInspector] public bool isJumpPlatformLeftChecked;
    [HideInInspector] public bool isPlayerMoving;
    [HideInInspector] public float jumpPlatformForce;
    [HideInInspector] public float jumpPlatformTime;
     public int direction;
    public int currentdirection;
    [HideInInspector] public Vector3 jumpPlatformPoint;
    public bool isOnPlatform;
    public bool isInPropulse;
    Quaternion basePlayerRotation;
    public GrapplingGun grapplingGunScript;

    [Header("Sliding")]
    [SerializeField] float slideForce;
    [SerializeField] float slideTime;
    [SerializeField] float justSlideTime = 0.5f;
    [SerializeField] CapsuleCollider2D normalCollider;
    [SerializeField] CapsuleCollider2D crouchCollider;

    [Header("Attack")]
    [SerializeField] float kickTime;
    [SerializeField] float attackRadius;
    [SerializeField] LayerMask glassLayer;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask leverLayer;
    [SerializeField] Transform attackPoint;
    [SerializeField] TrailRenderer kickTrailRenderer;
    [SerializeField] TrailRenderer punchTrailRenderer;
    bool isAttacking;
    [SerializeField] public float initialGravityScale;

    [Header("Death")]
    [SerializeField] GameObject deadMenu;
    public bool isDead;

    [Header("Audio")]
    [SerializeField] AudioClip[] soundsToPlay;
    [SerializeField, Range(0, 1)] float soundVolume = 1;


    [HideInInspector] public bool isInZipline;
    [HideInInspector] public bool isInZiplineButHasJump;
    [HideInInspector] public bool isInDialogueRange;

    bool isCrouching = false;
    bool justSlide = false;
    bool hasSlide = false;
    bool hasSetJumpToFalseWhenDialogueEnds;
    bool hasAttacked;

    private void Awake()
    {
        basePlayerRotation = transform.rotation;

        float secondJumpInitialVelocity = jumpingPower + 4;

        float thirdJumpInitialVelocity = jumpingPower + 8;

        initialJumpVelocities.Add(0, jumpingPower);
        initialJumpVelocities.Add(1, jumpingPower);
        initialJumpVelocities.Add(2, secondJumpInitialVelocity);
        initialJumpVelocities.Add(3, thirdJumpInitialVelocity);

        initialGravityScale = rb.gravityScale;
    }

    void Update()
    {
        if (deadMenu.transform.GetComponentInParent<Menu>().isInPause)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Velocity", 0);
            return;
        }

        if (FindObjectOfType<DialogueManager>().isTalking)
        {
            isJumping = false;
            animator.SetBool("IsJumping", false);
            hasSetJumpToFalseWhenDialogueEnds = false;
            rb.velocity = Vector2.zero;
            animator.SetFloat("Velocity", 0);
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && !isInDialogueRange)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Faire un saut
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !isCrouching && !isInDialogueRange)
        {
            if (jumpCount < 3 && currentJumpResetRoutine != null)
            {
                StopCoroutine(currentJumpResetRoutine);
                isCoroutineStarted = false;
            }

            if (jumpCount == 3)
            {
                jumpCount = 0;
                animator.SetInteger("JumpCount", jumpCount);
            }
            else
            {
                jumpCount += 1;
                animator.SetInteger("JumpCount", jumpCount);
            }

            if (IsJumpPlatformChecked())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPlatformForce);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, initialJumpVelocities[jumpCount]);
            }

            if(soundsToPlay.Length != 0) AudioManager.instance.PlayClipAt(soundsToPlay[UnityEngine.Random.Range(0, soundsToPlay.Length)], transform, soundVolume);


            isJumping = true;
 
            jumpBufferCounter = 0f;
        }

        // Faire un triple saut si on est accroupi
        if (isCrouching && jumpBufferCounter > 0f && coyoteTimeCounter > 0f && Mathf.Abs(rb.velocity.x) == 0 && !isInDialogueRange)
        {
            StopCoroutine(currentJumpResetRoutine);
            isCoroutineStarted = false;

            rb.velocity = new Vector2(rb.velocity.x, initialJumpVelocities[3]);
            animator.SetInteger("JumpCount", 3);

            if (soundsToPlay.Length != 0) AudioManager.instance.PlayClipAt(soundsToPlay[UnityEngine.Random.Range(0, soundsToPlay.Length)], transform, soundVolume);


            isJumping = true;

            jumpBufferCounter = 0f;
        }


        // Saut en étant en tyrolienne
        if (isInZipline)
        {
            jumpCount = 0;
            animator.SetInteger("JumpCount", 0);

            if (Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.velocity = new Vector2(rb.velocity.x + 10, jumpingPower * 1.1f);
                isJumping = true;
                isInZipline = false;
                isInZiplineButHasJump = true;

                if (soundsToPlay.Length != 0) AudioManager.instance.PlayClipAt(soundsToPlay[UnityEngine.Random.Range(0, soundsToPlay.Length)], transform, soundVolume);
            }
        }

        // Relachement du saut donc force vers le bas
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f && !isInPropulse)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.8f);

            coyoteTimeCounter = 0f;
        }

        if (isJumping && !IsGrounded())
        {
            hasJump = true;
        }

        if (IsGrounded() && hasJump)
        {
            isJumping = false;
            hasJump = false;
        }

        if (IsGrounded() && !isCoroutineStarted && !Input.GetButtonDown("Jump"))
        {
            isCoroutineStarted = true;
            currentJumpResetRoutine = StartCoroutine(jumpResetRoutine());
        }

        if (grapplingGunScript.IsGrappling())
        {
            jumpCount = 0;
            animator.SetInteger("JumpCount", 0);
        }

        //Dash();

        Slide();
        Kick();

        animator.SetFloat("Velocity", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsGrappling", grapplingGunScript.IsGrappling());
        animator.SetBool("IsGrounded", IsGrounded());
        animator.SetBool("IsInZipline", isInZipline);

        if(hasSetJumpToFalseWhenDialogueEnds && !FindObjectOfType<DialogueManager>().isTalking)
        {
            animator.SetBool("IsJumping", false);
            hasSetJumpToFalseWhenDialogueEnds = true;
        }

        Flip();
    }

    IEnumerator jumpResetRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        if (IsGrounded())
        {
            jumpCount = 0;
            isCoroutineStarted = false;
            animator.SetInteger("JumpCount", jumpCount);
        }

        isCoroutineStarted = false;
    }

    #region Attack

    void Kick()
    {
        if(IsGrounded())
        {
            hasAttacked = false;
        }

        if (Input.GetButtonDown("Fire2") && !grapplingGunScript.IsGrappling() && !isInZipline)
        {
            if (IsGrounded())
            {
                if(Mathf.Abs(rb.velocity.x) > 5 && !isCrouching)
                {
                    animator.SetTrigger("WalkPunch");
                    punchTrailRenderer.emitting = true;

                    isAttacking = true;
                }
                else if(Mathf.Abs(rb.velocity.x) < 3 && !isCrouching)
                {
                    animator.SetTrigger("Punch");
                    punchTrailRenderer.emitting = true;

                    isAttacking = true;
                }
            }
            else if(!hasAttacked)
            {
                animator.SetTrigger("AirKick");
                animator.SetInteger("JumpCount", 0);
                kickTrailRenderer.emitting = true;
                jumpCount = 0;

                hasAttacked = true;

                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.gravityScale = 0.2f;
                isAttacking = true;
            }

            StartCoroutine(KickWait());
        }

        if (isAttacking)
        {
            // Pour détruire des vitres
            if(Physics2D.OverlapCircle(attackPoint.position, attackRadius, glassLayer))
            {
                BreakGlass glassToBreak = Physics2D.OverlapCircle(attackPoint.position, attackRadius, glassLayer).GetComponent<BreakGlass>();

                glassToBreak.Break();
            }

            if (Physics2D.OverlapCircle(attackPoint.position, attackRadius, enemyLayer))
            {
                Monster monster = Physics2D.OverlapCircle(attackPoint.position, attackRadius, enemyLayer).GetComponent<Monster>();

                monster.TakeDamage(2);
            }

            if (Physics2D.OverlapCircle(attackPoint.position, attackRadius, leverLayer))
            {
                Lever leverScript = Physics2D.OverlapCircle(attackPoint.position, attackRadius, leverLayer).GetComponent<Lever>();

                leverScript.ActiveLever();
            }
        }

    }

    IEnumerator KickWait()
    {
        yield return new WaitForSeconds(kickTime);

        SetKickToFalse();
    }

    void SetKickToFalse()
    {
        // Désactiver les trainées
        kickTrailRenderer.emitting = false;
        punchTrailRenderer.emitting = false;

        rb.gravityScale = initialGravityScale;

        isAttacking = false;
    }

    #endregion

    #region Slide

    void Slide()
    {
        if(vertical == -1)
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;

            hasSlide = false;
        }

        if (vertical == 1 && IsGrounded())
        {
            animator.SetBool("IsLookingHight", true);
        }
        else
        {
            animator.SetBool("IsLookingHight", false);
        }

        if (isCrouching && IsGrounded())
        {
            crouchCollider.enabled = true;
            normalCollider.enabled = false;

            animator.SetBool("IsSliding", true);
        }
        else
        {
            crouchCollider.enabled = false;
            normalCollider.enabled = true;

            animator.SetBool("IsSliding", false);
        }

        if (Mathf.Abs(rb.velocity.x) < 9) return;

        if (vertical <= -0.8f)
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;

            hasSlide = false;
        }

        if ((isCrouching || justSlide) && IsGrounded())
        {
            crouchCollider.enabled = true;
            normalCollider.enabled = false;

            animator.SetBool("IsSliding", true);
        }
        else
        {
            crouchCollider.enabled = false;
            normalCollider.enabled = true;

            justSlide = false;
            StopCoroutine(JustSlide());

            animator.SetBool("IsSliding", false);
        }

        if (isFacingRight && isCrouching && IsGrounded() && !hasSlide)
        {
            rb.AddForce(Vector2.right * slideForce);
            StartCoroutine(JustSlide());
            StartCoroutine(StopSlide());
        }
        else if(!isFacingRight && isCrouching && IsGrounded() && !hasSlide)
        {
            rb.AddForce(Vector2.left * slideForce);
            StartCoroutine(JustSlide());
            StartCoroutine(StopSlide());
        }
    }

    IEnumerator JustSlide()
    {
        justSlide = true;

        yield return new WaitForSeconds(justSlideTime);

        justSlide = false;
    }

    IEnumerator StopSlide()
    {
        yield return new WaitForSeconds(slideTime);

        hasSlide = true;
    }

    #endregion

    #region Propulse

    public void Propulse()
    {
        if (isInPropulse) return;

        StopCoroutine(StopPropulsePlayer());

        isInPropulse = true;

        isJumping = false;

        StartCoroutine(StopPropulsePlayer());
    }

    IEnumerator StopPropulsePlayer()
    {
        yield return new WaitForSeconds(jumpPlatformTime);

        isInPropulse = false;
    }

    #endregion

    #region Dash

    void Dash()
    {
        var dashInput = Input.GetButtonDown("Dash");

        if(dashInput && canDash)
        {
            isDashing = true;
            canDash = false;
            dashingDir = new Vector2(horizontal, vertical);

            if(dashingDir == Vector2.zero)
            {
                dashingDir = new Vector2(transform.localScale.x, 0);
            }

            StartCoroutine(StopDashing());
        }

        if (isDashing)
        {
            trailRenderer.emitting = true;

            if (dashingDir.x != 0 && dashingDir.y != 0)
            {
                rb.velocity = dashingDir.normalized * dashingVelocity / 1.2f;
            }

            if (dashingDir.x != 0 && dashingDir.y == 0)
            {
                rb.velocity = dashingDir.normalized * dashingVelocity;
            }
            else if (dashingDir.x == 0 && dashingDir.y != 0)
            {
                rb.velocity = dashingDir.normalized * dashingVelocity / 1.2f;
            }

            if (dashingDir.x == 0 && dashingDir.y == 0)
            {
                rb.velocity = dashingDir.normalized * dashingVelocity;
            }

            return;
        }

        if (IsGrounded())
        {
            canDash = true;
        }
    }

    IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        trailRenderer.emitting = false;
        isDashing = false;
    }

    #endregion

    #region Bounce

    public void Bounce(float force)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * force);
    }

    #endregion

    #region Death

    public void KillPlayer()
    {
        if (grapplingGunScript.IsGrappling()) grapplingGunScript.StopGrapple();

        animator.SetBool("Death", true);

        deadMenu.SetActive(true);

        rb.velocity = Vector2.zero;
        grapplingGunScript.enabled = false;
        enabled = false;
    }

    #endregion

    private void FixedUpdate()
    {
        if(isInPropulse)
        {
            return;
        }


        if (!IsGrounded() && isCrouching)
        {
            rb.AddForce(new Vector2(horizontal, 0f) * speed);
        }

        if (!isCrouching) rb.AddForce(new Vector2(horizontal, 0f) * speed);

        if (Mathf.Abs(rb.velocity.x) > maxMoveSpeed)
        {
            // Remet la velocitée à la vitesse maximale
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxMoveSpeed, rb.velocity.y);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public bool IsJumpPlatformChecked()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, jumpPlatformLayer);
    }

    private void Flip()
    {
        if (!isInZipline)
        {
            if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}