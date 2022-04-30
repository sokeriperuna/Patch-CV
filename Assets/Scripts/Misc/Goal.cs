using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Goal : MonoBehaviour
{
    public static event System.Action LevelFinished;
    private void OnCollisionEnter(Collision other) {
        if(other.collider.gameObject.CompareTag("Player"))
            if(LevelFinished != null)
                LevelFinished();
    }
}
