using System;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] bool isInRunLevel;

    public GameObject player;
    public float timeOffset;
    public Vector3 posOffset;

    private Vector3 velocity;

    Camera mainCamera;

    Bounds cameraBounds;
    Vector3 targetPosition;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        var height = mainCamera.orthographicSize;
        var width = height * mainCamera.aspect;

        var minX = Globals.WorldBounds.min.x + width;
        var maxX = Globals.WorldBounds.extents.x - width;

        var minY = Globals.WorldBounds.min.y + height;
        var maxY = Globals.WorldBounds.extents.y - height;

        cameraBounds = new Bounds();
        cameraBounds.SetMinMax(new Vector3(minX, minY, 0), new Vector3(maxX, maxY, 0));
    }

    void Update()
    {
        if (player == null) return;

        posOffset = new Vector3(5 * Input.GetAxisRaw("Horizontal"), posOffset.y, posOffset.z);

        if (isInRunLevel)
        {
            targetPosition = new Vector3(5, 0) + player.transform.position;
        }
        else
        {
            targetPosition = new Vector3(0, 0) + player.transform.position;
        }

        targetPosition = GetCameraBounds();


        //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z) + posOffset, ref velocity, timeOffset);

        transform.position = targetPosition;
    }

    Vector3 GetCameraBounds()
    {
        return new Vector3(
            Mathf.Clamp(targetPosition.x, cameraBounds.min.x, cameraBounds.max.x),
            Mathf.Clamp(targetPosition.y, cameraBounds.min.y, cameraBounds.max.y),
            transform.position.z
        );
    }
}
