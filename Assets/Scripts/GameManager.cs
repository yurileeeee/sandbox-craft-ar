using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip audioClip;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }

    public bool isPC; // PC인지
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
    [SerializeField] RectTransform selectPanel;
    [SerializeField] GameObject menuBtn;
    [SerializeField] GameObject helpBtn;

    [Header("PopUp")]
    [SerializeField] GameObject uiBlock;
    [SerializeField] Transform exitPopUp;
    [SerializeField] Transform helpPopUp;

    [Header("ETC")]
    public GameObject plane;
    public bool isBuildMode = true;
    public bool isSelectMode = false;
    [SerializeField] float boostSpeed;
    [SerializeField] FixedJoystick fixedJoystick;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Sound[] sounds;

    bool onMenu = false;
    bool isBoost = false;

    private void Awake()
    {
        Inst = this;

        ShowPanel("GamePanel");
        ArPlaneEnable(false);
        ControlExitPopup(false);
        ControlHelpPopup(false);
    }

    private void Update()
    { 
        Array.ForEach(pcObjects, x => x.SetActive(isPC));
        Array.ForEach(arObjects, x => x.SetActive(!isPC));

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ControlExitPopup(true);
        }

        if (isBoost)
        {
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;
            Vector3 direction = forward * fixedJoystick.Vertical + right * fixedJoystick.Horizontal;

            arOrigin.transform.Translate(direction * boostSpeed * Time.deltaTime, Space.World);
        }
    }

    public void ShowPanel(string panelName)
    {
        gamePanel.DOAnchorPos(new Vector2(0, -600), 0.4f);
        menuPanel.DOAnchorPos(new Vector2(0, 900), 0.4f);
        joyPanel.DOAnchorPos(new Vector2(0, -600), 0.4f);
        selectPanel.DOAnchorPos(new Vector2(0, -600), 0.4f);

        if (panelName == gamePanel.name)
        {
            gamePanel.DOAnchorPos(new Vector2(0, 0), 0.4f);
        }
        else if (panelName == menuPanel.name)
        {
            menuPanel.DOAnchorPos(new Vector2(0, 0), 0.4f);
        }
        else if (panelName == joyPanel.name)
        {
            joyPanel.DOAnchorPos(new Vector2(0, 0), 0.4f);
        }
        else if (panelName == selectPanel.name)
        {
            selectPanel.DOAnchorPos(new Vector2(0, 0), 0.4f);
        }
    }

    public void MenuClick()
    {
        onMenu = !onMenu;
        ShowPanel(onMenu ? "MenuPanel" : "GamePanel");
        ArPlaneEnable(onMenu);

        StopAllCoroutines();
        StartCoroutine(TopClickCo(!onMenu));

        if (!onMenu && !isBuildMode)
        {
            ShowPanel("JoyPanel");
            isBoost = true;
        }
        else
        {
            isBoost = false;
        }

        if (!onMenu && isSelectMode && isBuildMode)
        {
            ShowPanel("SelectPanel");
        }

        PlaySound("ui_click");
    }

    IEnumerator TopClickCo(bool b)
    {
        yield return new WaitForSeconds(b ? 0.3f : 0);
        menuBtn.SetActive(b);
        helpBtn.SetActive(b);
    }

    void ArPlaneEnable(bool b)
    {
        arPlane.enabled = b;
        foreach (ARPlane plane in arPlane.trackables)
        {
            plane.gameObject.SetActive(b);
        }
    }

    public void ExitClick()
    {
        PlaySound("ui_click");
        Application.Quit();
    }

    public void HelpClick()
    {
        PlaySound("ui_click");
        ControlHelpPopup(true);
    }

    public void ArModeToggle(bool isArMode)
    {
        this.isPC = !isArMode;
        PlaySound("ui_click");
    }

    #region MenuPanel
    public void BuildModeToggle(bool isBuildMode)
    {
        this.isBuildMode = isBuildMode;
        PlaySound("ui_click");

        // true 이면 작아서 건축 가능, false 이면 커서 건축 불가능
        if (isBuildMode)
        {
            arOrigin.transform.localScale = Vector3.one * 24;
            arOrigin.transform.position = Vector3.zero;
        }
        else
        {
            arOrigin.transform.localScale = Vector3.one;
        }
    }

    public void SelectModeToggle(bool isSelectMode)
    {
        this.isSelectMode = isSelectMode;
        PlaySound("ui_click");
    }

    // 바닥 생성 버튼 클릭
    public void PlaceOrigin()
    {
        arRaycaster.Raycast(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), originHits, TrackableType.Planes);

        if (originHits.Count > 0)
        {
            Pose hitpose = originHits[0].pose;

            arOrigin.MakeContentAppearAt(arOrigin.transform, hitpose.position + Vector3.up * 1.5f, hitpose.rotation);
            plane.SetActive(true);
            PlaySound("ui_click");
        }
    }

    // 다시하기 버튼 클릭
    public void NewGameClick()
    {
        //Array.ForEach(GameObject.FindGameObjectsWithTag("Block"), x => Destroy(x));
        PlaySound("ui_click");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    #region 사운드 
    public static void PlaySound(string name)
    {
        AudioClip audioClip = Array.Find(Inst.sounds, x => x.name == name).audioClip;
        Inst.audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.3f);
        Inst.audioSource.PlayOneShot(audioClip);
    }
    #endregion

    #region 팝업창 관련 함수
    public void ControlExitPopup(bool active)
    {
        uiBlock.SetActive(active);

        if (active)
        {
            exitPopUp.DOScale(1.2f, 0.4f);
        }
        else
        {
            exitPopUp.DOScale(0f, 0.4f);
        }
    }

    public void ControlHelpPopup(bool active)
    {
        uiBlock.SetActive(active);

        if (active)
        {
            helpPopUp.DOScale(1.2f, 0.4f);
        }
        else
        {
            helpPopUp.DOScale(0f, 0.4f);
            helpPopUp.GetComponent<HelpPopUp>().Reset();
        }
    }
    #endregion
}
