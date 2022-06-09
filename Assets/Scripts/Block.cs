using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] Material selectMaterial;


    public void BlockClick()
    {
        this.GetComponent<Renderer>().material = selectMaterial;
    }
}
