﻿using System.Collections.Generic;
using UnityEngine;


public class HitWall : ASignal<Projectile> { }
public class Projectile : MonoBehaviour
{
    
    private Rigidbody rb;

    public ElementTable.ElementType type;
    public float force;
    public Vector3 direction;
    bool buffed = false;
    public List<ElementModel> listElement;

    Board board;

    public void Initialize(Vector3 direction, ElementTable.ElementType elementType, bool buffed = false ,float force = -1)
    {
        if (force != -1)
            this.force = force;
        type = elementType;
        this.direction = direction;
        this.buffed = buffed;

        ElementModel.ChangeModel(listElement, elementType);
    }

    // Use this for initialization
    void Start()
    {

        rb = GetComponent<Rigidbody>();

        rb.AddForce(direction * force, ForceMode.Impulse);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ElementBending elementBending = other.GetComponent<ElementBending>();
            ElementTable.ElementType otherElement = elementBending.elementType;
            bool otherBuff = elementBending.playerIsBuffed;
            ResolveInteraction(elementBending, ElementTable.GetProjectileResult(type, otherElement,buffed,otherBuff));
            Destroy(this.gameObject);
        }
        else
        {
            Signals.Get<HitWall>().Dispatch(this);
        }
    }

    void ResolveInteraction(ElementBending other, ElementTable.FightState result)
    {
        switch (result)
        {
            case ElementTable.FightState.Annulment:
                break;
            case ElementTable.FightState.Buffed:
                break;
            case ElementTable.FightState.Destroy:
                other.GetComponent<PlayerData>().Die();
                break;
            case ElementTable.FightState.Nothing:
                break;
        }
    }

}
