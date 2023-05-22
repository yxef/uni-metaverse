using UnityEngine;

public class Pickupable : MonoBehaviour, IPickupable
{

    [SerializeField] private Vector3 offset;
    private Rigidbody rb;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Object's reaction to being interacted with, assigns it to the right hand
    public void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);

        // Getting the Right Hand of player
        GameObject playerRightHand = GameObject.FindWithTag("Right Hand");
        HandScript rightHand = playerRightHand.GetComponent<HandScript>();

        if (!rightHand.isHandOccupied)
        {
            rightHand.isHandOccupied = true;
            AssignToHand(playerRightHand);
        }

    }

    public void AssignToHand(GameObject playerRightHand)
    {
        // Changing the objects transform and rotation to match the hand
        gameObject.transform.SetParent(playerRightHand.transform);
        gameObject.transform.position = playerRightHand.transform.position;
        gameObject.transform.rotation = playerRightHand.transform.rotation;

        // Disabling Colliders or you bump into the object
        Collider[] colliders = gameObject.GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        rb.isKinematic = true;

        // offset the object
        transform.position = transform.position + offset;
    }

    public void DropFromHand()
    {
        gameObject.transform.SetParent(null);
        Collider[] colliders = gameObject.GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
        rb.isKinematic = false;

    }

}
