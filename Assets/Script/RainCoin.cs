using HexR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainCoin : MonoBehaviour
{
    public ParticleSystem WaterParticle1,WaterParticle2, CoinParticle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.name.Contains("coin"))
        {
            WaterParticle1.gameObject.SetActive(true);
            WaterParticle2.gameObject.SetActive(true);
            WaterParticle1.Play();
            WaterParticle2.Play();
        }
        if(WaterParticle1.gameObject.activeInHierarchy && other.gameObject.TryGetComponent(out HapticFingerTrigger hapticFingerTrigger))
        {
            CoinParticle.gameObject.SetActive(true);
            CoinParticle.Play();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
    }
}
