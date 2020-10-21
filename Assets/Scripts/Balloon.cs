using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour {
    public float balloonSpeed = 2;
    [HideInInspector]
    public Wind windScript;

    private void Update() {
        transform.position += balloonSpeed * Vector3.up * Time.deltaTime;

        Vector3 pos = transform.position;
        if ( // the balloons (incl string) are 0.5 wide and 1.75 tall
            pos.y > 11.75f
            || pos.x < -0.5
            || pos.x > 18.5
        ) {
            gameObject.SetActive(false);
        } else if (transform.position.y > 4) {
            transform.position += windScript.wind * Vector3.right * Time.deltaTime;
        }
    }
}
