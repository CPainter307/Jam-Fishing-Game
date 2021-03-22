using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public RectTransform title;

    GameObject currentButton;
    EventSystem eventSystem;

    // Start is called before the first frame update
    public void PlayGame()
    {
        SceneManager.LoadScene("SethScene-init");
    }

    private void Start() {
        RotStart();
        LeanTween.scale(title.gameObject, new Vector3(1.3f, 1.3f, 1.3f), .4f).setLoopPingPong().setEaseInOutSine();
    }

    private void RotStart() {
        LeanTween.rotateZ(title.gameObject, 5f, .5f).setOnComplete(Rot).setEaseInOutSine();
    }

    void Rot()
    {
        LeanTween.rotateZ(title.gameObject, -5f, .5f).setOnComplete(RotStart).setEaseInOutSine();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0))
            EventSystem.current.SetSelectedGameObject(GameObject.FindObjectOfType<UnityEngine.UI.Button>().gameObject);
    }
}
