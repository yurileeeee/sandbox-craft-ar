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
    [SerializeField] RectTransform joyPanel;
    [SerializeField] GameObject menuBtn;

    [Header("ETC")]
    public bool isBuildMode = true;
    public GameObject plane;
    [SerializeField] float boostSpeed;

    bool onMenu = false;
    bool isBoost = false;

    private void Awake()
    {
        Inst = this;
#if !UNITY_EDITOR
        isTest = false;
        plane.SetActive(false);
#endif
        Array.ForEach(pcObjects, x => x.SetActive(isTest)); 
        Array.ForEach(arObjects, x => x.SetActive(!isTest));
        
        ShowPanel("GamePanel");
        ArPlaneEnable(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuClick();
        }

        if (isBoost)
        {
            Vector3 direction = Camera.main.transform.forward;
            arOrigin.transform.Translate(direction * boostSpeed * Time.deltaTime, Space.World);
        }
    }

    public void PlaceOrigin()
    {
        arRaycaster.Raycast(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), originHits, TrackableType.Planes);

        if (originHits.Count > 0)
        {
            Pose hitpose = originHits[0].pose;

            arOrigin.MakeContentAppearAt(arOrigin.transform, hitpose.position + Vector3.up * 1.5f, hitpose.rotation);
            plane.SetActive(true);
        }
    }

    public void ShowPanel(string panelName)
    {
        // gamePanel.DOAnchorPos(new Vector2(0, -600), 0.4f);
        gamePanel.anchoredPosition = new Vector2(0, -600);
        menuPanel.anchoredPosition = new Vector2(0, 600);
        joyPanel.anchoredPosition = new Vector2(0, -600);

        if (panelName == gamePanel.name)
        {
            gamePanel.anchoredPosition = new Vector2(0, 0);
        }
        else if (panelName == menuPanel.name)
        {
            menuPanel.anchoredPosition = new Vector2(0, 0);
        }
        else if (panelName == joyPanel.name)
        {
            joyPanel.anchoredPosition = new Vector2(0, 0);
        }
    }

    public void MenuClick()
    {
        onMenu = !onMenu;
        ShowPanel(onMenu ? "MenuPanel" : "GamePanel");
        ArPlaneEnable(onMenu);
        menuBtn.SetActive(!onMenu);

        if (!onMenu && !isBuildMode)
        {
            ShowPanel("JoyPanel");
        }
    }

    void ArPlaneEnable(bool b)
    {
        arPlane.enabled = b;
        foreach (ARPlane plane in arPlane.trackables)
        {
            plane.gameObject.SetActive(b);
        }
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

        // true 이면 작아서 건축 가능, false 이면 커서 건축 불가능
        if (isBuildMode)
        {
            arOrigin.transform.localScale = Vector3.one * 48;
            arOrigin.transform.position = Vector3.zero;
        }
        else
        {
            arOrigin.transform.localScale = Vector3.one;
        }
    }

    public void BoostClicking(bool isBoost)
    {
        this.isBoost = isBoost;
    }
}
