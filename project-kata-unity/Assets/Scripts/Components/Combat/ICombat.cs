using System.Collections;
using System.Collections.Generic;
using Anomaly;
using UnityEngine;

public interface ICombat
{
    void Attack();
    void Block(CustomBehaviour other);
    void Parry(CustomBehaviour other);

    void OnHit(CustomBehaviour other, params Collider[] hitParts);
    void OnBlocked(CustomBehaviour other);
    void OnParried(CustomBehaviour other);
}
