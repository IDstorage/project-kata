using UnityEngine;

[System.Serializable]
public class CharacterPhysicsComponent : PhysicsComponent
{
    [Space(10), SerializeField]
    private CharacterController character;

    public override void Update()
    {
        character.Move(CalculateForce());
    }
}