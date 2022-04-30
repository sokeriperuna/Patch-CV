using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Patch.SocketUtility;


public class GameController : MonoBehaviour
{
    private bool pauseMenuActive = false;
    private bool mouseLocked;

    private GameObject inputHandler;
    private ScriptThatHandlesInputs inputProvider;

    private GameObject player;
    private PlayerRotation playerRot;

    private PlayerGUI playerGUI;

    private static int loadIteration = 0;

    // Start is called before the first frame update
    void Awake() 
    {
        PurgeDuplicateGameControllers();

        if (!this.CompareTag("GameController")) // Are we the prime game controller?
            return;

        Debug.Log("GC load iteration: " + loadIteration.ToString());
        if (loadIteration++ > 0) // Is this the first time we're loading a scene?
            return; // It is? don't go through the rest of Awake().

        DontDestroyOnLoad(this.gameObject);

        SubscribeToEvents();

        LockAndHideMouse();

        InitializeInputs();

        SceneManager.LoadScene("PlayerGUI", LoadSceneMode.Additive); // We begin the additive load but it does not halt code execution.


    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode) // Called when a new scene is loaded in, including when this.gameObject is first loaded.
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "EndingScene")
            GameObject.Destroy(this.gameObject);

        // Have we loeaded in the scene GUI?
        if (scene.name == "PlayerGUI" && mode == LoadSceneMode.Additive)
        {
            playerGUI = GameObject.FindGameObjectWithTag("PlayerGUI").GetComponent<PlayerGUI>();
            if (playerGUI != null)
                Debug.Log("Player GUI succesfully loaded.");
        }
        else // Seems like we're just loading a normal scene
        {
            if(playerGUI != null)
                playerGUI.ResetGUI();

            // Get player reference if we don't have it already
            if (player == null)
            {
                player = FindObjectOfType<PlayerObject>().gameObject;
                playerRot = player.GetComponent<PlayerRotation>();
            }

            LockAndHideMouse();
        }
    }

    void PurgeDuplicateGameControllers()
    {
        GameController[] gcs = FindObjectsOfType<GameController>();
        if(gcs.Length > 1)
        {
            foreach(GameController gc in gcs)
                if (!gc.CompareTag("GameController")) // We're not the prime game controller? We destroy ourselves.
                {
                    Destroy(this.gameObject);
                    Debug.Log("Purged duplicate game controller from scene.");
                }
        }
        else
            this.tag = "GameController"; // This is the real and prime game controller.
    }

    void LockAndHideMouse()
    {
        mouseLocked = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void InitializeInputs()
    {
        GameObject inputHandlerPrefab = Resources.Load<GameObject>("Prefabs/Input Handler");
        inputHandler = Instantiate(inputHandlerPrefab, Vector3.zero, Quaternion.identity);
        DontDestroyOnLoad(inputHandler);

        inputProvider = inputHandler.GetComponent<ScriptThatHandlesInputs>();

        inputProvider.getPlayerInputs.escapeEvent += PauseMenuToggle;
    }

    void PauseMenuToggle()
    {
        pauseMenuActive = !pauseMenuActive; // Flip pause menu state

        //Debug.Log("PAUSE MENU TOGGLE: " + pauseMenuActive.ToString());

        // Show/hide pause menu
        playerGUI.ShowPauseMenu(pauseMenuActive);

        /// Update other pause stuff based on state

        playerRot.enabled = !pauseMenuActive; // Restrict player look based on pause menu state
        Time.timeScale    =  pauseMenuActive ? 0f : 1f; // Restrict timescale base on pause menu state

        // Lock mouse based on pause menu state
        mouseLocked       = !pauseMenuActive; 
        Cursor.visible    =  pauseMenuActive;
        Cursor.lockState  =  pauseMenuActive ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void PlayerDeath()
    {
        RestartLevel();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void RestartLevel()
    {
        if (pauseMenuActive)
            PauseMenuToggle(); // Unpause the game

        playerGUI.ResetGUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void LoadNextLevel()
    {
        if(SceneManager.GetActiveScene().name != "PlayerGUI")
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
        {
            QuitGame();
        }
    }

    void QuitGame()
    {
        Time.timeScale = 1f;
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying=false;
        #endif
        Application.Quit();
    }

    void SubscribeToEvents()
    {
        Debug.Log("Subscribing GameContorller");
        // Pause menu events
        PlayerGUI.ResumeEvent  += PauseMenuToggle;
        PlayerGUI.RestartEvent += RestartLevel;
        PlayerGUI.QuitEvent    += QuitGame;

        // Player events
        PlayerObject.OnPlayerDeath += PlayerDeath;

        // Goal event
        Goal.LevelFinished += LoadNextLevel;

        // Scene management
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    void OnApplicationQuit()
    {
        UnsubscribeFromEvents();
    }

    private void OnDestroy()
    {
        Destroy(playerGUI.gameObject);
        UnsubscribeFromEvents();
    }

    void UnsubscribeFromEvents()
    {
        if (!this.CompareTag("GameController")) // Are we not the prime game controller?
            return; // No need to unsubscribe as we never subscribed in the first place

        
        // Pause Menu
        PlayerGUI.ResumeEvent -= PauseMenuToggle;
        PlayerGUI.RestartEvent -= RestartLevel;
        PlayerGUI.QuitEvent -= QuitGame;

        // Inputs
        if (inputProvider == null)
            Debug.Log("IP: null");
        inputProvider.getPlayerInputs.escapeEvent -= PauseMenuToggle;

        // Player
        PlayerObject.OnPlayerDeath -= PlayerDeath;

        // Scene
        SceneManager.sceneLoaded -= OnSceneLoad;

        // Goal
        Goal.LevelFinished -= LoadNextLevel;

        Debug.Log("Game controller unsubscribed from events.");
    }
}
