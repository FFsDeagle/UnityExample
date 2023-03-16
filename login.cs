/*
    Login Script
    NOTE: DATABASE CONNECTIVITIY IS NOT YET IMPLEMENTED, ONLY A MOCKUP OF THE LOGIN SCREEN AND CHARACTER CREATION SCREEN
    UI Phase 01: Login Screen
    UI Phase 02: Character Selection
    UI Phase 03: Class Selection
    UI Phase 04: Character Creation
    Created by Muhammad Farid Firoz Ahmed
    Changes to be made:
    02.05.2022
        - Work needs to be done when it comes to correctly structuring this script, at this point of time I can see that it takes me 
          10 - 20 mins to re-understand what i've done.
        - Perhaps a document to explain the hierarchy of the class structure?
        - A number of these methods should be created as seperate classes and used globally, rather than writing them out in every script.
          A good example is the FadeOut for Sound and Screen Alpha.
        - Work needs to be done on the naming conventions used.
        - Remove/Reuse used/unused variables
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class login : MonoBehaviour
{
    public Transform cameraStartPos;
    private AudioSource btnClick;
    private bool m_Play, m_ToggleChange, startEffect, effectRunning, switchIntro, keyPress;

    public GameObject warriorSelection, thiefSelection, mageSelection, witchSelection, archerSelection, gunmanSelection;
    public GameObject warriorPrefab, thiefPrefab, magePrefab, witchPrefab, archerPrefab, gunmanPrefab;
    public Transform startMarker, endMarker, startMarkerPhase04, endMarkerPhase04;
    private float journeyLength, startTime, prevtimeToFade, journeyLengthPhase04;
    public float DeactivateTime;
    private Button btnLogin, btnOptions, btnCreate, btnWarrior, btnThief, btnMage, btnWitch, btnArcher, btnGunman;
    public GameObject characterSelectionEffect;
    public Transform effectPos, playerSpawnPos;
    public GameObject uiPhase01, uiPhase02, uiPhase03, playerUi, optionsMenu;
    private Camera cam;

    public GameObject camera01, camera02, playerControl;

    private GameObject prevInstance, checkInstance;

    private bool createFirstInstance;

    private bool startTran, startTranPhase04, play;
    public float cameraSpeed, cameraAccelleration, timeToFade;

    private Text classInfo, classDescription;

    public Button btnCreateClass, sampleAbility01, sampleAbility02, sampleAbility03, sampleAbility04, btnBack;

    public VideoPlayer video;

    public AudioSource IntroMusic, introPlayer, introVox;
    public Animator anim;

    private string playerSelection, pickedClass;

    private addPlayerTimeline startPhase;

    private getplayerGameObject playerObject;

    public float timeforIntro;
    private Transform NPC_Harold;

    private videoSampleAbility sampleAbility;
    private VideoClip[] classVideo = new VideoClip[4];
    public VideoPlayer playVideo;
    private GameObject showSampleAbility, classInfoTab;
    private playerController playerCont;
    private string currentMenu;

    void Start()
    {
        NPC_Harold = GameObject.Find("SM_Chr_Hermit_01").GetComponent<Transform>();
        switchIntro = false;
        createFirstInstance = false;
        prevtimeToFade = timeToFade;
        effectRunning = false;
        uiPhase01.SetActive(true);
        uiPhase02.SetActive(false);
        uiPhase03.SetActive(false);
        currentMenu = "uiPhase01";
        m_Play = true;
        btnClick = GameObject.Find("btnClickAudio").GetComponent<AudioSource>();
        journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
        journeyLengthPhase04 = Vector3.Distance(startMarkerPhase04.position, endMarkerPhase04.position);
        startTran = false;
        btnLogin = GameObject.Find("btnLogin").GetComponent<Button>();
        btnOptions = GameObject.Find("btnOptions").GetComponent<Button>();
        btnLogin.onClick.AddListener(() => ButtonClicked(01)); //btnLogin
        btnOptions.onClick.AddListener(() => ButtonClicked(02)); //btnOption
        cam = GameObject.Find("MainCamera1").GetComponent<Camera>();
        effectPos = GetComponent<Transform>();
        btnCreateClass.gameObject.SetActive(false);
        play = true;
        startPhase = FindObjectOfType(typeof(addPlayerTimeline)) as addPlayerTimeline;
        playerObject = FindObjectOfType(typeof(getplayerGameObject)) as getplayerGameObject;
        //ResetCameraPostion();
        keyPress = false;
    }

    private void ResetCameraPostion()
    {
        cam.transform.position = new Vector3(4.18004799f, 5.05999994f, -31.8099995f);
        cam.transform.Rotate(0f, 0f, 0f);
        Debug.Log(cam.transform.position);
    }

    void Update()
    {
        if (startEffect == true)
        {
            timeToFade -= Time.deltaTime;
            if (timeToFade <= 0)
            {
                RFX4_EffectSettings flame01;
                flame01 = GameObject.Find("Effect28(Clone)").GetComponent<RFX4_EffectSettings>();
                flame01.IsVisible = false;
                effectRunning = false;
                startEffect = false;
            }
        }

        //Phase01 Camera Moving Mechanism
        if (startTran)
        {
            float distCovered = Time.deltaTime * cameraSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            cam.transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);
            if (startMarker.position.y <= 0.632f)
            {
                startTran = false;
            }
        }

        if (startTranPhase04)
        {
            // FadeOut(IntroMusic, 2f);
            startTran = false;
            float distCovered = Time.deltaTime * cameraSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            cam.transform.position = Vector3.Lerp(startMarkerPhase04.position, endMarkerPhase04.position, fractionOfJourney);
            // if(startMarkerPhase04.position.y <= 0.632f){
            //     startTranPhase04 = false;
            // }
        }

        //Background sound
        playSound();

        //Method to detect keypress which has a cooldown for keypresses
        //Only applicable to menu items and NOT in game abilities
        menuKeyInput();
    }

    private void menuKeyInput()
    {
        if (keyPress == false)
        {
            if (Input.GetKey("escape"))
            {
                m_ToggleChange = true;
                keyPress = true;
                Debug.Log(keyPress);
                ButtonClicked(02);
                StartCoroutine(keyPressTime());
            }
        }
    }

    private IEnumerator keyPressTime()
    {
        float i = 0.05f;
        while (i >= 0)
        {
            i -= Time.deltaTime;
            yield return null;
        }
        keyPress = false;
    }

    void ButtonClicked(int ButtonNo)
    {
        switch (ButtonNo)
        {
            case 01:
                m_ToggleChange = true;
                //Proceed Character Selection/Creation
                moveCamera(true);
                break;
            case 02:
                //Proceed to Options screen UI
                m_ToggleChange = true;
                uiPhase01.SetActive(false);
                uiPhase02.SetActive(false);
                uiPhase03.SetActive(false);
                playerUi.SetActive(false);
                optionsMenu.SetActive(true);
                btnBack.onClick.AddListener(() => ButtonClicked(15));
                break;
            case 03:
                m_ToggleChange = true;
                uiPhase02.SetActive(false);
                uiPhase03.SetActive(true);
                currentMenu = "uiPhase03";
                setButtons();
                break;
            case 04:
                // playEffect();
                displayClass(04);
                break;
            case 05:
                // playEffect();
                displayClass(05);
                break;
            case 06:
                // playEffect();
                displayClass(06);
                break;
            case 07:
                // playEffect();
                displayClass(07);
                break;
            case 08:
                // playEffect();
                displayClass(08);
                break;
            case 09:
                // playEffect();
                displayClass(09);
                break;
            case 10:
                m_ToggleChange = true;
                setClass(pickedClass);
                moveCameraPhase04(true);
                break;
            case 11:
                //InitializeVideos will run a method to find out what class you have selected
                //Which then you can set the Sample Ability to play
                sampleAbility.InitalizeVideos();
                setSampleAbilityBtn(0);
                break;
            case 12:
                sampleAbility.InitalizeVideos();
                setSampleAbilityBtn(1);
                break;
            case 13:
                sampleAbility.InitalizeVideos();
                setSampleAbilityBtn(2);
                break;
            case 14:
                sampleAbility.InitalizeVideos();
                setSampleAbilityBtn(3);
                break;
            case 15:
                m_ToggleChange = true;
                setOptionsMenu(currentMenu);
                break;
        }
    }

    private void setOptionsMenu(string menuName)
    {
        switch (menuName)
        {
            case "uiPhase01":
                uiPhase01.SetActive(true);
                break;
            case "uiPhase02":
                uiPhase02.SetActive(true);
                break;
            case "uiPhase03":
                uiPhase03.SetActive(true);
                break;
            case "PlayerUi":
                playerUi.SetActive(true);
                break;
            default:
                break;
        }
        optionsMenu.SetActive(false);
    }

    public void setSampleAbilityBtn(int buttonNo)
    {
        showSampleAbility.SetActive(true);
        classVideo = sampleAbility.getVideoList();
        playVideo = GameObject.Find("videoPlayer").GetComponent<VideoPlayer>();
        Debug.Log("Play Video");
        // playVideo.clip = null;
        switch (buttonNo)
        {
            case 0:
                playVideo.frame = 0;
                playVideo.clip = classVideo[0];
                playVideo.SetDirectAudioMute(0, true);
                break;
            case 1:
                playVideo.frame = 0;
                playVideo.clip = classVideo[1];
                playVideo.SetDirectAudioMute(0, true);
                break;
            case 2:
                playVideo.frame = 0;
                playVideo.clip = classVideo[2];
                playVideo.SetDirectAudioMute(0, true);
                break;
            case 3:
                playVideo.frame = 0;
                playVideo.clip = classVideo[3];
                playVideo.SetDirectAudioMute(0, true);
                break;
        }
    }

    private void playEffect()
    {
        timeToFade = prevtimeToFade;
        if (effectRunning == false)
        {
            GameObject flame = Instantiate(characterSelectionEffect, new Vector3(-20.81f, 0.268f, 124.5f), Quaternion.identity);
            effectRunning = true;
            startEffect = true;
        }
    }

    private void destroyInstance(string className)
    {
        List<string> classNames = new List<string>(5);
        string[] names = {"Chr_FantasyHero_Preset_3(Clone)", "Chr_FantasyHero_Preset_19(Clone)",
                        "Chr_FantasyHero_Preset_23(Clone)", "Chr_FantasyHero_Preset_30(Clone)",
                        "Chr_FantasyHero_Preset_9(Clone)", "Chr_FantasyHero_Preset_2(Clone)"};

        List<string> namesList = new List<string>(names);

        namesList.Remove(className);

        foreach (string a in namesList)
        {
            GameObject checkInstance = GameObject.Find(a);
            Debug.Log(a);
            if (checkInstance)
            {
                Destroy(checkInstance.gameObject);
            }
        }
    }

    private bool checkDouble(string className)
    {
        createFirstInstance = true;
        checkInstance = GameObject.Find(className);
        if (checkInstance != prevInstance)
        {
            prevInstance = checkInstance;
        }

        if (checkInstance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void displayClass(int selectedClass)
    {
        m_ToggleChange = true;
        classInfoTab.SetActive(true);
        btnCreateClass.gameObject.SetActive(true);
        Quaternion rotation = Quaternion.Euler(0, -180, 0);
        Vector3 spawnPos = new Vector3(playerSpawnPos.position.x, playerSpawnPos.position.y, playerSpawnPos.position.z);
        switch (selectedClass)
        {
            case 04:
                if (!checkDouble("Chr_FantasyHero_Preset_3(Clone)"))
                {
                    GameObject warriorIns = Instantiate(warriorPrefab, spawnPos, rotation);
                    playerSelection = "Chr_FantasyHero_Preset_3(Clone)";
                    classInfo.text = "Warrior";
                    classDescription.text = "A powerful heavy melee class";
                    pickedClass = "Chr_FantasyHero_Preset_3";
                }
                destroyInstance("Chr_FantasyHero_Preset_3(Clone)");
                break;
            case 05:
                if (!checkDouble("Chr_FantasyHero_Preset_19(Clone)"))
                {
                    GameObject thiefIns = Instantiate(thiefPrefab, spawnPos, rotation);
                    playerSelection = "Chr_FantasyHero_Preset_19(Clone)";
                    classInfo.text = "Thief";
                    classDescription.text = "A swift agile melee class";
                    pickedClass = "Chr_FantasyHero_Preset_19";
                }
                destroyInstance("Chr_FantasyHero_Preset_19(Clone)");
                break;
            case 06:
                if (!checkDouble("Chr_FantasyHero_Preset_23(Clone)"))
                {
                    GameObject mageIns = Instantiate(magePrefab, spawnPos, rotation);
                    playerSelection = "Chr_FantasyHero_Preset_23(Clone)";
                    classInfo.text = "Mage";
                    classDescription.text = "A powerful ranged spell class";
                    pickedClass = "Chr_FantasyHero_Preset_23";
                }
                destroyInstance("Chr_FantasyHero_Preset_23(Clone)");
                break;
            case 07:
                if (!checkDouble("Chr_FantasyHero_Preset_30(Clone)"))
                {
                    GameObject witchIns = Instantiate(witchPrefab, spawnPos, rotation);
                    playerSelection = "Chr_FantasyHero_Preset_30(Clone)";
                    classInfo.text = "Witch";
                    classDescription.text = "A powerful ranged spell class";
                    pickedClass = "Chr_FantasyHero_Preset_30";
                }
                destroyInstance("Chr_FantasyHero_Preset_30(Clone)");
                break;
            case 08:
                if (!checkDouble("Chr_FantasyHero_Preset_2(Clone)"))
                {
                    GameObject archerIns = Instantiate(archerPrefab, spawnPos, rotation);
                    playerSelection = "Chr_FantasyHero_Preset_2(Clone)";
                    classInfo.text = "Gunman";
                    classDescription.text = "A powerful heavy Ranged class";
                    pickedClass = "Chr_FantasyHero_Preset_2";
                }
                destroyInstance("Chr_FantasyHero_Preset_2(Clone)");
                break;
            case 09:
                if (!checkDouble("Chr_FantasyHero_Preset_9(Clone)"))
                {
                    GameObject gunmanIns = Instantiate(gunmanPrefab, spawnPos, rotation);
                    playerSelection = "Chr_FantasyHero_Preset_9(Clone)";
                    classInfo.text = "Archer";
                    classDescription.text = "A powerful swift and agile Ranged class";
                    pickedClass = "Chr_FantasyHero_Preset_9";
                }
                destroyInstance("Chr_FantasyHero_Preset_9(Clone)");
                break;
        }
    }

    private void setButtons()
    {
        classDescription = GameObject.Find("classDescription").GetComponent<Text>();
        classInfo = GameObject.Find("classInfo").GetComponent<Text>();
        btnWarrior = GameObject.Find("btnWarrior").GetComponent<Button>();
        btnThief = GameObject.Find("btnThief").GetComponent<Button>();
        btnMage = GameObject.Find("btnMage").GetComponent<Button>();
        btnWitch = GameObject.Find("btnWitch").GetComponent<Button>();
        btnArcher = GameObject.Find("btnArcher").GetComponent<Button>();
        btnGunman = GameObject.Find("btnGunman").GetComponent<Button>();
        sampleAbility = FindObjectOfType(typeof(videoSampleAbility)) as videoSampleAbility;

        btnWarrior.onClick.AddListener(() => ButtonClicked(04));
        btnThief.onClick.AddListener(() => ButtonClicked(05));
        btnMage.onClick.AddListener(() => ButtonClicked(06));
        btnWitch.onClick.AddListener(() => ButtonClicked(07));
        btnArcher.onClick.AddListener(() => ButtonClicked(08));
        btnGunman.onClick.AddListener(() => ButtonClicked(09));
        btnCreateClass.onClick.AddListener(() => ButtonClicked(10)); //btnCreateClass
        sampleAbility01.onClick.AddListener(() => ButtonClicked(11));
        sampleAbility02.onClick.AddListener(() => ButtonClicked(12));
        sampleAbility03.onClick.AddListener(() => ButtonClicked(13));
        sampleAbility04.onClick.AddListener(() => ButtonClicked(14));

        showSampleAbility = GameObject.Find("showSampleAbility");
        showSampleAbility.SetActive(false);
        classInfoTab = GameObject.Find("classInfoTab");
        classInfoTab.SetActive(false);
    }

    private void playSound()
    {
        if (m_Play == true && m_ToggleChange == true)
        {
            //Play the audio you attach to the AudioSource component
            btnClick.Play();
            Debug.Log("Playing Audio");
            //Ensure audio doesn’t play more than once
            m_ToggleChange = false;
        }
    }

    private void moveCamera(bool startTransition)
    {
        startTran = startTransition;
        //set all UI objects to hidden to proceed into next scene
        uiPhase01.SetActive(false);
        uiPhase02.SetActive(true);
        currentMenu = "uiPhase02";
        btnCreate = GameObject.Find("btnCreate").GetComponent<Button>();
        btnCreate.onClick.AddListener(() => ButtonClicked(03));
    }

    //
    private void moveCameraPhase04(bool startTransition)
    {
        playerObject.getPlayerObject(playerSelection);
        startPhase.initializeSelectedClass(0);
        anim.SetTrigger("FadeOut");
        startTranPhase04 = startTransition;
        StartCoroutine(FadeOut(cam, IntroMusic, 3f));
        uiPhase03.SetActive(false);
        playSound();
    }

    //Start Intro Phase 01 & 02
    private void startPlayerIntro()
    {
        //Start Timer
        //Start Sequence
        introPlayer.Play(); //Intro CutScene 01 Background Music
        introVox.Play();    //Intro CutScene 01 Narration

        StartCoroutine(IntroTimer(timeforIntro));   //Intro CutScene 01 Duration as public variable
    }

    //Intro timer for the first CutScene
    //The time is the duration of the cutscene
    //The fadeout time is set by the user, currently as 3 seconds
    public IEnumerator IntroTimer2(float FadeTime)
    {
        bool fade = false;
        while (FadeTime >= 0)
        {
            FadeTime -= Time.deltaTime;
            if (FadeTime <= 3 && fade == false)
            {
                Debug.Log("Coroutine Fade");
                anim.SetTrigger("FadeOut");
                fade = true;
            }
            yield return null;
        }
        //Animation Fadeout
        anim.SetTrigger("FadeOut");
        camera02.SetActive(false);
        playerControl.SetActive(true);
        //Sounds required   
        //
        //Reposition NPC Harold
        startPhase.stopSelectedClass(1);
        currentMenu = "PlayerUi";
        NPC_Harold.transform.position = new Vector3(-57.7299995f, 0.129999995f, 152.309998f);
        NPC_Harold.transform.rotation = Quaternion.Euler(0, -180, 0);
        Debug.Log("Reset Position NPC Harold");
    }

    //Intro timer for the first CutScene
    //The time is the duration of the cutscene
    //The fadeout time is set by the user, currently as 3 seconds
    public IEnumerator IntroTimer(float FadeTime)
    {
        bool fade = false;
        while (FadeTime >= 0)
        {
            FadeTime -= Time.deltaTime;
            if (FadeTime <= 3 && fade == false)
            {
                Debug.Log("Coroutine Fade");
                anim.SetTrigger("FadeOut");
                fade = true;
            }
            yield return null;
        }

        Debug.Log("Fadeout02");
        anim.SetTrigger("FadeOut");
        introPlayer.Stop();
        introVox.Stop();
        startPhase.initializeSelectedClass(1);
        StartCoroutine(IntroTimer2(7f));    //Intro CutScene 02 Duration as 5 seconds
    }

    //Set picked class to show
    //Instead of using a Instance of the picked class I've decided to set the prefabs of the class types
    //onto the scene and then show/hide based on the users selection
    private void setClass(string className)
    {
        Debug.Log("SetClass");
        pickedClass = className;
        switch (className)
        {
            case "Chr_FantasyHero_Preset_3":
                warriorSelection.SetActive(true);
                break;
            case "Chr_FantasyHero_Preset_19":
                thiefSelection.SetActive(true);
                break;
            case "Chr_FantasyHero_Preset_23":
                mageSelection.SetActive(true);
                break;
            case "Chr_FantasyHero_Preset_30":
                witchSelection.SetActive(true);
                break;
            case "Chr_FantasyHero_Preset_9":
                gunmanSelection.SetActive(true);
                break;
            case "Chr_FantasyHero_Preset_2":
                archerSelection.SetActive(true);
                break;
        }
    }

    //Get picked class used for attatching the camera to the player
    public string getClass()
    {
        return pickedClass;
    }

    //Fadeout Sound
    //Note: After focusing more on structure, it may be worth working on this method.
    //Attempt to make it a global function which can be called from any class, this will be helpful
    //to use in future scenes.
    public IEnumerator FadeOut(Camera cam, AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }

        anim.SetTrigger("FadeOut");
        audioSource.Stop();
        audioSource.volume = startVolume;
        camera01.SetActive(false);
        camera02.SetActive(true);
        startPlayerIntro();
    }
}
