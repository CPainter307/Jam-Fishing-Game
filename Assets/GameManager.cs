using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Canvas GameCanvas;
    public CanvasGroup BigFade;
    public UnityEngine.UI.Text BigText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            print("Destroying existing singleton");
        }

        FadeIn();
    }

    public void FadeIn()
    {
        LeanTween.alphaCanvas(BigFade, 0f, 2.0f);
    }
    public void FadeOut()
    {
        LeanTween.alphaCanvas(BigFade, 1f, 2.0f);
    }


    public void TriggerWin()
    {
        BigText.gameObject.active = true;
        FadeOut();
        BigText.text = "You Win!";
    }
    public void TriggerLose()
    {
        BigText.gameObject.active = true;
        FadeOut();
        BigText.text = "You Lose!";
    }
}
