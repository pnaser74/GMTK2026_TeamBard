using UnityEngine;

[DisallowMultipleComponent]
public class ParallaxLayer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Parallax")]
    [Tooltip("0 means fixed to the camera. 1 means fixed in world space.")]
    [SerializeField, Range(-0.5f, 1.5f)]
    private float horizontalParallax = 0.2f;

    [Tooltip("Usually lower than horizontal parallax in a sidescroller.")]
    [SerializeField, Range(-0.5f, 1.5f)]
    private float verticalParallax = 0.05f;

    private Vector3 startingPosition;
    private Vector3 startingCameraPosition;

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        startingPosition = transform.position;

        if (cameraTransform != null)
        {
            startingCameraPosition = cameraTransform.position;
        }
    }

    private void LateUpdate()
    {
        if (cameraTransform == null)
        {
            return;
        }

        Vector3 cameraMovement = cameraTransform.position - startingCameraPosition;

        transform.position = new Vector3(
            startingPosition.x + cameraMovement.x * horizontalParallax,
            startingPosition.y + cameraMovement.y * verticalParallax,
            startingPosition.z
        );
    }
}