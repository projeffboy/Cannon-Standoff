using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cannon : MonoBehaviour {
    public bool isBeingControlled;
    public int rotateSpeed = 40;
    public float velocity = 1;
    public Text controlCannonText;
    public Text velocityText;

    void Start() {
        toggleControlCannon();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            isBeingControlled = !isBeingControlled;

            toggleControlCannon();
        }

        if (isBeingControlled) {
            float tilt = transform.rotation.eulerAngles.z;
            if (tilt >= 180) {
                tilt = tilt - 360; // euler angles doesn't return negative values
            }
            float vertInput = Input.GetAxis("Vertical");

            if (
                !(vertInput < 0 && tilt <= 0)
                && !(vertInput > 0 && tilt >= 90)
            ) {
                transform.Rotate(0, 0, vertInput * Time.deltaTime * rotateSpeed);
            }

            float hInput = Input.GetAxis("Horizontal");
            if (hInput != 0) {
                velocity += hInput * Time.deltaTime;
                velocity = Mathf.Clamp(velocity, 0.1f, 3f);

                toggleVelocity();
            }
        }
    }

    private void toggleControlCannon() {
        if (controlCannonText != null) {
            controlCannonText.text = (isBeingControlled ? "Pink" : "Blue")
                + " cannon in control";

            Color pink = new Color(255, 0, 148);
            Color teal = new Color(0, 255, 255);
            controlCannonText.color = isBeingControlled ? pink : teal;
        }

        toggleVelocity();
    }

    private void toggleVelocity() {
        if (velocityText != null && isBeingControlled) {
            float roundedVelocity = (float) System.Math.Round(velocity, 2);

            velocityText.text = "Muzzle Velocity: " + roundedVelocity;

            if (roundedVelocity == 0.1f || roundedVelocity == 3) {
                velocityText.color = Color.red;
            } else {
                velocityText.color = Color.black;
            }
        }
    }
}
