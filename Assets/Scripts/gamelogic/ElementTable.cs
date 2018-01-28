using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EType {
	Fire = 0,
    Metal,
    Wood,
    Earth,
    Water,
    Neutral
}
public enum EResult {
	Nothing=0,
	A_Buff,
	A_Damage,	//a take damage
	B_Buff,
	B_Damage	//b take damage

}

public class ETable {
						//[A-Row,B-Col]
	public static EResult[,] _contatct = {
		{	EResult.Nothing,	EResult.B_Damage,	EResult.A_Buff,		EResult.B_Buff,		EResult.A_Damage,	EResult.B_Damage	},
		{	EResult.A_Damage,	EResult.Nothing,	EResult.B_Damage,	EResult.A_Buff,		EResult.B_Buff,		EResult.B_Damage	},
		{	EResult.B_Buff,		EResult.A_Damage,	EResult.Nothing,	EResult.B_Damage,	EResult.A_Buff,		EResult.B_Damage	},
		{	EResult.B_Buff,		EResult.B_Buff,		EResult.A_Damage,	EResult.Nothing,	EResult.B_Damage,	EResult.B_Damage	},
		{	EResult.B_Damage,	EResult.A_Buff,		EResult.B_Buff,		EResult.A_Damage,	EResult.Nothing,	EResult.B_Damage	},
		{	EResult.A_Damage,	EResult.A_Damage,	EResult.A_Damage,	EResult.A_Damage,	EResult.A_Damage,	EResult.Nothing		}
	};

	public static EResult[] _inverse = {
		EResult.Nothing, EResult.B_Buff, EResult.A_Damage, EResult.A_Buff, EResult.A_Damage
	};
	public static EResult[] _upgradeA = {
		EResult.B_Damage, EResult.Nothing, EResult.Nothing, EResult.B_Buff, EResult.B_Damage
	};
};
/*
public static class ElementTable
{
    public enum FightState
    {
        Annulment = 0,
        Buffed = 1,
        Destroy = 2,
        Nothing = 3
    }

    public enum CollisionState
    {
        DestroydFirst = 0,
        DestroySecond = 1,
        Neutralize = 2,
        Nothing = 3
    }

    public enum ElementType
    {
        Fire = 0,
        Earth = 1,
        Metal = 2,
        Water = 3,
        Wood = 4,
        Neutral = 5
    }
	//TEMP public until refatory
    public static ElementType[] weakness = new ElementType[]
    {
                ElementType.Water,
                ElementType.Wood,
                ElementType.Fire,
                ElementType.Earth,
                ElementType.Metal,
                ElementType.Neutral
    };

    public static ElementType[] fortification = new ElementType[]
    {
                ElementType.Earth,
                ElementType.Metal,
                ElementType.Water,
                ElementType.Wood,
                ElementType.Fire,
                ElementType.Neutral
    };
	*/
    /// <summary>
    /// Function that returns the interaction between projectile and player elements.
    /// </summary>
    /// <param name="attackerElement">The attacker player element.</param>
    /// <param name="otherElement">The other player element.</param>
    /// <param name="attackerBuffed">Is attacker player buffed</param>
    /// <param name="otherBuffed">Is other player buffed</param>
public class Conflic {
    public static EResult GetProjectileResult(EType attackerElement, EType otherElement, bool attackerBuffed = false, bool otherBuffed = false)
    {
		EResult res = ETable._contatct[(int)attackerElement,(int)otherElement];
		if (res == EResult.A_Buff || res == EResult.A_Damage)
			res = EResult.Nothing;
		return res;
    }

    /// <summary>
    /// Function that returns the interaction between player and player elements.
    /// </summary>
    /// <param name="firstElement">The first player element.</param>
    /// <param name="secondElement">The second player element.</param>
    /// <param name="firstElementBuffed">Is first player buffed</param>
    /// <param name="secondElementBuffed">Is second player buffed</param>

    /*public static CollisionState GetCollisionResult(ElementType firstElement, ElementType secondElement, bool firstElementBuffed = false, bool secondElementBuffed = false)
    {
        if (firstElementBuffed)
        {
            if (secondElementBuffed)
            {
                if (weakness[(int)secondElement] == firstElement) //Primeiro ganha
                    return CollisionState.DestroySecond;
                else if (weakness[(int)firstElement] == secondElement) //Segundo ganha
                    return CollisionState.DestroydFirst;
                else //Empate
                    return CollisionState.Neutralize;
            }
            else
            {
                if (weakness[(int)secondElement] == firstElement) //Primeiro ganha
                    return CollisionState.DestroySecond;
                else if (weakness[(int)firstElement] == secondElement) //Segundo ganha
                    return CollisionState.Neutralize;
                else //Empate
                    return CollisionState.DestroySecond;
            }
        }
        else
        {
            if (secondElementBuffed)
            {
                if (weakness[(int)secondElement] == firstElement) //Primeiro ganha
                    return CollisionState.Neutralize;
                else if (weakness[(int)firstElement] == secondElement) //Segundo ganha
                    return CollisionState.DestroydFirst;
                else //Empate
                    return CollisionState.DestroydFirst;
            }
            else
                return CollisionState.Nothing;
        }
    }*/

}
