using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;

/// <summary>
/// The PlayerGUI class handless all the UI behavior of the player GUI. This includes sending info out in form of events and listening to events in order to perfrom certain tasks.
/// </summary>
public class PlayerGUI : MonoBehaviour
{
    // Pause menu events. Game controller does things based on these.
    public static event Action ResumeEvent;
    public static event Action RestartEvent;
    public static event Action QuitEvent;

    // Center recticle
    public RectTransform centerReticle;
    private RawImage reticleImage;

    // Pause Menu
    public RectTransform pauseMenuCanvas;

    // Patch tracker
    public TextMeshProUGUI patchCounterText;

    private static int loadIteration = 0;

    protected void Awake()
    {
        if (loadIteration++ > 0) // Is this the first time we're loading a scene?
            return; // It is? don't go through the rest of Awake().

        DontDestroyOnLoad(this.gameObject); // Player GUI stays present until told otherwise.

        // Events
        InitializeEventSystem();
        SubscribeToEvents();

        // UI elements
        InitializeCenterReticle();
        InitializePauseMenu();
        InitializePatchCounter();

        Debug.Log("PlayerGUI initialized.");

    }
    void SubscribeToEvents()
    {
        // When subscribing to events it is crucial to remember to also unsubscribe from them.
        PlayerPatchHandler.PatchInRange    += OnPatchInRange;
        PlayerPatchHandler.PatchOutOfRange += OnPatchOutOfRange;
        PlayerPatchHandler.CountUpdate     += UpdatePatchCounter;
    }
    void UnsubscribeFromEvents()
    {
        // Unsubscribing from events. Outhis is crucial in order to protect from memory leaks.
        PlayerPatchHandler.PatchInRange    -= OnPatchInRange;
        PlayerPatchHandler.PatchOutOfRange -= OnPatchOutOfRange;
        PlayerPatchHandler.CountUpdate     -= UpdatePatchCounter;

    }

    void InitializeEventSystem()
    {
        // We need to make sure there is only one event system in the scene.
        // Warning: may be perfomance intensive? Problem can also be solved by being strict about deleting event systems

        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        foreach(EventSystem eSys in eventSystems)
            if (eSys.CompareTag("EventSystem"))
                DontDestroyOnLoad(eSys.gameObject); // Make sure our desired event system doesn't get deleted in the future.
            else
                Destroy(eSys.gameObject); // Only one event system can exist at a time. Make sure extra-event system get deleted.

    }
    void InitializeCenterReticle()
    {
        if (centerReticle != null)
        {
            reticleImage = centerReticle.GetComponent<RawImage>();
            if (reticleImage == null)
                Debug.Log("Null image component");
            reticleImage.gameObject.SetActive(true); // Make sure the object is active.
        }
        else
            Debug.LogError("Center reticle reference missing from PlayerGUI. Please add reference!");

    }

    void InitializePatchCounter()
    {
        UpdatePatchCounter(0);
    }

    void InitializePauseMenu()
    {
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.gameObject.SetActive(false); // Pause menu is invisible by defeault
        }
        else
            Debug.LogError("Pause menu canvas reference missing from PlayerUI. Please add reference!");

    }
    void OnPatchInRange(string id)
    {
        //Debug.Log(id);
        switch(id)
        {
            case "Socket":
        reticleImage.color = Color.red;
            break;

            case "Cable":
        reticleImage.color = Color.cyan;
            break;

            default:
        reticleImage.color = Color.white;
            break;
        }
        //Debug.Log("Socket in range!");
    }

    void OnPatchOutOfRange(string id)
    {
        Debug.Log("Out.");
        switch(id)
        {
            default:
        reticleImage.color = Color.white;
            break;
        }
    }

    void UpdatePatchCounter(int count)
    {
        patchCounterText.text = "PATCHES: " + count.ToString();
    }

    public void ShowPauseMenu(bool shouldShow)
    {
        // State of pause menu UI
        pauseMenuCanvas.gameObject.SetActive(shouldShow);

        // State of other UI elements
        centerReticle.gameObject.SetActive(!shouldShow);
    }

    public void Resume()
    {
        if (ResumeEvent != null)
            ResumeEvent();
    }

    public void Restart()
    {
        if(RestartEvent != null)
            RestartEvent();
    }
    public void QuitGame()
    {
        if(QuitEvent != null)
           QuitEvent();
    }

    public void ResetGUI()
    {
        ShowPauseMenu(false);
    }

    protected void OnApplicationQuit()
    {
        UnsubscribeFromEvents();
    }



}
