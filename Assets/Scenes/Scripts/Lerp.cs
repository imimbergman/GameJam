using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class Lerp : MonoBehaviour
{
    public float LerpAmount = 0.5f;
    float lerpValue = 0;
    public float lerpSpeed;
    public float vibrationSpeed = 0.1f;
    private ParticleSystem ps;
    Vector3 CurrentPosition;
    Vector3 TargetPosition;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        CurrentPosition = transform.position;
        TargetPosition = CurrentPosition + new Vector3(0, LerpAmount, 0);
    }

    // Update is called once per frame
    void Update()
    {
        lerpValue += lerpSpeed * Time.deltaTime;
        transform.position = Vector3.Lerp(CurrentPosition, TargetPosition, lerpValue);
        TargetPosition = CurrentPosition + new Vector3(Random.Range(-vibrationSpeed, vibrationSpeed), LerpAmount, Random.Range(-vibrationSpeed, vibrationSpeed));
        if (lerpValue >= 1)
        {
            transform.position = TargetPosition = CurrentPosition + new Vector3(0, LerpAmount, 0);
            ps.Stop();
            this.enabled = false;
        }
    }
}
