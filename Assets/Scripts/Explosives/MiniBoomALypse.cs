using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoomALypse : MonoBehaviour
{
    [SerializeField]
    private GameObject _papi;
    [SerializeField]
    private GameObject _miniBoom;

    [SerializeField]
    public int _amountOfMiniBooms = 1;

    private GameObject[] _apocalypse;


    public void Start()
    {
        _apocalypse = new GameObject[_amountOfMiniBooms];
        for (int x = 0; x < _amountOfMiniBooms;x++)
        {
            _apocalypse[x] = Instantiate(_miniBoom,new Vector2(0,0), Quaternion.identity);
            _apocalypse[x].transform.parent = _papi.transform;
            _apocalypse[x].SetActive(false);
        }
        StartCoroutine(KeepOnBoomin());
    }

     IEnumerator KeepOnBoomin()
    {
        while(true)
        {
            for (int x = 0; x < _apocalypse.Length; x++)
            {
                _apocalypse[x].transform.localPosition = new Vector2(Random.Range(-2f,2f),Random.Range(-2f,2f));
                _apocalypse[x].SetActive(true);
                yield return new WaitForSeconds(.03f);        
            }
            yield return new WaitForSeconds(.01f);   
        }

    }
}
