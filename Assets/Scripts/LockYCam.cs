using UnityEngine;
using Cinemachine;

public class LockYCam : MonoBehaviour
{
    public float fixedYPosition = 0f; // The Y position you want to lock the camera to

    private CinemachineVirtualCamera virtualCamera;
    private Transform followTarget;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            followTarget = virtualCamera.Follow;
            Debug.Log("Follow target set to: " + followTarget.name);
        }
    }

    private void LateUpdate()
    {
        if (followTarget != null)
        {
            // Lock the camera's Y position to the fixed value
            Vector3 cameraPosition = transform.position;
            transform.position = new Vector3(followTarget.position.x, fixedYPosition, cameraPosition.z);
            Debug.Log($"Camera position locked to: {transform.position}");
        }
        else
        {
            Debug.LogWarning("Follow target is not assigned!");
        }
    }
}
