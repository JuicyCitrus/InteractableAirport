using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [Header("Interaction")]
    public Transform raycastOrigin;
    public float interactionRange;

    private WorldSpaceButton currentButton;

    private Controls controls;

    private void Start()
    {
        controls = new Controls();
        controls.Enable();
    }

    private void Update()
    {
        RaycastForward();
    }
    private void RaycastForward()
    {
        Ray ray = new Ray(raycastOrigin.position, raycastOrigin.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red);

        // Button Interaction
        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            if (hit.transform.TryGetComponent<WorldSpaceButton>(out WorldSpaceButton button))
            {
                if (currentButton != button)
                {
                    currentButton = button;
                }
                if (controls.Player.Interact.WasPressedThisFrame())
                {
                    currentButton.Push();
                }
            }
        }
        else
        {
            currentButton = null;
        }
    }
}
