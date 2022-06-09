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
    public GameObject plane;
    public bool isBuildMode = true;
    [SerializeField] float boostSpeed;
    [SerializeField] FixedJoystick fixedJoystick;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Sound[] sounds;

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
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;
            Vector3 direction = forward * fixedJoystick.Vertical + right * fixedJoystick.Horizontal;

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
            PlaySound("ui_click");
        }
    }

    public void ShowPanel(string panelName)
    {
        gamePanel.DOAnchorPos(new Vector2(0, -600), 0.4f);
        menuPanel.DOAnchorPos(new Vector2(0, 600), 0.4f);
        joyPanel.DOAnchorPos(new Vector2(0, -600), 0.4f);

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
    }

    public void MenuClick()
    {
        onMenu = !onMenu;
        ShowPanel(onMenu ? "MenuPanel" : "GamePanel");
        ArPlaneEnable(onMenu);

        StopAllCoroutines();
        StartCoroutine(MenuClickCo(!onMenu));

        if (!onMenu && !isBuildMode)
        {
            ShowPanel("JoyPanel");
            isBoost = true;
        }
        else
        {
            isBoost = false;
        }

        PlaySound("ui_click");
    }

    IEnumerator MenuClickCo(bool b)
    {
        yield return new WaitForSeconds(b ? 0.3f : 0);
        menuBtn.SetActive(b);
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

    public void NewGameClick()
    {
        //Array.ForEach(GameObject.FindGameObjectsWithTag("Block"), x => Destroy(x));
        PlaySound("ui_click");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

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

    public static void PlaySound(string name)
    {
        AudioClip audioClip = Array.Find(Inst.sounds, x => x.name == name).audioClip;
        Inst.audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.3f);
        Inst.audioSource.PlayOneShot(audioClip);
    }
}
