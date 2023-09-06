using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    private ParticleSystem particle;
    private float currentTime;
    private float EffectTime;
    private float particleSize;
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        particle.Play();
        currentTime = 0f;
        EffectTime = 0.2f;
        particleSize = 3f;
        color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime <= EffectTime)
        {
            currentTime += Time.deltaTime;

            var main = particle.main;
            color.a -= Time.deltaTime * 3f;
            main.startSize = particleSize - Time.deltaTime * 6f;
            main.startColor = new ParticleSystem.MinMaxGradient(color);
        }
        else
            Destroy(this.gameObject);
    }
}
