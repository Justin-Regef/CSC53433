using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularInstanceBrush : InstanceBrush {

    public override void draw(float x, float z) {
        for (int i = 0; i < 10; i++) {
            float r = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;
            float angle = Random.Range(0f, Mathf.PI * 2);

            // Convert polar to Cartesian coordinates
            float xOffset = r * Mathf.Cos(angle);
            float zOffset = r * Mathf.Sin(angle);

            // Spawn object at the calculated position
            spawnObject(x + xOffset, z + zOffset);
        }
    }
}
