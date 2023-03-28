using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using System.Xml.Linq;
using TMPro;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using Timer = System.Timers.Timer;

public class Clickable : MonoBehaviour
{
    public GameObject coalPlant;
    public GameObject solarPlant;
    public GameObject windPlant;

    public float climateHealth = 1.0f;
    public int moneyAmount;

    TextMeshProUGUI moneyText;
    Image climateBar;

    float LerpTarget = 1.0f;
    float LerpStart = 1.0f;
    Timer timer;

    float LerpTimer;

    List<PowerPlant> powerPlants = new List<PowerPlant>();

    PowerPlant coal = new PowerPlant(
        100,
        150,
        40,
        0.8f,
        0.8f,
        0.9f,
        Types.Coal
        );

    PowerPlant solar = new PowerPlant(
        400,
        200,
        10,
        0.75f,
        0.9f,
        0.99999f,
        Types.Solar
        );

    PowerPlant wind = new PowerPlant(
        700,
        250,
        30,
        0.55f,
        0.85f,
        0.99999f,
        Types.Wind
        );


    // Start is called before the first frame update
    void Start()
    {
        moneyText = GameObject.Find("MoneyText").GetComponent<TextMeshProUGUI>();
        climateBar = GameObject.Find("HPBar").GetComponent<Image>();
        moneyAmount = 1000;
        timer = new System.Timers.Timer();
        timer.Elapsed += new ElapsedEventHandler(updateValues);
        timer.Interval = 5000;
        timer.Start();
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
                } else if (hit.transform.gameObject.tag == "Grass")
                {
                    Debug.Log("Grass");
                }
            }
        }
        UpdateUi();
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

        if (moneyAmount < plant.productionCost)
        {
            return;
        }

        moneyAmount -= plant.productionCost;

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
            plant.instance = spawn;
        }

        powerPlants.Add(plant);
        Debug.Log("Created");
        
    }

    void UpdateUi()
    {

        //if (powerPlants.Count > 0)
        //{
        //    for (int i = 0; i < powerPlants.Count; i++)
        //    {
        //        climateHealth *= powerPlants[i].runningClimateImpact;
        //        Debug.Log(Time.deltaTime);
        //    }
        //}
        LerpTimer += Time.deltaTime;

        climateHealth = Mathf.Lerp(LerpStart, LerpTarget, LerpTimer / 5.0f);

        moneyText.SetText("" + moneyAmount.ToString());
        climateBar.fillAmount = climateHealth;
    }

    void updateValues(object source, ElapsedEventArgs e)
    {
        if (powerPlants.Count > 0)
        {
            float FinalHealth = 1;
            LerpStart = climateHealth;
            LerpTimer = 0;
            for (int i = 0; i < powerPlants.Count; i++)
            {
                moneyAmount += powerPlants[i].runningEarnings;
                //Lerp(climateHealth, climateHealth * powerPlants[i].runningClimateImpact, 0);
                FinalHealth *= powerPlants[i].runningClimateImpact;
            }
            LerpTarget = climateHealth * FinalHealth;
        }
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

