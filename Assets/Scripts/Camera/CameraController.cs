using System;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private float movementSpeed = 6f;
    [SerializeField] private float scrollSpeed = 3f;
    [SerializeField] private float scrollMultiplier = 0.1f;
    [SerializeField] private Camera gameCamera;
    private Transform cameraTransform;
    private float zoomAmount;
    private float startYPosition;
    private float startRotation;
    public static string xAxis = "Horizontal";
    public static string yAxis = "Vertical";
        
    private void Awake() {
        zoomAmount = 1f;
        cameraTransform = transform;
        startYPosition = cameraTransform.position.y;
        startRotation = cameraTransform.eulerAngles.x;
    }

    private void Update() { 
        HandleMovementInput();
        HandleZoomInput();
    }

    private void HandleMovementInput() {
        var xAxis = Input.GetAxis(CameraController.xAxis);
        var yAxis = Input.GetAxis(CameraController.yAxis);

        if (Mathf.Abs(xAxis) > 0.1f) {
            transform.position += Vector3.right * (movementSpeed * xAxis * Time.deltaTime);
        }

        if (Math.Abs(yAxis) > 0.1f) {
            transform.position += Vector3.forward * (movementSpeed * yAxis * Time.deltaTime);
        }

        var scrollDelta = Input.mouseScrollDelta;
        zoomAmount = Mathf.Clamp01(zoomAmount - scrollDelta.y * scrollMultiplier);
    }

    private void HandleZoomInput() {
        
        const float minimumYPosition = 3f;
        var maximumYPosition = startYPosition * 1.3f;
        var targetYPosition = Mathf.Lerp(minimumYPosition, maximumYPosition, zoomAmount);
            
        var cameraPosition = transform.position;
        cameraPosition.y = Mathf.Lerp(cameraPosition.y, targetYPosition, Time.deltaTime * scrollSpeed);
        transform.position = cameraPosition;

        var cameraRotation = cameraTransform.eulerAngles;
        var targetRotation = zoomAmount < 0.2f
            ? Mathf.Lerp(startRotation - 40f, startRotation, zoomAmount / 0.2f)
            : startRotation;

        cameraRotation.x = Mathf.Lerp(cameraRotation.x, targetRotation, Time.deltaTime * scrollSpeed);
        transform.eulerAngles = cameraRotation;
    }
}