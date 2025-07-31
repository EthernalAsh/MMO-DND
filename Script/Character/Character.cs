using UnityEngine;

public class Character : Entity
{
    [Tooltip("Move speed in feet")]
    public float moveSpeed;

    public Vector3 currentPosition;
    public Vector3 targetPosition;


    public virtual void Move()
    {
        SetTarget();
        MoveToTarget();
    }

    /// <summary>
    /// Safe
    ///1. Corners need to be same level and 50% of the area must be level
    ///2. 65% should be flat
    ///3. no block heigher than the flat area
    ///
    ///Dangerous
    ///2. 50% should be flat
    ///3. no block heigher than the flat area
    /// </summary>
    /// <param name="useQuickPath"></param>
    public virtual void SetTarget(bool useQuickPath = false)
    {

    }

    public void MoveToTarget()
    {

    }

    public void CalculateSafePath()
    {

    }

    public void CalculateQuickPath()
    {

    }
}
