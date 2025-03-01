using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public GameObject upgradePanel;
    public Button[] upgradeButtons;

    private List<string> possibleUpgrades = new List<string> { "Health", "Speed", "Dash", "Damage" };

    public void ShowUpgradeOptions()
    {
        upgradePanel.SetActive(true);

        // Shuffle upgrades and assign three random ones
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
            upgradeButtons[i].GetComponentInChildren<Text>().text = upgradeType;
            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() => SelectUpgrade(upgradeType));
        }
    }

    public void SelectUpgrade(string upgradeType)
    {
        PlayerStats.Instance.ApplyUpgrade(upgradeType);
        upgradePanel.SetActive(false);
    }
}
