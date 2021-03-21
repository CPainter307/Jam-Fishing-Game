using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerForHasStartedTheRealGameOnce : MonoBehaviour
{
    public bool hasStartedTheRealGameOnce = false;

    public static CheckerForHasStartedTheRealGameOnce instance;

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
    }
}
