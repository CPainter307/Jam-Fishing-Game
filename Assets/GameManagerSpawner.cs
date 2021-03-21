using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerSpawner : MonoBehaviour
{
    public GameObject gameManagerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectOfType<CheckerForHasStartedTheRealGameOnce>() != null) return;

        GameObject go = Instantiate(gameManagerPrefab);
        DontDestroyOnLoad(go);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
