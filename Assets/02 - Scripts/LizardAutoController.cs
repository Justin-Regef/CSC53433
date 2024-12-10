using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LizardAutoController : MonoBehaviour {

    public float max_speed = 0.001f;
    protected Terrain terrain;
    protected CustomTerrain cterrain;
    protected float width, height;

    void Start() {
        terrain = Terrain.activeTerrain;
        cterrain = terrain.GetComponent<CustomTerrain>();
        width = terrain.terrainData.size.x;
        height = terrain.terrainData.size.z;
    }

    void Update() {
        Animal animal = GetComponent<Animal>();
        // this should update the lizard's goal object's position instead of directly moving the animal
        if (animal.isDead) {
            return;
            }
        Vector3 scale = terrain.terrainData.heightmapScale;
        Transform tfm = GetComponent<QuadrupedProceduralMotion>().goal;
        Vector3 v = tfm.rotation * Vector3.forward * max_speed;
        Vector3 loc = tfm.position + v;
        if (loc.x < 0)
            loc.x = 0;
        else if (loc.x > width)
            loc.x = width;
        if (loc.z < 0)
            loc.z = 0;
        else if (loc.z > height)
            loc.z = height;
        loc.y = cterrain.getInterp(loc.x/scale.x, loc.z/scale.z);
        tfm.position = loc;
    }
}
