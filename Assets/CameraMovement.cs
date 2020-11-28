using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    void Update()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        Vector3 pos = transform.position;
        pos += (Vector3)movement * Time.deltaTime * 25;
        
        pos.x = Mathf.Clamp(pos.x, -20, 20);
        pos.y = Mathf.Clamp(pos.y, -15, 15);

        pos.z = transform.position.z;
        
        transform.position = pos;

        if(Input.mouseScrollDelta.y > 0)
        {
            GetComponent<Camera>().orthographicSize--;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            GetComponent<Camera>().orthographicSize++;
        }
    }
}
