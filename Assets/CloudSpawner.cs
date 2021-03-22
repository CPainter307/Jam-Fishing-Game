using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject cloudPrefab;

    public Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnCloud());
    }

    IEnumerator SpawnCloud()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(1, 3));
        GameObject go = Instantiate(cloudPrefab, transform.position, Quaternion.identity);
        go.GetComponent<SpriteRenderer>().sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
        StartCoroutine(SpawnCloud());
    }
}
