using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyScript : MonoBehaviour
{
    public int startMoney = 9999;
    private TextMeshProUGUI _displayText;
    // Start is called before the first frame update
    void Start()
    {
        _displayText = GetComponent<TextMeshProUGUI>();
        _displayText.SetText("" + startMoney);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
