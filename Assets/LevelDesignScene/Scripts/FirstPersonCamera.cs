using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 2f;
    float cameraVerticalRotation = 0f;

    public Quaternion ogRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        ogRotation = Quaternion.Euler(0, 270, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.GetComponent<PlayerController>().uiManager.menuOpen &&
            !player.GetComponent<PlayerController>().uiManager.givingHint)
        {
            float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            cameraVerticalRotation -= inputY;
            cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
            transform.localEulerAngles = Vector3.right * cameraVerticalRotation;

            player.Rotate(Vector3.up * inputX);
        }
    }

    public void resetRotation()
    {
        cameraVerticalRotation = 0;
        player.SetPositionAndRotation(player.gameObject.GetComponent<PlayerController>().ogPos, ogRotation);
    }
}
