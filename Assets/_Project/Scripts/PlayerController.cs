using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private Transform mainCamera;
    [SerializeField] public Transform rightHand;
    [SerializeField][Min(1)] private float rayDistance = 5f;
    [SerializeField] private InputActionReference interactInput, dropInput, useInput;
    private RaycastHit hit;
    void Start()
    {
        useInput.action.performed += Use;
        interactInput.action.performed += Interact;
        dropInput.action.performed += Drop;

    }

    private void Use(InputAction.CallbackContext obj){
        HandScript rightHandScript = rightHand.GetComponent<HandScript>();
        IUseable objectInHand;

        if (rightHandScript.isHandOccupied)
        {
            objectInHand = rightHand.GetComponentInChildren<IUseable>();
            objectInHand.Use();
        }
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        Ray forwardRay = new Ray(mainCamera.position, mainCamera.forward);
        Debug.DrawRay(forwardRay.origin, forwardRay.direction * rayDistance, Color.green, 1);

        if (Physics.Raycast(forwardRay, out hit, rayDistance, interactableLayerMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    private void Drop(InputAction.CallbackContext obj)
    {
        HandScript rightHandScript = rightHand.GetComponent<HandScript>();
        Pickupable objectInHand;

        if (rightHandScript.isHandOccupied)
        {
            objectInHand = rightHand.GetComponentInChildren<Pickupable>();
            objectInHand.DropFromHand();
            rightHandScript.isHandOccupied = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Resets old raycast
        if (hit.collider != null)
        {
            hit.collider.GetComponent<Highlight>().ToggleHighlight(false);
        }
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, rayDistance, interactableLayerMask))
        {
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
        }
    }

    // Debug Function - Draws sphere gizmo at raycas hit position
    void OnDrawGizmos()
    {
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, rayDistance, interactableLayerMask))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, 0.005f);
        }
    }
}
