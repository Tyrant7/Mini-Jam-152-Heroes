using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeMenu : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] string gameSceneName;

    [Header("Upgrade Menu")]
    [SerializeField] Transform anchor;
    [SerializeField] GameObject upgradePrefab;
    [SerializeField] TextMeshProUGUI moneyText;

    private void Start()
    {
        Upgrade[] upgrades = UpgradeManager.Instance.GetUpgrades();
        foreach (Upgrade upgrade in upgrades)
        {
            GameObject g = Instantiate(upgradePrefab, anchor);
            UpgradeDisplay d = g.GetComponent<UpgradeDisplay>();
            d.Display(upgrade, this);
        }
        moneyText.text = GameManager.Instance.TotalMoney.ToString() + " $";
    }

    public void PurchaseUpgrade(UpgradeDisplay clickedDisplay)
    {
        if (UpgradeManager.Instance.PurchaseUpgrade(clickedDisplay.Upgrade))
        {
            Destroy(clickedDisplay.gameObject);
        }
        moneyText.text = GameManager.Instance.TotalMoney.ToString() + " $";
    }

    public void StartNextDay()
    {
        SceneLoader.Instance.LoadScene(gameSceneName);
    }
}
