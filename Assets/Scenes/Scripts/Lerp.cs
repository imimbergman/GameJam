using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Lerp : MonoBehaviour
{
    public float LerpAmount = 0.5f;
    float lerpValue = 0;
    public float lerpSpeed;
    Vector3 CurrentPosition;
    Vector3 TargetPosition;

    // Start is called before the first frame update
    void Start()
    {
        CurrentPosition = transform.position;
        TargetPosition = CurrentPosition + new Vector3(0, LerpAmount, 0);
    }

    // Update is called once per frame
    void Update()
    {
        lerpValue += lerpSpeed * Time.deltaTime;
        transform.position = Vector3.Lerp(CurrentPosition, TargetPosition, lerpValue);
        if (lerpValue >= 1)
            this.enabled = false;
    }
}
