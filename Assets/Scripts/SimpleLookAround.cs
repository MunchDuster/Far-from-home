using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLookAround : MonoBehaviour
{
    public float maxX = 30;

    public float minX = -30;

    public float maxY = 30;

    public float minY = -30;

    float x;

    float y;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float deltaX = Input.GetAxis("Mouse X");
        float deltaY = Input.GetAxis("Mouse Y");

        x += deltaX;
        y += deltaY;

        x = Mathf.Clamp(x, minX, maxX);
        y = Mathf.Clamp(y, minY, maxY);

        transform.localRotation = Quaternion.Euler(y, x, 0);
    }
}
