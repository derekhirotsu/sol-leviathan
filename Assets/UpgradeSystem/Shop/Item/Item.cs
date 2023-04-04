using UnityEngine;

public abstract class Item : ScriptableObject {
    [SerializeField]
    protected ItemType itemType;
    public ItemType ItemType { get { return itemType; } }

    [SerializeField]
    protected string itemName = "";
    public string ItemName { get { return itemName; } }

    [SerializeField]
    [Multiline]
    protected string itemDescription = "";
    public string ItemDescription { get { return itemDescription; } }

    public bool IsAvailable { get { return CheckAvailabilityRequirements(); } }
    
    public abstract bool CheckAvailabilityRequirements();

    public abstract void Purchase();

    public override string ToString() {
        return $"Item: {itemName} - Type: {itemType} - Available: {IsAvailable}";
    }
}
