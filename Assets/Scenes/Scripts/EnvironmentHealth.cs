using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentHealth : MonoBehaviour
{
    public float currentHealth = 1f;
    private Image Healthbar;
    // Start is called before the first frame update
    void Start()
    {
        Healthbar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Healthbar.fillAmount = currentHealth;
    }
}
