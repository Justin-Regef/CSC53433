using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinBrush : TerrainBrush {


    public override void draw(int x, int z) {
        float baseDrawSpeed = 0.1f;
        float scale = 0.05f;   // Lower scale for smoother noise
        int octaves = 3;       // Number of Perlin noise layers
        float persistence = 0.5f;  // Amplitude reduction per octave
        float lacunarity = 2f;     // Frequency increase per octave
        float baseHeight = Mathf.Max(radius, 25);  // Base height

        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {
                float distance = Mathf.Sqrt(xi * xi + zi * zi);
                if (distance > radius) continue;  // Skip points outside the radius

                // Compute falloff and inverted falloff for draw speed
                float normalizedDistance = Mathf.Clamp01(distance / radius);
                float falloff = Mathf.Pow(1f - normalizedDistance, 2);  // Quadratic falloff
                float drawSpeed = baseDrawSpeed * normalizedDistance;   // Inverted draw speed

                // Generate multi-octave Perlin noise
                float noiseValue = 0f;
                float amplitude = 1f;
                float frequency = 1f;
                float maxAmplitude = 0f;

                for (int octave = 0; octave < octaves; octave++) {
                    float sampleX = (x + xi) * frequency * scale;
                    float sampleZ = (z + zi) * frequency * scale;

                    noiseValue += Mathf.PerlinNoise(sampleX, sampleZ) * amplitude;
                    maxAmplitude += amplitude;

                    amplitude *= persistence;  // Reduce amplitude for next octave
                    frequency *= lacunarity;  // Increase frequency for next octave
                }

                // Normalize and adjust height
                noiseValue /= maxAmplitude;
                float currentHeight = terrain.get(x + xi, z + zi);
                float adjustedHeight = currentHeight + drawSpeed * baseHeight * noiseValue * falloff;

                if (adjustedHeight >= currentHeight) {
                    terrain.set(x + xi, z + zi, adjustedHeight);
                }
            }
        }
    }



}
