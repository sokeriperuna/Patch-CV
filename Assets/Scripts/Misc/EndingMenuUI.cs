using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingMenuUI : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        GameObject gc = GameObject.FindGameObjectWithTag("GameController");
        if (gc != null)
            Destroy(gc);
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void OpenFeedbackForm(){
        Application.OpenURL("https://forms.gle/ARwVSAtuYTVdmA739");
    }
}
