using UnityEngine;
using UnityEngine.UI;

public class XRaySystem : MonoBehaviour
{
    public static XRaySystem Instance { get; private set; }

    public SecurityLuggage currentLuggage; 
    
    [Header("Image Spawning")]
    public Canvas xRayScreen;
    public float xPositionRange = 0.5f;
    public float yPositionRange = 0.5f;

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
    public void SpawnImagesOnUI()
    {
        ClearXRayScreen();

        for (int i = 0; i < currentLuggage.xRayImages.Count; i++)
        {
            // Create a new Image game object instance
            GameObject newImage = new GameObject("XRayImage" + i, typeof(Image));

            // Set the parent of the new image to be the x-ray screen canvas
            newImage.transform.SetParent(xRayScreen.transform);

            // Get the image's components so it can be set up properly
            Image imageComponent = newImage.GetComponent<Image>();
            RectTransform rectTransform = newImage.GetComponent<RectTransform>();

            // Set the image component's sprite to be the current luggage's x-ray image
            imageComponent.sprite = currentLuggage.xRayImages[i].image;

            // Set the image's position, and rotation on the canvas
            float randomX = Random.Range(-xPositionRange, xPositionRange);
            float randomY = Random.Range(-yPositionRange, yPositionRange);
            rectTransform.localPosition = new Vector3(randomX, randomY, 0);

            float randomRotation = Random.Range(0f, 360f);
            rectTransform.localRotation = Quaternion.Euler(0, 0, randomRotation);

            rectTransform.sizeDelta = new Vector2(currentLuggage.xRayImages[i].image.rect.size.x, currentLuggage.xRayImages[i].image.rect.size.y) * currentLuggage.xRayImages[i].spriteResizeMultiplier;
        }
    }

    public void ClearXRayScreen()
    {
        foreach (Transform child in xRayScreen.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
