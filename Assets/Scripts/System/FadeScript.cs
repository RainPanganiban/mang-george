using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScript : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] public GameObject Countdown;

    public void StartCountdown()
    {
        Countdown.SetActive(true);
    }

    
}
