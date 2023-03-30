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
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using Timer = System.Timers.Timer;

public class Main : MonoBehaviour
{
    public GameObject coalPlant;
    public GameObject solarPlant;
    public GameObject windPlant;
    GameObject gameOver;
    GameObject menu;
    GameObject tearMenu;
    public GameObject instanceParent = null;
    PowerPlant takenPlant = null;

    public float climateHealth = 1.0f;
    public int moneyAmount;

    TextMeshProUGUI moneyText;
    Image climateBar;

    float LerpTarget = 1.0f;
    float LerpStart = 1.0f;
    Timer timer;

    float LerpTimer;

    List<PowerPlant> powerPlants = new List<PowerPlant>();
    public List<GameObject> placedTrees = new List<GameObject>();
    public List<GameObject> trees = new List<GameObject>();

    PowerPlant coal = new PowerPlant(
        100,
        150,
        40,
        0.5f,
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
        tearMenu = GameObject.Find("TearDown");
        tearMenu.SetActive(false);
        menu = GameObject.Find("PurchaseManu");
        gameOver = GameObject.Find("GameOver");
        gameOver.SetActive(false);
        moneyText = GameObject.Find("MoneyText").GetComponent<TextMeshProUGUI>();
        climateBar = GameObject.Find("HPBar").GetComponent<Image>();
        moneyAmount = 200;
        timer = new System.Timers.Timer();
        timer.Elapsed += new ElapsedEventHandler(updateValues);
        timer.Interval = 5000;
        timer.Start();
        //GameObject.Find("BuildCoal").GetComponent<Button>().onClick.AddListener();
        GameObject.Find("WindInfo").GetComponent<TextMeshProUGUI>().SetText($"Build: ${wind.productionCost} Earnings: ${wind.runningEarnings/5}/sec Climate impact: -{(1 - wind.runningClimateImpact) * 100}%");
        GameObject.Find("SolarInfo").GetComponent<TextMeshProUGUI>().SetText($"Build: ${solar.productionCost} Earnings: ${solar.runningEarnings/5}/sec Climate impact: -{(1 - solar.runningClimateImpact) * 100}%");
        GameObject.Find("CoalInfo").GetComponent<TextMeshProUGUI>().SetText($"Build: ${coal.productionCost} Earnings: ${coal.runningEarnings/5}/sec Climate impact: -{(1- coal.runningClimateImpact)*100}%");
        menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && instanceParent==null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag == "Clickable")
                {
                    instanceParent = hit.transform.gameObject;
                    if (powerPlants.Count > 0)
                    {
                        for (int i = 0; i < powerPlants.Count; i++)
                        {
                            if (powerPlants[i].parent == hit.transform.gameObject)
                            {
                                takenPlant = powerPlants[i];
                            }
                        }
                    }
                    if (takenPlant == null)
                    {
                        menu.SetActive(true);
                    } 
                    else
                    {
                        tearMenu.SetActive(true);
                        GameObject.Find("TearMenu").GetComponent<TextMeshProUGUI>().SetText($"{takenPlant.type.ToString()} Powerplant");
                        GameObject.Find("TearInfo").GetComponent<TextMeshProUGUI>().SetText($"Teardown cost: ${takenPlant.removalCost}");
                    }
                    //ConstructPowerPlant(hit.transform.position, coal, hit.transform.gameObject);

                } else if (hit.transform.gameObject.tag == "Grass")
                {
                    MakeTree(hit.transform.gameObject);
                }
            }
        }
        UpdateUi();
    }

    void MakeTree(GameObject parent)
    {
        if (moneyAmount < 10)
        {
            return;
        }
        moneyAmount -= 10;
        var randomPos = new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, Random.Range(-0.5f, 0.5f));
        placedTrees.Add(Instantiate(trees[(int)Random.Range(0,trees.Count)], parent.transform.position+ randomPos, Quaternion.identity));
    }

    public void TearPowerPlant()
    {
        for (int i = 0; i < powerPlants.Count; i++)
        {
            if (powerPlants[i] == takenPlant)
            {
                if (moneyAmount < powerPlants[i].removalCost)
                {
                    return;
                }
                moneyAmount -= powerPlants[i].removalCost;
                Destroy(powerPlants[i].instance);
                powerPlants.RemoveAt(i);
                takenPlant = null;
                tearMenu.SetActive(false);
                instanceParent = null;
            }
        }
    }

    void ConstructPowerPlant(PowerPlant plant, GameObject parent)
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
            spawn = Instantiate(solarPlant, parent.transform.position, Quaternion.identity);
            plant.instance = spawn;
        } 
        else if (plant.type == Types.Coal)
        {
            spawn = Instantiate(coalPlant, parent.transform.position, Quaternion.identity);
            plant.instance = spawn;
        }
        else if (plant.type == Types.Wind)
        {
            spawn = Instantiate(windPlant, parent.transform.position, Quaternion.identity);
            plant.instance = spawn;
        }

        powerPlants.Add(plant);
        Debug.Log("Created");
        
    }

    public void BuildCoal()
    {
        ConstructPowerPlant(coal, instanceParent);
        instanceParent = null;
        menu.SetActive(false);
    }

    public void BuildSolar()
    {
        ConstructPowerPlant(solar, instanceParent);
        instanceParent = null;
        menu.SetActive(false);
    }

    public void BuildWind()
    {
        ConstructPowerPlant(wind, instanceParent);
        instanceParent = null;
        menu.SetActive(false);
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

        if (climateHealth <= 0.02f)
            gameOver.SetActive(true);
    }

    public void HideUi()
    {
        takenPlant = null;
        tearMenu.SetActive(false);
        menu.SetActive(false);
        instanceParent = null;
    }

    void updateValues(object source, ElapsedEventArgs e)
    {
        if (powerPlants.Count > 0 && climateHealth > 0.02f)
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
            FinalHealth *= Mathf.Pow(1.01f, placedTrees.Count);
            LerpTarget = climateHealth * FinalHealth;
            //if (LerpTarget > 1)
            //    LerpTarget = 1;
        }
        Debug.Log(placedTrees.Count > 0);
        if(placedTrees.Count > 0)
        {
            if (climateHealth != 1.0f)
            {
                int maxDelete = Mathf.RoundToInt(placedTrees.Count * (1 - climateHealth));
                //int deleteTreeNumber = Random.Range(0, maxDelete);
                Debug.Log($"Killing {maxDelete} trees");
                for (int i = 0; i < maxDelete; i++)
                {
                    Debug.Log($"Killing {i}");
                    int o = Random.Range(0, placedTrees.Count);
                    Destroy(placedTrees[o].transform.gameObject);
                    placedTrees.RemoveAt(0);
                }
            } 
            else
            {
                int maxDelete = Mathf.RoundToInt(placedTrees.Count * 0.1f);
                //int deleteTreeNumber = Random.Range(0, maxDelete);
                Debug.Log($"Killing {maxDelete} trees");
                for (int i = 0; i < maxDelete; i++)
                {
                    Debug.Log($"Killing {i}");

                    //int o = Random.Range(0, placedTrees.Count);
                    Debug.Log($"Random {1}");
                    Destroy(placedTrees[0].transform.gameObject);
                    placedTrees.RemoveAt(0);
                }
            }
            Debug.Log("Done");
        }

    }

    [ContextMenu("Make World Die")]
    public void DoCheat()
    {
        Debug.Log("Run Cheat");
        LerpTarget = 0.01f;
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

