using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    private float xPos = 9f;
    private float Ypos = 8f;
    [SerializeField]
    private float _timeToWait = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        //Spawn an enemy every 5 seconds
        //create a Coroutine of type IEnumerator that allows us to yield events.
        //use a while loop, an infinite game loop.
        //whiles run as long as a condition is true.

    }

    IEnumerator SpawnRoutine()
    {
        //while loop (infinite loop)
            //instantiate enemy prefab
            //yield wait for 5 seconds.
        while(true)
        {            
            float randomX = Random.Range(-xPos,xPos);
            Vector3 posToSpawn = new Vector3(randomX,Ypos,0);
            Instantiate (_enemyPrefab,posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(_timeToWait);
            
        }

    }
}