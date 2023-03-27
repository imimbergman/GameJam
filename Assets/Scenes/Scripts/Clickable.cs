using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    Vector3 lastpos;
    public GameObject testBuilding;
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
                if (hit.transform.gameObject.tag == "Clickable")
                {
                    lastpos = hit.transform.position;
                    GameObject spawnBuilding = Instantiate(testBuilding, lastpos, Quaternion.identity);
                    //spawnBuilding.transform.localScale = new Vector3(16, 16, 16);
                    //uncomment if buildings are small
                    this.enabled = false;
                }
            }
        }
        
    }
}
