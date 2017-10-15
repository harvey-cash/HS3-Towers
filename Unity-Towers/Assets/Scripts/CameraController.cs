using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private int multiplier = 30;

    private Camera myCamera;
    private void Start() {
        myCamera = transform.GetChild(0).GetComponent<Camera>();
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            myCamera.orthographic = !myCamera.orthographic;
        }

        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.Rotate(Vector3.up * Time.deltaTime * multiplier);
        }
        else if (Input.GetKey(KeyCode.RightArrow)) {
            transform.Rotate(-Vector3.up * Time.deltaTime * multiplier);
        }

        if (myCamera.orthographic) {
            if (Input.GetKey(KeyCode.DownArrow)) {
                myCamera.orthographicSize += Time.deltaTime;
            } else if (Input.GetKey(KeyCode.UpArrow)) {
                myCamera.orthographicSize -= Time.deltaTime;
            }
        } else {
            if (Input.GetKey(KeyCode.DownArrow)) {
                myCamera.transform.position -= myCamera.transform.forward * Time.deltaTime;
            } else if (Input.GetKey(KeyCode.UpArrow)) {
                myCamera.transform.position += myCamera.transform.forward * Time.deltaTime;
            }
        }
        
    }
	
}
