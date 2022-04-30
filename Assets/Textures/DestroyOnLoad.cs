using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple suicide script for additive loading
/// </summary>
public class DestroyOnLoad : MonoBehaviour
{
    private void Awake()
    {
        this.tag = "Untagged";
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        Destroy(this.gameObject);
    }
}
