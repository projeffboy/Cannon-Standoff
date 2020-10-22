using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour {
    public float balloonSpeed = 2;
    public LineRenderer head;
    public LineRenderer tail;
    [HideInInspector]
    public Wind windScript;

    private Vector3[] head_old_points;
    private Vector3[] head_current_points;
    private Vector3[] tail_old_points;
    private Vector3[] tail_current_points;
    private float prev_delta_time = 1;

    private void Start() {
        head_old_points = new Vector3[head.positionCount];
        head_current_points = new Vector3[head.positionCount];
        head.GetPositions(head_old_points);
        head.GetPositions(head_current_points);

        tail_old_points = new Vector3[tail.positionCount];
        tail_current_points = new Vector3[tail.positionCount];
        tail.GetPositions(tail_old_points);
        tail.GetPositions(tail_current_points);
    }

    private void Update() {
        OutOfBounds();

        // Approach without Verlets
        // transform.position += balloonSpeed * Vector3.up * Time.deltaTime;

        Vector3[][] head_result = VerletIntegration(head, head_old_points, head_current_points);
        head_old_points = head_result[0];
        head_current_points = head_result[1];

        VerletConstraintsHead();

        Vector3[][] tail_result = VerletIntegration(tail, tail_old_points, tail_current_points);
        tail_old_points = tail_result[0];
        tail_current_points = tail_result[1];

        prev_delta_time = Time.deltaTime;
    }

    private void OutOfBounds() {
        if ( // the balloons (incl string) are 0.5 wide and 1.75 tall
             // i disabled world space for the lines, so they are in their parents' coordinate frame
            tail_current_points[4].y > 8.25f
        ) {
            gameObject.SetActive(false);
        }
        /* Approach without Verlets
            else if (transform.position.y > 4) {
            Vector3 current_points = new Vector3[line.positionCount];
            line.GetPositions(out current_points);

            for (int i = 0; i < line.positionCount; i++) {
                old_points[i] += windScript.wind * Vector3.right * Time.deltaTime;
            }

            transform.position += windScript.wind * Vector3.right * Time.deltaTime;
        }
        */
    }

    private Vector3[][] VerletIntegration(
        LineRenderer line,
        Vector3[] old_points,
        Vector3[] current_points
    ) {
        Vector3[] next_points = new Vector3[line.positionCount];

        for (int i = 0; i < line.positionCount; i++) {
            float wind = 0;
            if (current_points[i].y > 2) { // in parent's frame, in world space it's > 4
                wind = windScript.wind;
            }

            next_points[i] += current_points[i]
                - (current_points[i] - old_points[i]) * Time.deltaTime / prev_delta_time
                + new Vector3(wind, balloonSpeed, 0) * Time.deltaTime;
        }

        line.SetPositions(next_points);

        return new Vector3[][]{current_points, next_points};
    }

    private void VerletConstraintsHead() {
        for (int i = 0; i < head.positionCount; i++) {
            // Vector3.Distance(head)
        }
    }
}
