using HexR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainCoin : MonoBehaviour
{
    public ParticleSystem WaterParticle1,WaterParticle2, CoinParticle;
    public GameObject Coin1, Coin2, Coin3, Coin4, Coin5;
    private Vector3 startPos1, startPos2, startPos3, startPos4, startPos5;
    private float ParticleRate = 0;
    // Start is called before the first frame update
    void Start()
    {
        startPos1 = Coin1.transform.position;   // store initial position
        startPos2 = Coin2.transform.position;   // store initial position
        startPos3 = Coin3.transform.position;   // store initial position
        startPos4 = Coin4.transform.position;   // store initial position
        startPos5 = Coin5.transform.position;   // store initial position
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
            ParticleRate += 5;
            MoreCoin();
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
    public void ResetFountain()
    {
        WaterParticle1.gameObject.SetActive(false);
        WaterParticle2.gameObject.SetActive(false);
        WaterParticle1.Stop();
        WaterParticle2.Stop();

        CoinParticle.gameObject.SetActive(false);
        CoinParticle.Stop();

        Coin1.transform.position = startPos1;   // store initial position
        Coin2.transform.position = startPos2;   // store initial position
        Coin3.transform.position = startPos3;   // store initial position
        Coin4.transform.position = startPos4;   // store initial position
        Coin5.transform.position = startPos5;   // store initial position

        ParticleRate = 0;
    }
    public void MoreCoin()
    {
        var emission = CoinParticle.emission;  // Get the emission module
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(ParticleRate);
    }
}
