using UnityEngine;

public class RotateGun : MonoBehaviour
{
    public GrapplingGun grappling;

    private Quaternion desiredRotation;
    private float rotationSpeed = 5f;

    private void Update()
    {
        if (!grappling.IsGrappling())
        {
            desiredRotation = transform.parent.rotation;
        }
        else
        {
            desiredRotation = Quaternion.LookRotation(grappling.GetGrapplePoint() - transform.position);
        }

        transform.rotation = Quaternion.Lerp(new Quaternion(0, 0, transform.rotation.z, transform.rotation.w), new Quaternion(0, 0, desiredRotation.z * -1, desiredRotation.w * -1), Time.deltaTime * rotationSpeed);
    }
}
