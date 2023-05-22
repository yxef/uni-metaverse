using UnityEngine;

public class ImpulseChair : MonoBehaviour, IInteractable
{
    private Vector3 impulseDirection;
    [SerializeField] private float impulseStrength = 10f;
    private Rigidbody rb;
    void Start(){
        rb = GetComponent<Rigidbody>();
    }

    public void Interact()
    {
        // Getting the player
        GameObject player = GameObject.FindWithTag("Player");

        // Calculating the direction opposite of where the player currently is
        impulseDirection = transform.position  - player.transform.position;

        // Applying the force multiplied by the impulse strength
        rb.AddForce(impulseDirection.normalized*impulseStrength, ForceMode.Impulse);
    }
}
