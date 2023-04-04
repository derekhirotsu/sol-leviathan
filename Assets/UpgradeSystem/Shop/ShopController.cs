using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopController : MonoBehaviour {
    [SerializeField]
    protected List<Item> inventory;

    [SerializeField]
    int numUnlocksDisplayed;

    [SerializeField]
    int numUpgradesDisplayed;

    [SerializeField]
    GameObject buttonGroup;

    ShopItemUI[] shopItems;

    [SerializeField]
    protected TMP_Text itemDescription;

    [SerializeField]
    ScriptableVariables.ScriptableVariableReference<int> currentWave;

    void Awake() {
        shopItems = GetComponentsInChildren<ShopItemUI>(true);
        itemDescription.text = "";
    }

    public List<Item> GetItemsOfType(ItemType typeMask) {
        return inventory.FindAll(item => (item.ItemType & typeMask) == typeMask);
    }

    protected List<Item> FilterAvailable(List<Item> items, ItemType itemType) {
        return items.FindAll(item => item.IsAvailable && (item.ItemType & itemType) == itemType);
    }

    public List<Item> SelectItemSet(ItemType itemType, int setSize) {
        List<Item> availableItems = FilterAvailable(inventory, itemType);

        if (availableItems.Count <= setSize) {
            return availableItems;
        }

        while (availableItems.Count > setSize) {
            int index = Random.Range(0, availableItems.Count);
            availableItems.Remove(availableItems[index]);
        }

        return availableItems;
    }

    public List<Item> SelectUnlocks() {
        return SelectItemSet(ItemType.Unlock, numUnlocksDisplayed);
    }

    public List<Item> SelectUpgrades() {
        return SelectItemSet(ItemType.Upgrade, numUpgradesDisplayed);
    }

    public void SetAvailableItems(List<Item> newItems) {
        Debug.Log("Settings items..." + shopItems.Length);
        for (int i = 0; i < shopItems.Length; i++) {
            if (i < newItems.Count) {
                shopItems[i].gameObject.SetActive(true);
                shopItems[i].SetShopItem(newItems[i]);
            } else {
                shopItems[i].gameObject.SetActive(false);
            }
        }
    }

    public void GetItems() {
        if (currentWave % 3 == 1)  {
            List<Item> itemSet = SelectUnlocks();
            SetAvailableItems(itemSet);
        } else {
            List<Item> itemSet = SelectUpgrades();
            SetAvailableItems(itemSet);
        }
    }
}
