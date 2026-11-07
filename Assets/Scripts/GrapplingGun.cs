using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    public GameObject grapplingGun;
    public Transform grapplingPoint;
    public LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip;
    private Transform mainCamera, player;
    private GameObject cameraObj, playerObj, weaponHolderObj;
    public float maxDistance = 100f;
    private SpringJoint2D joint;
    public GameObject grapplingIndicator;
    GameObject currentGrapplingIndicator;

    [HideInInspector] public bool isRunFinished;

    private void Awake()
    {
        //cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        playerObj = GameObject.FindGameObjectWithTag("Player");
        //weaponHolderObj = GameObject.FindGameObjectWithTag("WeaponHolder");
        player = playerObj.transform;
        //mainCamera = cameraObj.transform;
        //grapplingGun = GameObject.FindGameObjectWithTag("GrapplingGun");
        lr = grapplingGun.GetComponent<LineRenderer>();

        lr.positionCount = 0;
    }

    private void Update()
    {
        if (isRunFinished) return;

        if (Input.GetAxis("Fire1") == -1 && !IsGrappling())
        {
            StartGrapple();
        }
        else if ( Input.GetAxis("Fire1") != -1 && IsGrappling())
        {
            StopGrapple();
        }

        

        if (Physics2D.OverlapCircle(grapplingPoint.position, maxDistance, whatIsGrappleable) != null && !IsGrappling())
        {
            if (currentGrapplingIndicator == null)
            {
                GameObject thingToGrapple = Physics2D.OverlapCircle(grapplingPoint.position, maxDistance, whatIsGrappleable).gameObject;

                if (thingToGrapple != null)
                {
                    currentGrapplingIndicator = Instantiate(grapplingIndicator, thingToGrapple.transform);
                    currentGrapplingIndicator.transform.position = thingToGrapple.transform.position;
                }
            }
        }
        else if(currentGrapplingIndicator != null)
        {
            currentGrapplingIndicator.transform.GetComponentInChildren<Animator>().SetTrigger("Fade");

            Destroy(currentGrapplingIndicator, 0.5f);
        }
    }

    // called after Update
    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        if(Physics2D.OverlapCircle(grapplingPoint.position, maxDistance, whatIsGrappleable))
        {
            GameObject thingToGrapple = Physics2D.OverlapCircle(grapplingPoint.position, maxDistance, whatIsGrappleable).gameObject;

            if (thingToGrapple != null)
            {
                grapplePoint = thingToGrapple.transform.position;
                joint = player.gameObject.AddComponent<SpringJoint2D>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = grapplePoint;

                float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

                // the distance grapple will to keep from grapple point.
                //joint.maxDistance = distanceFromPoint * 0.8f;
                //joint.minDistance = distanceFromPoint * 0.25f;
                joint.autoConfigureDistance = false;
                joint.distance = distanceFromPoint * 0.55f;

                // change these values to fit your game.
                //joint.spring = 4.5f;
                //joint.damper = 7f;
                //joint.massScale = 4.5f;
                joint.dampingRatio = 1f;
                joint.enableCollision = true;

                lr.positionCount = 2;
            }
        }
        //RaycastHit hit;
        //if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, maxDistance, whatIsGrappleable))
        //{
        //    grapplePoint = hit.point;
        //    joint = player.gameObject.AddComponent<SpringJoint>();
        //    joint.autoConfigureConnectedAnchor = false;
        //    joint.connectedAnchor = grapplePoint;

        //    float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

        //    // the distance grapple will to keep from grapple point.
        //    joint.maxDistance = distanceFromPoint * 0.8f;
        //    joint.minDistance = distanceFromPoint * 0.25f;

        //    // change these values to fit your game.
        //    joint.spring = 4.5f;
        //    joint.damper = 7f;
        //    joint.massScale = 4.5f;

        //    lr.positionCount = 2;
        //}
    }

    void DrawRope()
    {
        // if not grappling, don't draw rope
        if (!joint) return;

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);
    }

    public void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(grapplingPoint.position, maxDistance);
    }
}
