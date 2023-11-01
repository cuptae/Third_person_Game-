using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] monster;
    public Transform[] spawnPoints;
    public float spawnTimer=20.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private IEnumerator Monsterspawn()
    {

        yield return null;
    }
    private IEnumerator Spawntimerstart()
    {
        yield return new WaitForSeconds(spawnTimer);
    }
}
