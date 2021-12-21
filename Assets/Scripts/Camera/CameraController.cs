using System;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private float movementSpeed = 6f;
    [SerializeField] private float scrollSpeed = 3f;
    [SerializeField] private float scrollMultiplier = 0.1f;
    private Transform cameraTransform;
    private float zoomAmount;
    private float startYPosition;
    public static string xAxis = "Horizontal";
    public static string yAxis = "Vertical";
        
    private void Awake() {
        zoomAmount = 1f;
        cameraTransform = transform;
        startYPosition = cameraTransform.position.y;
    }

    private void Update() { 
        HandleMovementInput();
        CameraZoomInput();
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

    private void CameraZoomInput() {
        
        const float minimumYPosition = 3f;
        var maximumYPosition = startYPosition;
        var targetYPos = Mathf.Lerp(minimumYPosition, maximumYPosition, zoomAmount);
            
        var cameraPosition = transform.position;
        cameraPosition.y = Mathf.Lerp(cameraPosition.y, targetYPos, Time.deltaTime * scrollSpeed);
        transform.position = cameraPosition;
    }
}