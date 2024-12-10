using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundBrush : TerrainBrush {
    public int baseHeight = 25;
    public override void draw(int x, int z) {
        float height = Mathf.Max(radius, baseHeight) ; // Base height
        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {
                float distance = Mathf.Sqrt(Mathf.Pow(xi, 2) + Mathf.Pow(zi, 2));

                if (distance <= radius) {
                    float falloff = Mathf.Cos((distance / radius) * Mathf.PI / 2);
                    float adjustedHeight = height * falloff;

                    float currHeight = terrain.get(x + xi, z + zi);
                    if (currHeight <= adjustedHeight) {
                        terrain.set(x + xi, z + zi, adjustedHeight);
                    }
                }
            }
        }
    }

}
