using UnityEngine;

public class InteractableChair : MonoBehaviour, IInteractable
{
    //[SerializeField] private float moveDuration = 1f;   // Duration of the movement
    //private float elapsedTime = 0f; // Time Passed since start of movement

    public void Interact()
    {
        Transform playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>(); // Getting the player
        //Debug.Log("Object "+ player.name + " Position: " + player.transform.position);
        //Debug.Log("Object "+ gameObject.name + " Position: " + gameObject.transform.position);
        playerTransform.position = transform.position;
    }
}
