using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }

    public bool isTest; // PC인지
    [SerializeField] GameObject[] pcObjects;
    [SerializeField] GameObject[] arObjects;

    [Header("AR")]
    [SerializeField] ARPlaneManager arPlane;
    [SerializeField] ARSessionOrigin arOrigin;
    [SerializeField] ARRaycastManager arRaycaster;
    List<ARRaycastHit> originHits = new List<ARRaycastHit>();

    [Header("Panel")]
    [SerializeField] RectTransform gamePanel;
    [SerializeField] RectTransform menuPanel;
    [SerializeField] GameObject menuBtn;

    bool onMenu = false;
    public bool isBuildMode = true;

    private void Awake()
    {
        Inst = this;
#if !UNITY_EDITOR
        isTest = false;
#endif
        Array.ForEach(pcObjects, x => x.SetActive(isTest)); 
        Array.ForEach(arObjects, x => x.SetActive(!isTest));

        ShowPanel("GamePanel");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuClick();
        }
    }

    public void PlaceOrigin()
    {
        arRaycaster.Raycast(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), originHits);

        if (originHits.Count > 0)
        {
            Pose hitpose = originHits[0].pose;

            arOrigin.MakeContentAppearAt(arOrigin.transform, hitpose.position + Vector3.up * 1.5f, hitpose.rotation);
        }
    }

    public void ShowPanel(string panelName)
    {
        if (panelName == gamePanel.name)
        {
            gamePanel.anchoredPosition = new Vector2(0, 0);
            menuPanel.anchoredPosition = new Vector2(0, 500);
        }
        else if (panelName == menuPanel.name)
        {
            gamePanel.anchoredPosition = new Vector2(0, -600);
            menuPanel.anchoredPosition = new Vector2(0, 0);
        }
    }

    public void MenuClick()
    {
        onMenu = !onMenu;
        ShowPanel(onMenu ? "MenuPanel" : "GamePanel");
        arPlane.enabled = onMenu;
        menuBtn.SetActive(!onMenu);
    }

    public void ExitClick() => Application.Quit();

    public void NewGameClick()
    {
        //Array.ForEach(GameObject.FindGameObjectsWithTag("Block"), x => Destroy(x));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BuildModeToggle(bool isBuildMode)
    {
        this.isBuildMode = isBuildMode;

        //// true 이면 작아서 건축 가능, false 이면 커서 건축 불가능
        //if (isBuildMode)
        //{
        //    arOrigin.transform.localScale = Vector3.one * 64;
        //}
        //else
        //{
        //    arOrigin.transform.localScale = Vector3.one;
        //}
    }
}
