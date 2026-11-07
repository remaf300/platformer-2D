using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
	public Rigidbody2D rb;
	[SerializeField] Animator animator;
    [SerializeField] BoxCollider2D boxCollider;

    public float moveSpeed;
	public int damage;

	[Header("Health")]
	public ParticleSystem hitEffect;
	public int maxHealth;
	public int currentHealth;
	public float invincibilityTime;
	bool isInvincible;

	bool isInRange;

	PlayerMovement playerScript;

	[Header("TurnPoint")]
	public Transform turnPoint;
	public LayerMask collisionLayers;
	public float turnPointradius;
	public bool mustTurn;

	[Header("TurnPointHole")]
	public Transform turnPointHole;
	public LayerMask collisionLayersHole;
	public float turnPointHoleradius;
	public bool mustTurnHole;

	[Header("CrushThings")]
	[SerializeField] float bounceForce;
	[SerializeField] Transform crushPoint;
    public LayerMask crushLayer;
    public float crushPointsize;
	bool isCrushing;

	[Header("Spleep")]
	[SerializeField] bool isSpleeping;

    [Header("Audio")]
    [SerializeField] AudioClip soundsToPlay;
    [SerializeField, Range(0, 1)] float soundVolume = 1;


    bool isDead;
	bool haskill;

    private void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

		currentHealth = maxHealth;
	}

    private void Update()
	{
        if (isDead)
        {
            rb.velocity = Vector3.zero;

            return;
        }

        isCrushing = Physics2D.OverlapCircle(crushPoint.position, crushPointsize, crushLayer);

        Crush();

        if (isInRange && !haskill && !isDead)
        {
			haskill = true;

            playerScript.KillPlayer();
		}

        if (isSpleeping)
        {
            animator.SetBool("IsSpleeping", true);
            return;
        }

        Walk();

        mustTurn = Physics2D.OverlapCircle(turnPoint.position, turnPointradius, collisionLayers);

        mustTurnHole = Physics2D.OverlapCircle(turnPointHole.position, turnPointHoleradius, collisionLayersHole);
    }

	void Crush()
	{
		if (isCrushing && !isDead)
		{
			isDead = true;

			if(Physics2D.OverlapCircle(crushPoint.position, crushPointsize, crushLayer))
			{
                Physics2D.OverlapCircle(crushPoint.position, crushPointsize, crushLayer).gameObject.GetComponent<PlayerMovement>().Bounce(bounceForce);
            }

            CrushDie();
        }
	}

    void CrushDie()
    {
        hitEffect.Play();
        animator.SetBool("IsCrushDead", true);

        AudioManager.instance.PlayClipAt(soundsToPlay, transform, soundVolume);

        boxCollider.enabled = false;

        isDead = true;

        Destroy(gameObject, 3);
    }

    public void TakeDamage(int damage)
    {
		if (isInvincible) return;

		currentHealth -= damage;

		hitEffect.Play();

		if (currentHealth <= 0 && !isDead)
        {
			currentHealth = 0;
            isDead = true;
            Die();
			return;
        }

		StartCoroutine(Invincibility());
		isInvincible = true;
	}

	IEnumerator Invincibility()
    {
		yield return new WaitForSeconds(invincibilityTime);

		isInvincible = false;
	}

	void Die()
    {
		animator.SetBool("IsDead", true);

		boxCollider.enabled = false;

        AudioManager.instance.PlayClipAt(soundsToPlay, transform, soundVolume);

		Destroy(gameObject, 3);
    }

	void Walk()
	{
		rb.AddForce(transform.right * moveSpeed);

		if (rb.velocity.x > 5)
		{
			rb.velocity = new Vector2(5, rb.velocity.y);
	    }

		if (rb.velocity.x < -5)
		{
			rb.velocity = new Vector2(-5, rb.velocity.y);
		}

		if (mustTurn)
		{
			Turn();
		}

		if (!mustTurnHole)
		{
			Turn();
		}
	}

	void Turn()
	{
		if (transform.eulerAngles == new Vector3(0, 0, 0))
		{
			transform.eulerAngles = new Vector3(0, 180, 0);
		}
		else if (transform.eulerAngles == new Vector3(0, 180, 0))
		{
			transform.eulerAngles = new Vector3(0, 0, 0);
		}

		float velocityY = rb.velocity.y;
		rb.velocity = new Vector2(0, velocityY);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(turnPoint.position, turnPointradius);

		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(turnPointHole.position, turnPointHoleradius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(crushPoint.position, crushPointsize);
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
}
