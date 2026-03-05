using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

    [Header("Interaction")]
    public Transform raycastOrigin;
    public float interactionRange;
    public Transform itemHoldArea;

    public WorldSpaceButton currentButton;
    public GameObject heldItem;
    private Controls controls;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        controls = new Controls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        RaycastForward();
        if(controls.Player.Drop.WasPressedThisFrame() && heldItem != null)
        {
            DropItem();
        }
    }

    private void RaycastForward()
    {
        Ray ray = new Ray(raycastOrigin.position, raycastOrigin.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red);
        Debug.Log("Raycasting forward from " + ray.origin + " in direction " + ray.direction);

        // Interaction
        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            // Cannot interact if holding an item
            if (heldItem != null)
                return;

            // Button Interaction
            if (hit.transform.TryGetComponent<WorldSpaceButton>(out WorldSpaceButton button))
            {
                if (currentButton != button)
                {
                    currentButton = button;
                }
                if (controls.Player.Attack.WasPressedThisFrame())
                {
                    currentButton.Push();
                }
            }
            // Luggage Interaction
            else if (hit.transform.TryGetComponent<SecurityLuggage>(out SecurityLuggage luggage))
            {
                if (controls.Player.Attack.WasPressedThisFrame() && luggage.markedAsContraband)
                {
                    luggage.gameObject.transform.SetParent(itemHoldArea);
                    luggage.gameObject.transform.localPosition = Vector3.zero;
                    luggage.gameObject.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    luggage.rb.useGravity = false;
                    luggage.rb.isKinematic = true;

                    heldItem = luggage.gameObject;
                }
            }
        }
        else
        {
            currentButton = null;
        }
    }

    private void DropItem()
    {
        if (heldItem != null)
        {
            heldItem.transform.SetParent(null);
            if(heldItem.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
            heldItem = null;
        }
    }
}
