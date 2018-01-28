using UnityEngine;

[System.Serializable]
public class MinMaxRange
{
    public float limitMin = 0f, limitMax = 1f;
    public float rangeStart, rangeEnd;

    public MinMaxRange(float limitMin, float rangeStart, float rangeEnd, float limitMax)
    {
        if (rangeStart < limitMin)
        {
            rangeStart = limitMin;
        }
        if (limitMin > rangeStart)
        {
            limitMin = rangeStart;
        }
        if (rangeEnd > limitMax)
        {
            rangeEnd = limitMax;
        }
        if (limitMax < rangeEnd)
        {
            limitMax = rangeEnd;
        }

        this.limitMin = limitMin;
        this.limitMax = limitMax;

        this.rangeStart = rangeStart;
        this.rangeEnd = rangeEnd;
    }

    public float GetRandomValue()
    {
        return Random.Range(rangeStart, rangeEnd);
    }

    public float GetInverseRandomValue()
    {
        return ((Random.Range(limitMin, limitMax - (rangeEnd - rangeStart))) + rangeEnd) % limitMax;
    }

    public bool IsInsideRange(float value)
    {
        if (value > rangeStart && value < rangeEnd)
            return true;
        else
            return false;
    }
}