using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour {
    public float gravity = -9.81f;
    public float secondsLeft = 5;

    // The two variables underneath must be set by another script when the cannon ball is created
    [HideInInspector]
    public float speedMult = 1; // placeholder value
    [HideInInspector]
    public float degAngle = 90; // placeholder
    [HideInInspector]
    public bool shootRight = true; // placeholder

    private float cannonLength = 2.5f;
    private float speedX;
    private float speedY;

    private void Start() {
        int direction_X = shootRight ? 1 : -1;
        float rad_angle = Mathf.Deg2Rad * degAngle;
        float x_norm = direction_X * Mathf.Cos(rad_angle);
        float y_norm = Mathf.Sin(rad_angle);

        transform.position += cannonLength / 4 * new Vector3(x_norm, y_norm, 0);

        float initialSpeed = 5f;
        float magnitude = speedMult * initialSpeed;

        speedX = magnitude * x_norm;
        speedY = magnitude * y_norm;


    }

    private void Update() {
        secondsLeft -= Time.deltaTime;
        if (secondsLeft <= 0) {
            gameObject.SetActive(false);
        }

        transform.position += new Vector3(speedX, speedY, 0) * Time.deltaTime;
        speedY += gravity * Time.deltaTime;
    }
}
