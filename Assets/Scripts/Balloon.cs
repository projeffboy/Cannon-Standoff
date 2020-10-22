using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour {
    public float balloonSpeed = 2;
    public LineRenderer head;
    public LineRenderer tail;
    public EdgeCollider2D headCollider;
    public EdgeCollider2D tailCollider;
    [HideInInspector]
    public Wind windScript;
    [HideInInspector]
    public bool cannonHitsTail;

    private Vector3[] original_head_points;
    private Vector3[] head_old_points;
    private Vector3[] head_current_points;
    private Vector3[] original_tail_points;
    private Vector3[] tail_old_points;
    private Vector3[] tail_current_points;
    private float prev_delta_time = 1;

    private void Start() {
        original_head_points = new Vector3[head.positionCount];
        head.GetPositions(original_head_points);

        head_old_points = new Vector3[head.positionCount];
        head_current_points = new Vector3[head.positionCount];
        head.GetPositions(head_old_points);
        head.GetPositions(head_current_points);

        original_tail_points = new Vector3[tail.positionCount];
        tail.GetPositions(original_tail_points);

        tail_old_points = new Vector3[tail.positionCount];
        tail_current_points = new Vector3[tail.positionCount];
        tail.GetPositions(tail_old_points);
        tail.GetPositions(tail_current_points);
    }

    private void Update() {
        OutOfBounds();

        // Approach without Verlets
        // transform.position += balloonSpeed * Vector3.up * Time.deltaTime;

        Vector3[][] head_result = VerletIntegration(
            head, head_old_points, head_current_points, true
        );
        head_old_points = head_result[0];
        head_current_points = head_result[1];

        VerletConstraintsHead();

        Vector2[] vectors = new Vector2[head.positionCount];
        for (int i = 0; i < head.positionCount; i++) {
            vectors[i] = (Vector2) head_current_points[i];
        }
        headCollider.points = vectors;

        Vector3[][] tail_result = VerletIntegration(
            tail, tail_old_points, tail_current_points, false
        );
        tail_old_points = tail_result[0];
        tail_current_points = tail_result[1];

        // tail must be connected to head
        tail.SetPosition(0, head_current_points[3]);
        tail_current_points[0] = tail.GetPosition(0);

        VerletConstraintsTail();

        Vector2[] vectors2 = new Vector2[tail.positionCount];
        for (int i = 0; i < tail.positionCount; i++) {
            vectors2[i] = (Vector2)tail_current_points[i];
        }
        tailCollider.points = vectors2;

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
        Vector3[] current_points,
        bool isHead
    ) {
        Vector3[] next_points = new Vector3[line.positionCount];

        for (int i = 0; i < line.positionCount; i++) {
            float wind = 0;
            if (isHead && current_points[i].y > 2) { // in parent's frame, in world space it's > 4
                wind = windScript.wind;
            } else if (!isHead && cannonHitsTail) {
                /*
                wind = 10 / Time.deltaTime;
                cannonHitsTail = false;
                */
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
            int j = (i + 1) % head.positionCount;

            Vector3 displacement = head_current_points[j] - head_current_points[i];
            float distance = displacement.magnitude;
            float half_angle = Vector3.Angle(displacement, Vector3.down);
            if (distance < 0.25 || distance > 0.45 || half_angle < 50 || half_angle > 70) {
                head_current_points[j] = head_current_points[i] + (original_head_points[j] - original_head_points[i]);
            }
        }
    }

    private void VerletConstraintsTail() {
        for (int i = 0; i < tail.positionCount - 1; i++) {
            int j = i + 1;

            Vector3 displacement = tail_current_points[j] - tail_current_points[i];
            float distance = displacement.magnitude;
            float half_angle = Vector3.Angle(displacement, Vector3.down);

            if (distance < 0.2 || distance > 0.3 || half_angle > 45) {
                tail_current_points[j] = tail_current_points[i] + (original_tail_points[j] - original_tail_points[i]);
            }
        }
    }

    /* STUFF THAT ENDED UP NOT WORKING OUT
    private void VerletConstraintsHead() {
        for (int i = 0; i < head.positionCount; i++) {
            int j = (i + 1) % head.positionCount;

            Vector3 displacement = head_current_points[j] - head_current_points[i];
            float dist_x = Mathf.Abs(displacement.x);
            float dist_y = Mathf.Abs(displacement.y);
            float sgn_x = Mathf.Sign(displacement.x);
            float sgn_y = Mathf.Sign(displacement.y);

            // x-distance constraint of 0, y-distance constraint of 0.25
            if (i == 1 || i == 4) {
                Helper(head_current_points, i, j, dist_x, dist_y, sgn_x, sgn_y, 0, 0.25f);
            }
            // x and y distance constraint of 0.25
            else {
                Helper(head_current_points, i, j, dist_x, dist_y, sgn_x, sgn_y, 0.25f, 0.25f);
            }
        }
    }

    private void VerletConstraintsTail() {
        for (int i = 0; i < tail.positionCount - 1; i++) {
            Vector3 displacement = tail_current_points[i + 1] - tail_current_points[i];
            float dist_x = Mathf.Abs(displacement.x);
            float dist_y = Mathf.Abs(displacement.y);
            float sgn_x = Mathf.Sign(displacement.x);
            float sgn_y = Mathf.Sign(displacement.y);

            Helper(tail_current_points, i, i + 1, dist_x, dist_y, sgn_x, sgn_y, 0, 0.25f);
        }
    }

    private void Helper(
        Vector3[] current_points, int i, int j,
        float dist_x, float dist_y, float sgn_x, float sgn_y,
        float correct_dist_x, float correct_dist_y  
    ) {
        float overflow_x = dist_x - correct_dist_x;
        float overflow_y = dist_y - correct_dist_y;

        float correction_x = sgn_x * overflow_x / 2 * Time.deltaTime * 10;
        if (overflow_x > 0.1f) {
            current_points[i] += correction_x * Vector3.right;
            current_points[j] += correction_x * Vector3.left;
        } else if (overflow_x < -0.1f) {
            current_points[i] -= correction_x * Vector3.right;
            current_points[j] -= correction_x * Vector3.left;
        }

        if (overflow_y > 0.1f) {
            float correction_y = sgn_y * overflow_y / 2 * Time.deltaTime * 10;

            current_points[i] += correction_y * Vector3.down;
            current_points[j] += correction_y * Vector3.up;
        }
    }
    */
}
