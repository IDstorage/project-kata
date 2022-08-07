using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class State
{
    public enum Identity
    {
        None,
        PlayerLocomotion,
        PlayerInteraction,
        PlayerAttack,
        PlayerDefense
    }
}
