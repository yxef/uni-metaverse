using UnityEngine;

public interface IInteractable
{
    void Interact();
}

public interface IPickupable : IInteractable{
    void AssignToHand(GameObject playerRightHand);
    void DropFromHand();
}

public interface IUseable{
    void Use();
}