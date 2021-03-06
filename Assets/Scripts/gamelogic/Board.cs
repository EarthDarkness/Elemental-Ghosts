﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public List<CharacterMovement> _players;
    public List<GameObject> _tilePrefab = new List<GameObject>(5);//5 elements

    public bool _lock = false;//looks interactivity (true for prefab)
    public int _width = 20;
    public int _height = 20;

    public float _thresholdAlign = 0.1f;

    public static float _slowSpeed = 0.5f;
    public static float _fastSpeed = 2.0f;

    public static float _boost = 2.0f;
    public static float _recoveryRate = 80.0f;



    [InspectorReadOnly, SerializeField] // if not serialized this won't be kept
    float[] _dim = new float[2] { 1.0f, 1.0f };

    [InspectorReadOnly, SerializeField] // if not serialized this won't be kept
    public GameObject[,] _map;
    void Awake()
    {
        if (_map == null)
        {

            bool temp = _lock;
            _lock = false;
            Fetch();
            _lock = temp;
        }
        Signals.Get<CharacterCreated>().AddListener(join);
    }
    void Start()
    {
        Signals.Get<PickupElement>().AddListener(PickElement);
        Signals.Get<HitWall>().AddListener(DropElement);
    }

    private void OnDestroy()
    {
        Signals.Get<PickupElement>().RemoveListener(PickElement);
        Signals.Get<HitWall>().RemoveListener(DropElement);
        Signals.Get<CharacterCreated>().RemoveListener(join);

    }

    void Update()
    {
        for (int i = 0; i < _players.Count; ++i)
        {
            if (_players[i].GetComponent<ElementBending>().elementalPickup > 0.0f)
                PickElement(_players[i].GetComponent<ElementBending>());

            if (_players[i].currentDirection == PlayerInput.Direction.Left || _players[i].currentDirection == PlayerInput.Direction.Right)
            {
                _players[i].aligned = (GetAlignX(_players[i].transform.position.x) < _thresholdAlign);
            }
            else if (_players[i].currentDirection == PlayerInput.Direction.Top || _players[i].currentDirection == PlayerInput.Direction.Bottom)
            {
                _players[i].aligned = (GetAlignY(_players[i].transform.position.z) < _thresholdAlign);
            }

            int px = GetTileX(_players[i].transform.position.x);
            int py = GetTileY(_players[i].transform.position.z);

            EType tel = _map[px, py].GetComponent<TileCell>()._elementType;
            EType pel = _players[i].transform.GetComponent<ElementBending>().ElementType;

            if (pel != EType.Neutral)
            {
                EResult res = ETable._contatct[(int)pel, (int)tel];
                if (res == EResult.A_Damage)
                {
                    _players[i].velocity = CharacterMovement.baseVelocity * _slowSpeed;
                    if (_players[i].transform.GetComponent<ElementBending>().buffed)
                        _players[i].velocity *= _boost;
                }
                else if (res == EResult.A_Buff)
                {
                    _players[i].velocity = CharacterMovement.baseVelocity * _fastSpeed;
                    if (_players[i].transform.GetComponent<ElementBending>().buffed)
                        _players[i].velocity *= _boost;
                }
             
            }
            if (Mathf.Abs(_players[i].velocity - CharacterMovement.baseVelocity) > 0.01f)
            {
                if (_players[i].velocity < CharacterMovement.baseVelocity)
                {
                    _players[i].velocity += Time.deltaTime * _recoveryRate;
                    if (_players[i].velocity > CharacterMovement.baseVelocity)
                    {
                        _players[i].velocity = CharacterMovement.baseVelocity;
                    }
                }
                else if (_players[i].velocity > CharacterMovement.baseVelocity)
                {
                    _players[i].velocity -= Time.deltaTime * _recoveryRate;
                    if (_players[i].velocity < CharacterMovement.baseVelocity)
                    {
                        _players[i].velocity = CharacterMovement.baseVelocity;
                    }
                }

            }
        }
    }

    public bool SpawnElement(int x, int y, EType element)
    {
        //Debug.Log(element.ToString());
        //Debug.Log(_map[x, y].GetComponent<TileCell>()._elementType);
        if (_map[x, y].GetComponent<TileCell>()._elementType != EType.Neutral)
            return false;
        //if (_map[x, y].GetComponent<TileCell>().GetComponentInChildren<Collider>() != null)
        //	return false;

        if (_map[x, y].GetComponent<TileCell>()._component.Count > 0)
            return false;

        _map[x, y].GetComponent<TileCell>()._elementType = element;
        _map[x, y].GetComponent<TileCell>()._elementVisual = Instantiate(_tilePrefab[(int)element], _map[x, y].transform);
        return true;
    }


    [EasyButtons.Button()]
    void Fetch()
    {
        if (_lock)
            return;

        float minx = -(_width * _dim[0]) / 2;
        float miny = -(_height * _dim[1]) / 2;

        if (_map == null)
            _map = new GameObject[_width, _height];

        for (int j = 0; j < _height; ++j)
        {
            GameObject line;
            if (transform.childCount > j)
            {
                line = transform.GetChild(j).gameObject;
                for (int i = 0; i < _width; ++i)
                {
                    if (line.transform.childCount > i)
                    {
                        _map[i, j] = line.transform.GetChild(i).gameObject;
                        //_map[i, j].GetComponent<TileCell>().Reset();
                    }
                    else
                    {
                        _map[i, j] = new GameObject();
                        _map[i, j].transform.parent = line.transform;
                        _map[i, j].transform.position = new Vector3(minx + _dim[0] * i, 0.0f, miny + _dim[1] * j);
                        _map[i, j].AddComponent<TileCell>();
                        _map[i, j].name = "Col " + i.ToString();
                    }
                }
            }
            else
            {
                line = new GameObject();
                line.name = "Row " + j.ToString();
                line.transform.parent = transform;
                for (int i = 0; i < _width; ++i)
                {
                    _map[i, j] = new GameObject();
                    _map[i, j].transform.parent = line.transform;
                    _map[i, j].transform.position = new Vector3(minx + _dim[0] * i, 0.0f, miny + _dim[1] * j);
                    _map[i, j].AddComponent<TileCell>();
                    _map[i, j].name = "Col " + i.ToString();
                }
            }
        }

    }

    [EasyButtons.Button()]
    void Deploy()
    {
        if (_lock)
            return;
        float minx = -(_width * _dim[0]) / 2;
        float miny = -(_height * _dim[1]) / 2;
        _map = new GameObject[_width, _height];

        for (int j = 0; j < _height; ++j)
        {
            GameObject line = new GameObject();
            line.name = "Row " + j.ToString();
            line.transform.parent = transform;
            for (int i = 0; i < _width; ++i)
            {
                _map[i, j] = new GameObject();
                _map[i, j].transform.parent = line.transform;
                _map[i, j].transform.position = new Vector3(minx + _dim[0] * i, 0.0f, miny + _dim[1] * j);
                _map[i, j].AddComponent<TileCell>();
                _map[i, j].name = "Col " + i.ToString();
            }
        }
        IconControl icc = this.transform.GetComponent<IconControl>();
        if (icc != null)
        {
            icc._container = this;
            icc.Deploy(_width, _height, _dim[0], _dim[1]);
        }


    }
    [EasyButtons.Button()]
    void Reset()
    {
        if (_lock)
            return;
        if (_map == null)
            return;
        for (int j = 0; j < _height; ++j)
        {
            for (int i = 0; i < _width; ++i)
            {
                //if (_map[i, j] == null)
                //	continue;
                _map[i, j].GetComponent<TileCell>()._component.Clear();
                DestroyImmediate(_map[i, j]);
                _map[i, j] = null;
            }
        }
        /*for(int i=transform.childCount-1;i>=0;++i) {
			DestroyImmediate(transform.GetChild(i).gameObject);
		}*/

        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
        }

    }

    [EasyButtons.Button()]
    void ToogleLock()
    {
        _lock = !_lock;
        for (int j = 0; j < _height; ++j)
        {
            for (int i = 0; i < _width; ++i)
            {
                _map[i, j].GetComponent<TileCell>()._lock = _lock;
            }
        }

    }

    int GetTileX(float x)
    {
        float minx = -(_width * _dim[0]) / 2;
        float gx = x - minx;
        return Mathf.RoundToInt(gx);
    }
    int GetTileY(float y)
    {
        float miny = -(_height * _dim[1]) / 2;
        float gy = y - miny;
        return Mathf.RoundToInt(gy);
    }
    float GetAlignX(float x)
    {
        float minx = -(_width * _dim[0]) / 2;
        float gx = x - minx;
        return Mathf.Abs(gx - Mathf.Round(gx));
    }
    float GetAlignY(float y)
    {
        float miny = -(_height * _dim[1]) / 2;
        float gy = y - miny;
        return Mathf.Abs(gy - Mathf.Round(gy));
    }

    void PickElement(ElementBending bend)
    {
        if (bend.ElementType != EType.Neutral)
            return;

        int gix = GetTileX(bend.transform.position.x);
        int giy = GetTileY(bend.transform.position.z);

        TileCell tileCell = _map[gix, giy].GetComponent<TileCell>();

        EType ele = tileCell._elementType;
        tileCell._elementType = EType.Neutral;
        bend.ElementType = ele;

        if (ele != EType.Neutral)
            bend.animator.SetTrigger("Receive");

        if (tileCell._elementVisual)
        {
            if (tileCell._elementVisual.GetComponent<ElementAnimation>())
                tileCell._elementVisual.GetComponent<ElementAnimation>().InstantiateAnimation();
            tileCell._elementVisual = null;
        }

        if (bend.ElementType != EType.Neutral)
            bend.elementalPickup = 0.0f;
    }
    void DropElement(Projectile shot)
    {
        int gix = GetTileX(shot.transform.position.x);
        int giy = GetTileY(shot.transform.position.z);

        if (gix < 0)
            gix = 0;
        if (gix >= _width)
            gix = _width - 1;
        if (giy < 0)
            giy = 0;
        if (giy >= _height)
            giy = _height - 1;

        int dx = 0;
        int dy = 0;

        //Debug.Log(gix + " " + giy);

        if (Mathf.Abs(shot.direction.x) > Mathf.Abs(shot.direction.z))
        {
            if (shot.direction.x > 0.0f)
            {
                dx -= 1;
            }
            else
            {
                dx += 1;
            }
        }
        else
        {
            if (shot.direction.z > 0.0f)
            {
                dy -= 1;
            }
            else
            {
                dy += 1;
            }
        }

        int nx = gix;
        int ny = giy;
        //Debug.Log((_map[nx, ny].GetComponent<TileCell>() != null));
        while (_map[nx, ny].GetComponent<TileCell>()._component.Count > 0)
        {
            nx += dx;
            ny += dy;
        }
        //Debug.Log(nx + " " + ny);
        TileCell tc = _map[nx, ny].GetComponent<TileCell>();

        EType nElement = shot.type;
        EType cElement = tc._elementType;
        --Spawner._count;
        EResult res = ETable._contatct[(int)cElement, (int)nElement];
        if (cElement == EType.Neutral)
        {
            tc._elementType = nElement;
            tc._elementVisual = Instantiate(_tilePrefab[(int)nElement], tc.transform);
        }
        else if (res == EResult.A_Damage)
        {
            tc._elementType = nElement;
            Destroy(tc._elementVisual);
            tc._elementVisual = Instantiate(_tilePrefab[(int)nElement], tc.transform);
        }
        else if (res == EResult.B_Damage)
        {
            //stay same
        }
        else
        {
            Destroy(tc._elementVisual);//no element
            tc._elementVisual = null;
            tc._elementType = EType.Neutral;//no element
            --Spawner._count;
        }
        //tc._elementType = nElement;
        //tc._elementVisual = Instantiate(_tilePrefab[nElement],_map[nx, ny].transform);
        Destroy(shot.transform.gameObject);
    }

    void join(CharacterMovement cm)
    {
        _players.Add(cm);
    }

}



