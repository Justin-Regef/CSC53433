using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothingBrush : TerrainBrush {
    public int baseHeight = 25;
    public override void draw(int x, int z) {
    for (int zi = -radius; zi <= radius; zi++) {
        for (int xi = -radius; xi <= radius; xi++) {
            float distance = Mathf.Sqrt(xi * xi + zi * zi);
            if (distance > radius) continue;

            float avgHeight = 0f;
            int count = 0;

            for (int dz = -1; dz <= 1; dz++) {
                for (int dx = -1; dx <= 1; dx++) {
                    if (dz == 0 && dx == 0) continue;

                    float neighborHeight = terrain.get(x + xi + dx, z + zi + dz);
                    avgHeight += neighborHeight;
                    count++;
                }
            }

            avgHeight /= count; 

            float currentHeight = terrain.get(x + xi, z + zi);
            float smoothedHeight = Mathf.Lerp(currentHeight, avgHeight, 0.5f); 

            terrain.set(x + xi, z + zi, smoothedHeight);
        }
    }
}


}
