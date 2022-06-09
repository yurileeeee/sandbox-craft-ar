using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpPopUp : MonoBehaviour
{
    [SerializeField] GameObject[] pages;
    [SerializeField] GameObject[] pageCircles;
    [SerializeField] Button leftBtn;
    [SerializeField] Button rightBtn;

    public int activeIndex = 0;

    private void Awake()
    {
        ShowPage(0);
    }

    private void Update()
    {
        if (activeIndex == 0)
        {
            leftBtn.interactable = false;
        }
        else if (activeIndex == pages.Length - 1)
        {
            rightBtn.interactable = false;
        }
        else
        {
            leftBtn.interactable = true;
            rightBtn.interactable = true;
        }
    }

    public void ShowPage(int index)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            pageCircles[i].GetComponentsInChildren<Image>(true)[0].gameObject.SetActive(false);
            pageCircles[i].GetComponentsInChildren<Image>(true)[1].gameObject.SetActive(true);
        }

        pages[index].SetActive(true);
        pageCircles[index].GetComponentsInChildren<Image>(true)[0].gameObject.SetActive(true);
        pageCircles[index].GetComponentsInChildren<Image>(true)[1].gameObject.SetActive(false);
    }

    public void LeftClick()
    {
        if (activeIndex != 0)
        {
            activeIndex--;
        }
        ShowPage(activeIndex);
    }

    public void RightClick()
    {
        if (activeIndex != pages.Length - 1)
        {
            activeIndex++;
        }
        ShowPage(activeIndex);
    }

    public void Reset()
    {
        activeIndex = 0;
        ShowPage(activeIndex);

        leftBtn.interactable = false;
        rightBtn.interactable = true;
    }
}
