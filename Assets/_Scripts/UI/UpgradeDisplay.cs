using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeDisplay : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] Image iconImage;

    [HideInInspector] public Upgrade Upgrade;
    private UpgradeMenu menu;

    public void Display(Upgrade upgrade, UpgradeMenu menu)
    {
        nameText.text = upgrade.Name;
        descriptionText.text = upgrade.Description;
        costText.text = upgrade.Cost.ToString() + " $";
        iconImage.sprite = upgrade.Icon;

        Upgrade = upgrade;
        this.menu = menu;
    }

    public void Click()
    {
        menu.PurchaseUpgrade(this);
    }
}
