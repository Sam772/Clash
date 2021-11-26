using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float movementSpeed;
    public float movementTime;

    public Transform cameraTransform;
    public Vector3 zoomAmount;
    public Vector3 newZoom;

    public Vector3 newPosition;

    void Start() {
        newPosition = transform.position;
        newZoom = cameraTransform.localPosition;
    }

    void Update() {
        HandleMovementInput();
    }
 
    void HandleMovementInput() {

        // || Input.GetKey(KeyCode.UpArrow
        if (Input.GetKey(KeyCode.T)) {
            newPosition += (transform.TransformDirection(0, 1, 0) * movementSpeed);
        }
        if (Input.GetKey(KeyCode.G)) {
            newPosition += (transform.TransformDirection(0, 1, 0) * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.H)) {
            newPosition += (transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.F)) {
            newPosition += (transform.right * -movementSpeed);
        }

        // for zooming
        if (Input.GetKey(KeyCode.R)) {
            newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.Y)) {
            newZoom -= zoomAmount;
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
