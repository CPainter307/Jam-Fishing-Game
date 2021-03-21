using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    public GameObject brickPrefab;
    public float range = 1f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnBrick());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnBrick()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(range, 3f));
        Instantiate(brickPrefab, transform);
        StartCoroutine(SpawnBrick());
    }
}
