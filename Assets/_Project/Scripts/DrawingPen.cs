using UnityEngine;

public class DrawingPen : MonoBehaviour, IUseable
{
    [SerializeField] private GameObject PenDotPrefab;     // Object that will be spawned to draw emulate drawing
    [SerializeField] private float drawDistance = 3.5f;

    // The drawing pen use is to draw :)
    public void Use()
    {
        GameObject playerRightHand = GameObject.FindWithTag("Right Hand");
        HandScript rightHand = playerRightHand.GetComponent<HandScript>();
        Transform cameraObject = GameObject.FindWithTag("MainCamera").transform;

        Ray forwardRay = new Ray(cameraObject.position, cameraObject.forward);
        RaycastHit hit;


        if (rightHand.isHandOccupied)
        {
            if (Physics.Raycast(forwardRay, out hit, drawDistance))
            {
                Debug.Log("raycast time");
                GameObject spawnedObject = Instantiate(PenDotPrefab, hit.point, cameraObject.rotation);
            }
        }
    }
}