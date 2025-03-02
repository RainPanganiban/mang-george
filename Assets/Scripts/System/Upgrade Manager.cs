using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Needed to load the next level

public class UpgradeManager : MonoBehaviour
{
    public GameObject upgradePanel; // UI panel for upgrades
    public Button[] upgradeButtons; // Upgrade choices (3 buttons)

    // List of all possible upgrades
    private List<string> possibleUpgrades = new List<string> { "Health", "Speed", "Dash", "Damage" };

    public void ShowUpgradeOptions()
    {
        upgradePanel.SetActive(true); // Show the panel

        // Select 3 random upgrades
        List<string> selectedUpgrades = new List<string>();
        while (selectedUpgrades.Count < 3)
        {
            string randomUpgrade = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
            if (!selectedUpgrades.Contains(randomUpgrade))
            {
                selectedUpgrades.Add(randomUpgrade);
            }
        }

        // Assign upgrades to buttons
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            string upgradeType = selectedUpgrades[i];
            upgradeButtons[i].GetComponentInChildren<Text>().text = upgradeType; // Display upgrade name
            upgradeButtons[i].onClick.RemoveAllListeners(); // Clear old listeners
            upgradeButtons[i].onClick.AddListener(() => SelectUpgrade(upgradeType)); // Add new listener
        }
    }

    public void SelectUpgrade(string upgradeType)
    {

        Debug.Log("Upgrade Selected: " + upgradeType);

        PlayerStats.Instance.ApplyUpgrade(upgradeType); // Apply upgrade
        upgradePanel.SetActive(false); // Hide panel

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);

    }

}
