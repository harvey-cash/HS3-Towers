using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private int multiplier = 30;
    private float redRotation = 75, greenRotation = 15;

    private const float lengthOfTime = 4;

    private Camera myCamera;
    private void Start() {
        myCamera = transform.GetChild(0).GetComponent<Camera>();
    }

    public IEnumerator ChangeTurn(Player.TEAM team) {
        float currentTime = Time.time;
        float rotation;

        if (team.Equals(Player.TEAM.GREEN)) {
            rotation = greenRotation;
        } else { rotation = redRotation; }

        while ((transform.rotation.y - rotation) < 5) {
            yield return new WaitForEndOfFrame();
            transform.rotation = Quaternion.Euler(
                new Vector3(0, Mathf.Lerp(transform.rotation.y, rotation,
                (Time.time - currentTime) / lengthOfTime), 0));
        }
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
