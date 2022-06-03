using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item 
{
    public int code;
    public Sprite icon;
    public Texture2D texture;
}


public class Inventory : MonoBehaviour
{
    [SerializeField] Item[] items;
    [SerializeField] RectTransform[] slots;
    [SerializeField] RectTransform activeSlot;

    Dictionary<int, Material> materialCodeTable;

    int activeSlotCode = 0;

    public bool GetActiveTexture(out Texture2D texture)
    {
        texture = items[activeSlotCode].texture;

        return false;
    }


    void Start()
    {
        materialCodeTable = new Dictionary<int, Material>();
        activeSlot.transform.position = slots[activeSlotCode].transform.position;
    }

    public void SlockClick(int code)
    {
        activeSlotCode = code;
        activeSlot.transform.position = slots[activeSlotCode].transform.position;
    }
}
