using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zipline : MonoBehaviour
{
    [SerializeField] Zipline targetZipline;
    [SerializeField] float zipSpeed = 5f;
    [SerializeField] float maxSpeed = 15f;
    [SerializeField] float zipScale = 0.2f;

    [SerializeField] float arrivalThreshold = 0.4f;
    [SerializeField] LineRenderer cable;

    public Transform ZipTransform;

    bool zipping = false;
    public GameObject localZip;

    PlayerMovement pMovement;

    private void Awake()
    {
        cable.SetPosition(0, ZipTransform.position);
        cable.SetPosition(1, targetZipline.ZipTransform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !zipping)
        {
            pMovement = collision.gameObject.GetComponent<PlayerMovement>();
            StartZipline(collision.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!zipping || localZip == null) return;

        if (pMovement.isInZiplineButHasJump)
        {
            pMovement.isInZiplineButHasJump = false;
            ResetZipline();
        }

        localZip.GetComponent<Rigidbody2D>().velocity = (targetZipline.ZipTransform.position - ZipTransform.position).normalized * zipSpeed;

        if (Vector2.Distance(localZip.transform.position, targetZipline.ZipTransform.transform.position) <= arrivalThreshold)
        {
            ResetZipline();
        }
    }

    public void StartZipline(GameObject player)
    {
        if (zipping) return;

        // Set the zip
        localZip.transform.position = ZipTransform.position;

        // Set the player
        pMovement.isInZiplineButHasJump = false;
        player.GetComponent<Rigidbody2D>().gravityScale = 0;
        player.GetComponent<Rigidbody2D>().isKinematic = true;
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        player.GetComponent<PlayerMovement>().isInZipline = true;
        //StopAllCoroutines();
        player.transform.parent = localZip.transform;
        zipping = true;
    }

    void ResetZipline()
    {
        if (!zipping) return;

        GameObject player = localZip.transform.GetChild(0).gameObject;
        player.GetComponent<Rigidbody2D>().isKinematic = false;
        //StartCoroutine(SetIsInZipline());
        player.GetComponent<PlayerMovement>().isInZipline = false;
        player.transform.parent = null;
        player.GetComponent<Rigidbody2D>().gravityScale = player.GetComponent<PlayerMovement>().initialGravityScale;
        player.GetComponent<Rigidbody2D>().AddForce(new Vector2(Input.GetAxis("Horizontal"), 0) * 20);

        localZip.transform.position = ZipTransform.position;
        zipping = false;
    }
}
