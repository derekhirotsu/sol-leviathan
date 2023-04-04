using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour {
    [SerializeField]
    protected TMP_Text itemLabel;

    [SerializeField]
    protected TMP_Text itemDescription;

    [SerializeField]
    protected Button exit;

    Button button;

    Item currentItem;

    void Awake() {
        button = GetComponent<Button>();
        // var nav = button.navigation;
        // nav.selectOnUp = exit;
        // Debug.Log(nav);
        // nav.mode = Navigation.Mode.Explicit;
        // button.navigation = nav;
        // button.interactable = true;
    }

    // void OnEnable() {
    //     button.interactable = true;
    // }

    public void SetShopItem(Item item) {
        currentItem = item;
        itemLabel.text = item.ItemName;
    }

    public void SelectItem() {
        currentItem.Purchase();
        // button.interactable = false;
    }

    public void UpdateItemDescriptionText() {
        itemDescription.text = currentItem.ItemDescription;
    }

    public void ClearItemDescriptionText() {
        itemDescription.text = "";
    }
}
