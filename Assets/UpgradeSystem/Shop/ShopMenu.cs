using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenu : Menu {
    [SerializeField]
    protected ShopController shopController;

    public override void ShowCanvas() {
        base.ShowCanvas();
        shopController.GetItems();

    }

    public override void HideCanvas() {
        base.HideCanvas();
    }
}
