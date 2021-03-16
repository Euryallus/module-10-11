using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private GameObject playerCamera;
    [SerializeField]
    private CharacterController controller;

    private float mouseX;
    private float mouseY;
    private float rotateY = 0;
    private float inputX;
    private float inputY;
    private float defaultSpeed = 7;

    private Vector3 moveTo;

    private float mouseSensitivity = 400;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotateY -= mouseY;
        rotateY = Mathf.Clamp(rotateY, -75f, 75f);

        transform.Rotate(Vector3.up * mouseX);
        playerCamera.transform.localRotation = Quaternion.Euler(rotateY, 0, 0f);

        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        moveTo = transform.right * inputX + transform.forward * inputY;

        controller.Move(moveTo * defaultSpeed * Time.deltaTime); //applies movement to player
    }
}
