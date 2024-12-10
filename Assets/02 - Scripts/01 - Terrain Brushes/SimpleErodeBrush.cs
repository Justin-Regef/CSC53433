using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleErodeBrush : TerrainBrush {
    public float erode_rate = 0.99f;

    public override void draw(int x, int z) {
        float lowest_point = float.MaxValue; 

        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {
                float currHeight = terrain.get(x + xi, z + zi);
                if (currHeight < lowest_point) {
                    lowest_point = currHeight;
                }
            }
        }


        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {
                float distance = Mathf.Sqrt(xi * xi + zi * zi);

                if (distance <= radius) {
                    float normalizedDistance = Mathf.Clamp01(distance / radius);
                    float falloff = normalizedDistance;  // Inverted falloff

                    float currHeight = terrain.get(x + xi, z + zi);

                    // Apply height-proportional erosion
                    float heightFactor = Mathf.Clamp01(currHeight);  // Ensure valid range
                    float erosionStrength = erode_rate * heightFactor * falloff;

                    float erodedHeight = Mathf.Max(lowest_point, currHeight - erosionStrength);

                    // Ensure height doesn't go below zero
                    terrain.set(x + xi, z + zi, erodedHeight);
                }
            }
        }
    }

}
