using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOverlay : MonoBehaviour
{

    public GameObject tutorialOverlay;

    public void EnableOverlay()
    {
        tutorialOverlay.SetActive(true);
    }

    public void DisableOverlay()
    {
        tutorialOverlay.SetActive(false);
    }
}
