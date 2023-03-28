using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    Vector3 lastpos;
    public GameObject coalPlant;
    public GameObject solarPlant;
    public GameObject windPlant;

    List<PowerPlant> powerPlants = new List<PowerPlant>();

    PowerPlant coal = new PowerPlant(
    100,
    150,
    2,
    0.5f,
    0.8f,
    0.99f,
    Types.Coal);

    PowerPlant solar = new PowerPlant(
        400,
        200,
        1,
        0.75f,
        0.9f,
        0.99999f,
        Types.Coal);
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag == "Clickable")
                {
                    ConstructPowerPlant(hit.transform.position, coal, hit.transform.gameObject);
                    //this.enabled = false;
                }
            }
        }
        
    }

    void ConstructPowerPlant(Vector3 position, PowerPlant plant, GameObject parent)
    {
        if (powerPlants.Count > 0)
        {
            for (int i = 0; i < powerPlants.Count; i++)
            {
                if (powerPlants[i].parent == parent)
                {
                    Debug.Log($"Checking {powerPlants[i]}");
                    return;
                }
            }
        }

        GameObject spawn;
        plant = plant.DeepCopy();
        plant.parent = parent;

        if (plant.type == Types.Solar)
        {
            spawn = Instantiate(solarPlant, position, Quaternion.identity);
            plant.instance = spawn;
        } 
        else if (plant.type == Types.Coal)
        {
            spawn = Instantiate(coalPlant, position, Quaternion.identity);
            plant.instance = spawn;
        }
        else if (plant.type == Types.Wind)
        {
            spawn = Instantiate(windPlant, position, Quaternion.identity);
            spawn.SetActive(true);
            plant.instance = spawn;
        }

        powerPlants.Add(plant);
    }
}

public class PowerPlant {
    public PowerPlant(
        int productionCost, 
        int removalCost, 
        int runningEarnings,
        float productionClimateImpact,
        float removalClimateImpact,
        float runningClimateImpact,
        Types type
        )
    {
        this.productionCost = productionCost;
        this.removalCost = removalCost;
        this.runningEarnings = runningEarnings;
        this.productionClimateImpact = productionClimateImpact;
        this.removalClimateImpact = removalClimateImpact;
        this.runningClimateImpact = runningClimateImpact;
        this.type = type;
}
    public PowerPlant DeepCopy()
    {
        PowerPlant other = (PowerPlant)MemberwiseClone();
        other.productionCost = productionCost;
        other.removalCost = removalCost;
        other.runningEarnings = runningEarnings;
        other.productionClimateImpact = productionClimateImpact;
        other.removalClimateImpact = removalClimateImpact;
        other.runningClimateImpact = runningClimateImpact;
        other.type = type;
        return other;
    }
    public GameObject instance;
    public GameObject parent = null;

    public int productionCost;
    public int removalCost;
    public int runningEarnings;
     
    public float productionClimateImpact;
    public float removalClimateImpact;
    public float runningClimateImpact;

    public Types type;
}

public enum Types
{
    Solar,
    Wind,
    Coal
}