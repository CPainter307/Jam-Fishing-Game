using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject cloudPrefab;

    public Sprite[] sprites;

    bool startedSpawn = false;

    // Start is called before the first frame update
    void Update()
    {
        if (GameObject.FindObjectOfType<PlayerLineController>().realGameHasStarted && !startedSpawn)
        {
            StartCoroutine(SpawnCloud());
            startedSpawn = true;
        }
    }

    IEnumerator SpawnCloud()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(1, 3));
        GameObject go = Instantiate(cloudPrefab, transform.position, Quaternion.identity);
        go.GetComponent<SpriteRenderer>().sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
        go.transform.position = new Vector3(transform.position.x, transform.position.y, 7);
        Destroy(go, 5.0f);
        StartCoroutine(SpawnCloud());
    }
}
