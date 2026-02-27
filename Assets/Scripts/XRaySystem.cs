using UnityEngine;
using UnityEngine.UI;

public class XRaySystem : MonoBehaviour
{
    public static XRaySystem Instance { get; private set; }

    public Image displayImage;
    public SecurityLuggage currentLuggage;

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
    }

    public void UpdateXRayImage(Sprite newImage)
    {
        displayImage.sprite = newImage;
    }
}
