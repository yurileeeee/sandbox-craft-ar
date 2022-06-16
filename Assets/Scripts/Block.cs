using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour
{
    public BlockSide[] side;
    public GameObject insideCube;
    public GameObject previewCube;
    public bool isSelected = false;

    private void Update()
    {
        if (isSelected)
        {
            SelectBlock();
        }
        else
        {
            DeselectBlock();
        }
    }

    public void SelectBlock()
    {
        previewCube.SetActive(true);
    }

    public void DeselectBlock()
    {
        previewCube.SetActive(false);
    }
}
