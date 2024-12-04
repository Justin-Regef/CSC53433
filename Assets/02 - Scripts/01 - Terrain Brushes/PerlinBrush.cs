using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinBrush : TerrainBrush {

    public float baseHeight = 25f; // Base height
    public float scale = 0.1f; // Lower scale for smoother noise
    public int octaves = 3; // Number of Perlin noise layers
    public float persistence = 0.5f; // Amplitude reduction per octave
    public float lacunarity = 2f; // Frequency increase per octave

    public override void draw(int x, int z) {
        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++) {
                float distance = Mathf.Sqrt(Mathf.Pow(xi, 2) + Mathf.Pow(zi, 2));
                float noiseValue = 0f;
                float amplitude = 1f;
                float frequency = 1f;
                float maxAmplitude = 0f; // Normalize noise values

                for (int octave = 0; octave < octaves; octave++) {
                    float sampleX = (x + xi) * frequency * scale;
                    float sampleZ = (z + zi) * frequency * scale;

                    noiseValue += Mathf.PerlinNoise(sampleX, sampleZ) * amplitude;
                    maxAmplitude += amplitude;

                    amplitude *= persistence; // Reduce amplitude for next octave
                    frequency *= lacunarity; // Increase frequency for next octave
                }

                float falloff = distance > radius / 2
                    ? Mathf.Cos(((distance - (radius / 2)) / (radius / 2)) * Mathf.PI / 2)
                    : 1f;


                noiseValue /= maxAmplitude;
                float adjustedHeight = baseHeight * noiseValue * falloff;
                float currentHeight = terrain.get(x + xi, z + zi);

                if (adjustedHeight >= currentHeight) {
                    terrain.set(x + xi, z + zi, adjustedHeight);
                }


                
            }
        }
    }


}
