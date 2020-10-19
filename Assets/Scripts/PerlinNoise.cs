using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : MonoBehaviour {
    public bool valleyNotWater;
    public float startingX = 0;

    private int noiseLevel = 8;

    private void Start() {
        LineRenderer line = GetComponent<LineRenderer>();

        int num_points = line.positionCount * noiseLevel;
        if (!valleyNotWater) {
            num_points *= 3;
        }
        num_points++;

        // Octave 1 with Interpolation
        float[] octave1 = new float[num_points];
        for (int i = 0; i < num_points; i++) {
            if (i % 2 == 0) {
                octave1[i] = Random.value / noiseLevel;
            } else {
                octave1[i] = 0;
            }
        }
        for (int i = 0; i < num_points - 1; i++) {
            if (i % 2 == 1) {
                octave1[i] = octave1[i + 1] - octave1[i - 1];
            }
        }

        // Octave 2
        float[] octave2 = new float[num_points];
        for (int i = 0; i < num_points; i++) {
            octave2[i] = Random.value / (2 * noiseLevel);
        }

        // Adding Octaves to Line
        Vector3[] old_points = new Vector3[line.positionCount];
        line.GetPositions(old_points);

        Vector3[] new_points = new Vector3[num_points];

        Vector3 start_point = line.GetPosition(0);
        Vector3 end_point = line.GetPosition(line.positionCount - 1);
        float line_length = (start_point - end_point).magnitude;

        for (int i = 0; i < num_points; i++) {
            float x = i * line_length / (num_points - 1) + startingX;

            new_points[i].x = x;

            if (valleyNotWater) {
                if (x < 2) {
                    new_points[i].y = 2;
                } else if (x < 4) {
                    new_points[i].y = x;
                } else if (x < 6) {
                    new_points[i].y = 4 - 1.5f * (x - 4);
                } else if (x < 12) {
                    new_points[i].y = 1;
                } else if (x < 14) {
                    new_points[i].y = 1 + 1.5f * (x - 12);
                } else if (x < 16) {
                    new_points[i].y = 4 - (x - 14);
                } else {
                    new_points[i].y = 2;
                }
            } else {
                new_points[i].y = 2;
            }

            new_points[i].y += octave1[i] + octave2[i];
        }

        line.positionCount = num_points;
        line.SetPositions(new_points);
    }
}
