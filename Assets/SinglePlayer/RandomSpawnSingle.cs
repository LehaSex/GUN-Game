using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnSingle : MonoBehaviour
{
    public LootSpawnedEvent LootSpawn;
    private Vector3 Min;
    private  Vector3 Max;
    private  float _xAxis;
    private  float _yAxis;
    private  float _zAxis;
    private IEnumerator coroutine;
    private Vector3 _randomPosition ;
    public GameObject WeaponCrate;
    private int[] Values = {10, 20, 25, 30};
    
    void Start()
    {
        StartSpawner();
    }

    public void StartSpawner()
    {
        SetRanges();
        coroutine = SpawnCrate();
        StartCoroutine(coroutine);
    }

    public void StopSpawner()
    {
        StopCoroutine(coroutine);
    }

    public IEnumerator SpawnCrate()
    {
        while (true)
        {
            yield return new WaitForSeconds(GetRandomValue());
            _xAxis = Random.Range(Min.x, Max.x);
            _yAxis = Random.Range(Min.y, Max.y);
            _zAxis = -5.885f;
            _randomPosition = new Vector3(_xAxis, _yAxis, _zAxis );
            var crate = Instantiate(WeaponCrate, _randomPosition, transform.rotation * Quaternion.Euler (0f, 180f, 0f));
            Destroy(crate, 10f);
        }
    }

    int GetRandomValue()
    {
    return Values[Random.Range(0, Values.Length)];
    }

    private void SetRanges()
    {
         Min = new Vector2(-1.34f, 0.75f);
         Max = new Vector2(1.39f, 1.241f); 
    }
}
