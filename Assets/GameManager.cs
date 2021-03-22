using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Canvas GameCanvas;
    public Canvas FishCanvas;
    public Canvas ReelCanvas;
    public GameObject HealthCanvas;
    public CanvasGroup BigFade;
    public UnityEngine.UI.Text BigText;
    public UnityEngine.UI.Text RetryText;

    public UnityEngine.UI.Text LosingText;
    public UnityEngine.UI.Image ReelString;
    public UnityEngine.UI.Image ReelOutside;

    public float minStringScale = .3f;
    public float maxStringScale = 1.0f;

    private bool gameEnded = false;
    private bool gameWon = false;

    public string fishName = "The Big One";
    public Sprite fishSprite;

    public AudioMixer audioMixer;
    public AudioSource music;
    public AudioClip calmTheme;
    public AudioClip extremeTheme;
    public AudioClip victoryTheme;
    float mixerStartTime;
    float audioPitch;

    public GameObject timer;

    public Animator playerAnimator;

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
        gameWon = true;
        if (GameObject.FindObjectOfType<PlayerBehavior>().dead) return;
        timer.GetComponent<GameTimer>().StopTimer();

        GameObject.FindObjectOfType<PlayerBehavior>().godMode = true;

        gameEnded = true;
        SetFishText(fishName);
        SetFishSprite(fishSprite);
        print("Time Taken: " + timer.GetComponent<GameTimer>().timeElapsed);

        FindObjectOfType<CameraController>().EnableSpeedParticles(false);

        Destroy(GameObject.FindObjectOfType<FishBehavior>().gameObject);
        
        music.clip = victoryTheme;
        music.Play();
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
        timer.GetComponent<GameTimer>().StopTimer();
        mixerStartTime = Time.time;

        playerAnimator.SetBool("Dead", true);
        
        gameEnded = true;

        BigText.gameObject.active = true;
        RetryText.gameObject.active = true;
        BigText.text = "You Couldn't Catch\n The Big One...";
        
    }

    public void HideReelUI(bool hide)
    {
        ReelCanvas.enabled = !hide;
        if (HealthCanvas)
            HealthCanvas.SetActive(!hide);
    }

    public void UpdateReelUI(float reelSize, PlayerLineController.State reelState, float reelSpeed)
    {

        float reelInScaled = 15.0f;
        float reelOutScaled = reelSpeed * 1500.0f;

        float reelSpeedScaled = reelSpeed / 1.5f;

        switch (reelState)
        {
            case PlayerLineController.State.ReelingOut:
                ReelOutside.rectTransform.eulerAngles -= new Vector3(0, 0, reelInScaled);
                ReelString.rectTransform.localScale += new Vector3(reelSpeedScaled, reelSpeedScaled);
                break;
            case PlayerLineController.State.ReelingIn:
                ReelOutside.rectTransform.eulerAngles += new Vector3(0, 0, reelOutScaled);
                ReelString.rectTransform.localScale -= new Vector3(reelSpeedScaled, reelSpeedScaled);
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
                    LeanTween.move(ReelString.rectTransform, new Vector3(1.3f, 1f), .05f).setLoopPingPong().setEaseInOutSine();
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
        //For some reason you have to set the font size to 0 and then 28 again
        //otherwise it's stupid big but only the first time you play it
        //whywhywhywhywhywhywhywhywhywhywhywhywhy
        LosingText.fontSize = 0;
        LosingText.fontSize = 28;
        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     TriggerWin();
        // }

        if (gameEnded && !gameWon)
        {
            // Happens even if you win - needs conditional
            audioMixer.GetFloat("musicPitch", out audioPitch);
            float t = Time.time - mixerStartTime;
            Debug.Log("lowering pitch");
            audioMixer.SetFloat("musicPitch", audioPitch - (t * 0.001f));

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!CheckerForHasStartedTheRealGameOnce.instance.hasStartedTheRealGameOnce)
                {
                    audioMixer.SetFloat("musicPitch", 1f);
                    music.clip = calmTheme;
                    SceneManager.LoadScene("SethScene-init");
                }
                else
                {
                    audioMixer.SetFloat("musicPitch", 1f);
                    SceneManager.LoadScene("SethScene");
                }
            }
        }
    }
}
