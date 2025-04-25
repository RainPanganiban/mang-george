using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public GameObject upgradePanel;
    public Button[] upgradeButtons;
    public GameObject transition;

    private List<string> possibleUpgrades = new List<string> { "Health", "Speed", "Dash", "Damage", "TripleShot" };

    public void ShowUpgradeOptions()
    {
        upgradePanel.SetActive(true);

        List<string> selectedUpgrades = new List<string>();
        while (selectedUpgrades.Count < 3)
        {
            string randomUpgrade = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
            if (!selectedUpgrades.Contains(randomUpgrade))
            {
                selectedUpgrades.Add(randomUpgrade);
            }
        }

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            string upgradeType = selectedUpgrades[i];

            TextMeshProUGUI buttonText = upgradeButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = upgradeType;
            }

            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() => SelectUpgrade(upgradeType));
        }
    }

    public void SelectUpgrade(string upgradeType)
    {
        PlayerStats.Instance.ApplyUpgrade(upgradeType);
        upgradePanel.SetActive(false);

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }
}
