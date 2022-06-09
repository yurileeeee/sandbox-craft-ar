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
    [SerializeField] Material blockMaterial;

    Dictionary<int, Material> materialCodeTable = new Dictionary<int, Material>();
    int activeSlotCode = 0;


    void Start()
    {
        activeSlot.transform.position = slots[activeSlotCode].transform.position;
    }

    public Material GetMaterial()
    {
        if (materialCodeTable.ContainsKey(activeSlotCode))
        {
            return materialCodeTable[activeSlotCode];
        }
        else
        {
            Material material = new Material(blockMaterial);
            material.mainTexture = items[activeSlotCode].texture;
            materialCodeTable.Add(activeSlotCode, material);
            return material;
        }
    }

    public void SlockClick(int code)
    {
        activeSlotCode = code;
        activeSlot.transform.position = slots[activeSlotCode].transform.position;
        GameManager.PlaySound("ui_click");
    }
}
