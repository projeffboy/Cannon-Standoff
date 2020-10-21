using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour {
    public float gravity = -9.81f;
    public float baseSpeed = 5;
    public float restitutionCoefficient = 0.75f;
    public float secondsLeft = 10;

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
    private bool still = false;

    private void Start() {
        int direction_X = shootRight ? 1 : -1;
        float rad_angle = Mathf.Deg2Rad * degAngle;
        float x_norm = direction_X * Mathf.Cos(rad_angle);
        float y_norm = Mathf.Sin(rad_angle);

        transform.position += cannonLength / 4 * new Vector3(x_norm, y_norm, 0);

        float magnitude = speedMult * baseSpeed;

        speedX = magnitude * x_norm;
        speedY = magnitude * y_norm;
    }

    private void Update() {
        secondsLeft -= Time.deltaTime;
        if (secondsLeft <= 0) {
            gameObject.SetActive(false);
        }

        if (Mathf.Pow(speedX, 2) + Mathf.Pow(speedY, 2) < 0.0001) { // 0.01 squared
            still = true;
        }

        if (!still) {
            transform.position += new Vector3(speedX, speedY, 0) * Time.deltaTime;
            speedY += gravity * Time.deltaTime;

            Debug.DrawRay(transform.position, new Vector3(speedX, speedY, 0), Color.grey);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(speedX, speedY, 0), 0.1f);
            if (hit.collider != null) {
                if (hit.collider.gameObject.CompareTag("Valley")) {
                    if (hit.normal.x < 0) {
                        speedX *= -restitutionCoefficient;
                    }
                    speedY *= -restitutionCoefficient;
                } else if (hit.collider.gameObject.CompareTag("Water")) {
                    gameObject.SetActive(false);
                } else if (hit.collider.gameObject.CompareTag("Balloon Head")) {
                    hit.collider.transform.parent.gameObject.SetActive(false);
                }
            }

            float pos_x = transform.position.x;
            float pos_y = transform.position.y;
            if (pos_x < 0 || pos_x > 18 || pos_y < 0 || pos_y > 10) {
                gameObject.SetActive(false);
            }
        }
    }
}
