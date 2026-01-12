using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Health : MonoBehaviour
{
    public float sanity = 75f;
    public float maxSanity = 100f;

    public Image sanityBarImage;


    // Update is called once per frame
    void Update()
    {
        sanityBarImage.fillAmount = sanity / maxSanity;
    }
}