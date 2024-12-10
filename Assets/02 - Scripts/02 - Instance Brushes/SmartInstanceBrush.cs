using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartInstanceBrush : InstanceBrush {
    private int density = 3;
    private Vector2 lastPosition;
    private float minDistance = 10f; 
    private float maxSlopeAngle = 30f; 
    private float flatnessThreshold = 0.2f; // Threshold to determine flatness
    private float flatnessPercentage = 0.9f; // Percentage of the circle that must be flat to use prefabIdx = 1
    
    public override void draw(float x, float z) {
        int flatPoints = 0;
        int totalPoints = 0;
        Vector2 currentPosition = new Vector2(x, z);
        if (Vector2.Distance(currentPosition, lastPosition) < minDistance) {
            return; // Don't spawn new trees if the brush has not moved enough
        }
        lastPosition = currentPosition;

        // First pass: check for flatness in the circle
        for (int i = 0; i < 100; i++) {
            float r = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;  // Random radius within circle
            float angle = Random.Range(0f, Mathf.PI * 2);  // Random angle for circular distribution

            // Calculate the offset in the x and z directions
            float xOffset = r * Mathf.Cos(angle);
            float zOffset = r * Mathf.Sin(angle);

            // Get the normal at the new position
            Vector3 normal = terrain.getNormal(x + xOffset, z + zOffset);
            
            // Calculate the angle between the terrain normal and the vertical axis (Vector3.up)
            float slopeAngle = Vector3.Angle(normal, Vector3.up);
            
            // Check if the slope angle is less than the threshold to be considered flat
            if (slopeAngle < flatnessThreshold * 90f) {
                flatPoints++;
            }
            totalPoints++;
        }


        bool isFlat = (flatPoints / (float)totalPoints) > flatnessPercentage;

        int prefabIdx = 0;
        if (isFlat) {
            prefabIdx = 3;
        }


        for (int i = 0; i < density; i++) {
            float r = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;
            float angle = Random.Range(0f, Mathf.PI * 2);

            float xOffset = r * Mathf.Cos(angle);
            float zOffset = r * Mathf.Sin(angle);

            float terrainX = x + xOffset;
            float terrainZ = z + zOffset;

            Vector3 normal = terrain.getNormal(terrainX, terrainZ);
            float slopeAngle = Vector3.Angle(normal, Vector3.up);

            if (slopeAngle <= maxSlopeAngle) {
                spawnObject(terrainX, terrainZ, prefabIdx);
            }
        }
    }

    public void spawnObject(float x, float z, int prefabIdx) {
        if (prefabIdx == -1) {
            return;
        }

        float scaleDiff = Mathf.Abs(terrain.max_scale - terrain.min_scale);
        float scaleMin = Mathf.Min(terrain.max_scale, terrain.min_scale);
        float scale = (float)CustomTerrain.rnd.NextDouble() * scaleDiff + scaleMin;

        terrain.spawnObject(terrain.getInterp3(x, z), scale, prefabIdx);
    }
}
