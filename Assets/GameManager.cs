using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Canvas GameCanvas;
    public Canvas FishCanvas;
    public CanvasGroup BigFade;
    public UnityEngine.UI.Text BigText;

    public UnityEngine.UI.Text LosingText;
    public UnityEngine.UI.Image ReelString;
    public UnityEngine.UI.Image ReelOutside;

    public float minStringScale = .3f;
    public float maxStringScale = 1.0f;

    private bool gameEnded = false;

    public string fishName = "The Big One";
    public Sprite fishSprite;

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

        StartState();
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
        if (GameObject.FindObjectOfType<PlayerBehavior>().dead) return;

        GameObject.FindObjectOfType<PlayerBehavior>().godMode = true;

        gameEnded = true;
        SetFishText(fishName);
        SetFishSprite(fishSprite);

        Destroy(GameObject.FindObjectOfType<FishBehavior>().gameObject);
    }

    public void SetFishSprite(Sprite fishSprite)
    {
        FishCanvas.GetComponentInChildren<UnityEngine.UI.Image>().sprite = fishSprite;
    }

    public void SetFishText(string fishName)
    {
        FishCanvas.GetComponentInChildren<UnityEngine.UI.Text>().text = "You Caught " + fishName + "!";
        LeanTween.alphaCanvas(FishCanvas.GetComponent<CanvasGroup>(), 1f, 1.0f);
    }

    public void ResetFishOverlay()
    {
        LeanTween.cancel(FishCanvas.gameObject);
        LeanTween.alphaCanvas(FishCanvas.GetComponent<CanvasGroup>(), 0f, .2f);
    }

    public void TriggerLose()
    {
        gameEnded = true;

        BigText.gameObject.active = true;
        BigText.text = "You Couldn't Catch\n The Big One...";
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

        if (gameEnded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!CheckerForHasStartedTheRealGameOnce.instance.hasStartedTheRealGameOnce)
                    SceneManager.LoadScene("SethScene-init");
                else
                    SceneManager.LoadScene("SethScene");
            }
        }
    }
}
