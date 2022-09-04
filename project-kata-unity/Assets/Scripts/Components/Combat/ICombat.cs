using System.Collections;
using System.Collections.Generic;
using Anomaly;
using UnityEngine;

public interface ICombat
{
    void Attack();
    void Block();
    void Parry();

    void OnHit(CustomBehaviour other, params Collider[] hitParts);
    void OnBlocked(CustomBehaviour other);
    void OnParried(CustomBehaviour other);
}
