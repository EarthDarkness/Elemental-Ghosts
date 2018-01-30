using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    Board _map = null;

    public static int _maxElements = 8;
    int _minElements = 4;
    public static int _count = 0;

    float _baseDelay = 4.0f;
    float _cooldown = 0.0f;
    // Use this for initialization
    void Start()
    {
        _map = transform.GetComponent<Board>();

        // reset static variables
        _count = 0;
        _maxElements = 8;


    }

    // Update is called once per frame
    void Update()
    {
        if (_count < _minElements)
        {
            for (int i = _count; i < _minElements; ++i)
            {
                spawn();
            }
        }
        if (_count >= _maxElements)
            return;
        if (_cooldown <= 0.0f)
        {
            spawn();
        }
        _cooldown -= Time.deltaTime;
    }
    void spawn()
    {
        float rx = Random.value * ((float)_map._width - 0.5f);
        float ry = Random.value * ((float)_map._height - 0.5f);

        int re = Random.Range(0, 5);
        if (_map.SpawnElement(Mathf.FloorToInt(rx), Mathf.FloorToInt(ry), (EType)re))
            ++_count;



        _cooldown = _baseDelay;
    }
}
