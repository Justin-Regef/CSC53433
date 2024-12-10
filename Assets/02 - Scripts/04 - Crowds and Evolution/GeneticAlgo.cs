using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneticAlgo : MonoBehaviour
{

    [Header("Genetic Algorithm parameters")]
    public int popSize = 10;
    public GameObject animalPrefab;

    [Header("Dynamic elements")]
    public float vegetationGrowthRate = 0.0f;
    public float currentGrowth;

    private List<GameObject> animals;
    private List<GameObject> decomposing_animals;
    private List<GameObject> zombies;
    protected Terrain terrain;
    protected CustomTerrain customTerrain;
    protected float width;
    protected float height;

    void Start()
    {
        // Retrieve terrain.
        terrain = Terrain.activeTerrain;
        customTerrain = GetComponent<CustomTerrain>();
        width = terrain.terrainData.size.x;
        height = terrain.terrainData.size.z;

        // Initialize terrain growth.
        currentGrowth = 0.0f;

        // Initialize animals array.
        animals = new List<GameObject>();
        decomposing_animals = new List<GameObject>();
        zombies = new List<GameObject>();
        for (int i = 0; i < popSize; i++)
        {
            GameObject animal = makeAnimal();
            animals.Add(animal);
        }
    }

    void Update()
    {
        // Keeps animal to a minimum.
        while ((animals.Count + decomposing_animals.Count) < popSize / 2)
        {
            animals.Add(makeAnimal());
        }
        customTerrain.debug.text = "N animals: " + animals.Count.ToString() + "    N decomposing: " + decomposing_animals.Count.ToString();

        // Update grass elements/food resources.
        updateResources();
    }

    /// <summary>
    /// Method to place grass or other resource in the terrain.
    /// </summary>
    public void updateResources()
    {
        Vector2 detail_sz = customTerrain.detailSize();
        int[,] details = customTerrain.getDetails();
        currentGrowth += vegetationGrowthRate;
        while (currentGrowth > 1.0f)
        {
            int x = (int)(UnityEngine.Random.value * detail_sz.x);
            int y = (int)(UnityEngine.Random.value * detail_sz.y);
            details[y, x] = 1;
            currentGrowth -= 1.0f;
        }
        for (int i = 0; i < decomposing_animals.Count; i++) {
            float dice_roll = UnityEngine.Random.value;
             if (dice_roll < 0.001f){
                Animal animal = decomposing_animals[i].GetComponent<Animal>();
                decomposing_animals.Remove(animal.transform.gameObject);
                zombies.Add(animal.transform.gameObject);
                animal.isZombie = true;
                animal.isDead = false;
            }
            else if (dice_roll < 0.02f){
                Animal animal = decomposing_animals[i].GetComponent<Animal>();
                Transform goal = decomposing_animals[i].GetComponent<QuadrupedProceduralMotion>().goal;
                decomposing_animals.Remove(animal.transform.gameObject);
                int dx = (int)(goal.position.x / animal.terrainSize.x * detail_sz.x);
                int dy = (int)(goal.position.z / animal.terrainSize.y * detail_sz.y);
                details[dy, dx] = 1;
                Destroy(animal.transform.gameObject);
            }
        }
        customTerrain.saveDetails();
    }

    /// <summary>
    /// Method to instantiate an animal prefab. It must contain the animal.cs class attached.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public GameObject makeAnimal(Vector3 position)
    {
        GameObject animal = Instantiate(animalPrefab, transform);
        animal.GetComponent<Animal>().Setup(customTerrain, this);
        animal.transform.position = position;
        animal.transform.Rotate(0.0f, UnityEngine.Random.value * 360.0f, 0.0f);
        return animal;
    }

    /// <summary>
    /// If makeAnimal() is called without position, we randomize it on the terrain.
    /// </summary>
    /// <returns></returns>
    public GameObject makeAnimal()
    {
        Vector3 scale = terrain.terrainData.heightmapScale;
        float x = UnityEngine.Random.value * width;
        float z = UnityEngine.Random.value * height;
        float y = customTerrain.getInterp(x / scale.x, z / scale.z);
        return makeAnimal(new Vector3(x, y, z));
    }

    /// <summary>
    /// Method to add an animal inherited from anothed. It spawns where the parent was.
    /// </summary>
    /// <param name="parent"></param>
    public void addOffspring(Animal parent)
    {
        GameObject animal = makeAnimal(parent.transform.position);
        animal.GetComponent<Animal>().InheritBrain(parent.GetBrain(), true);
        animals.Add(animal);
    }

    /// <summary>
    /// Remove instance of an animal.
    /// </summary>
    /// <param name="animal"></param>
    public void removeAnimal(Animal animal)
    {
        animal.isDead = true;
        animals.Remove(animal.transform.gameObject);
        decomposing_animals.Add(animal.transform.gameObject);
        //Destroy(animal.transform.gameObject);
    }

}
