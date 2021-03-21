using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Canvas GameCanvas;
    public CanvasGroup BigFade;
    public UnityEngine.UI.Text BigText;

    public UnityEngine.UI.Text LosingText;
    public UnityEngine.UI.Image ReelString;
    public UnityEngine.UI.Image ReelOutside;

    public float minStringScale = .3f;
    public float maxStringScale = 1.0f;

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
        BigText.text = "You Caught The Big One!!";

        GameObject.FindObjectOfType<PlayerBehavior>().godMode = true;

        Destroy(GameObject.FindObjectOfType<FishBehavior>().gameObject);
    }

    public void TriggerLose()
    {
        BigText.gameObject.active = true;
        BigText.text = "You Couldn't Catch The Big One...";
    }

    public void UpdateReelUI(float reelSize, PlayerLineController.State reelState, float reelSpeed)
    {

        float reelInScaled = 15.0f;
        float reelOutScaled = reelSpeed * 1500.0f;
        switch (reelState)
        {
            case PlayerLineController.State.ReelingOut:
                ReelOutside.rectTransform.eulerAngles -= new Vector3(0, 0, reelInScaled);
                ReelString.rectTransform.localScale += new Vector3(reelSpeed, reelSpeed);
                break;
            case PlayerLineController.State.ReelingIn:
                ReelOutside.rectTransform.eulerAngles += new Vector3(0, 0, reelOutScaled);
                ReelString.rectTransform.localScale -= new Vector3(reelSpeed, reelSpeed);
                break;
            case PlayerLineController.State.MaxReel:
                if (!LeanTween.isTweening(ReelOutside.rectTransform))
                {
                    LeanTween.scale(ReelOutside.rectTransform, new Vector3(1.2f, 1.2f), .1f).setLoopPingPong().setEaseInOutSine();
                    LeanTween.color(ReelOutside.rectTransform, Color.red, 3.0f);
                }
                if (!LeanTween.isTweening(LosingText.rectTransform))
                    LeanTween.alphaCanvas(LosingText.GetComponent<CanvasGroup>(), 1f, .3f);
                break;
            case PlayerLineController.State.MinReel:
                if (!LeanTween.isTweening(ReelOutside.rectTransform))
                {
                    LeanTween.move(ReelOutside.rectTransform, new Vector3(1.3f, 1f), .05f).setLoopPingPong().setEaseInOutSine();
                    LeanTween.color(ReelOutside.rectTransform, new Color(1f, 1f, 1f, .5f), .1f);
                }
                break;
        }
    }

    public void StartState()
    {
        LeanTween.cancel(LosingText.rectTransform);
        LeanTween.cancel(ReelOutside.rectTransform);
        ReelOutside.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        ReelOutside.rectTransform.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        LeanTween.alphaCanvas(LosingText.GetComponent<CanvasGroup>(), 0f, .3f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TriggerWin();
        }
    }
}
