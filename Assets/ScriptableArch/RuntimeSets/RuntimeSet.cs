using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : ScriptableObject
{
    public List<T> Items = new List<T>();

    public int ItemCount { get { return Items.Count; } }

    public void AddItem(T item) {
        if (!Items.Contains(item)) {
            Items.Add(item);
        }
    }

    public void RemoveItem(T item) {
        if (Items.Contains(item)) {
            Items.Remove(item);
        }
    }

    public void Clear() {
        Items.Clear();
    }
    
}
