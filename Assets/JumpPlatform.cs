using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    [SerializeField] float jumpPlatformForce;
    [SerializeField] float jumpPlatformTime;
    [SerializeField] Transform jumpPlatformPoint;
    [SerializeField] bool mustJump;
    [SerializeField, Range(-1, 1)] int direction;
    bool isInRange;
    PlayerMovement player;
    Animator animator;

    bool isPlayerTransformSet;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;                
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    void Update()
    {
        if(mustJump)
        {
            if (player.IsJumpPlatformChecked())
            {
                player.jumpPlatformForce = jumpPlatformForce;
            }

            if (player.IsJumpPlatformChecked() && Input.GetButtonDown("Jump"))
            {
                animator.SetBool("IsActive", true);
            }
            else
            {
                animator.SetBool("IsActive", false);
            }
        }
        else
        {
            if (isInRange && !isPlayerTransformSet)
            {
                isPlayerTransformSet = true;

                animator.SetBool("IsActive", true);

                player.transform.position = jumpPlatformPoint.position;
                player.rb.velocity = new Vector2(0, 0);

                player.isInPropulse = false;

                player.jumpPlatformTime = jumpPlatformTime;
                player.rb.AddForce(new Vector2(direction, 0) * jumpPlatformForce * 100);
                player.rb.AddForce(Vector2.up * jumpPlatformForce * 50);

                player.Propulse();
            }
            else if(!isInRange)
            {
                isPlayerTransformSet = false;
                animator.SetBool("IsActive", false);
            }
        }
    }
}
