using UnityEngine;
using UnityEngine.UI;

public class Spotlight : MonoBehaviour
{
    [Header("Spotlight Settings")]
    public Material spotlightMaterial;           // The spotlight material
    public string shaderSpotProperty = "_SpotPos"; // Shader property name in material

    [Header("Movement Settings")]
    [Range(0.01f, 10f)]
    public float moveSpeed = 5f;                // Movement smoothness

    private Material runtimeMaterial;
    private Image uiImage;
    private Vector2 currentPos;
    private Vector2 targetPos;
    private bool moving = false;

    [Header("Canvas Reference")]
    public Canvas canvas;                        // Canvas containing the UI spotlight

    void Awake()
    {
        runtimeMaterial = new Material(spotlightMaterial);
        uiImage = GetComponent<Image>();
        uiImage.material = runtimeMaterial;

        // Start in the center
        currentPos = new Vector2(0.5f, 0.5f);
        targetPos = currentPos;
        runtimeMaterial.SetVector(shaderSpotProperty, currentPos);
    }

    void Update()
    {
        if (!moving) return;

        // Smoothly interpolate
        currentPos = Vector2.Lerp(currentPos, targetPos, Time.deltaTime * moveSpeed);

        // Update shader
        runtimeMaterial.SetVector(shaderSpotProperty, currentPos);

        // Stop when close enough
        if (Vector2.Distance(currentPos, targetPos) < 0.001f)
        {
            currentPos = targetPos;
            moving = false;
            runtimeMaterial.SetVector(shaderSpotProperty, currentPos);
        }
    }

    public void MoveTo(GameObject target)
    {
        if (target == null || canvas == null) return;

        Vector2 viewportPos = Camera.main.WorldToViewportPoint(target.transform.position);

        // Convert viewport (0–1) to normalized canvas coordinates
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 canvasPos = new Vector2(
            viewportPos.x,
            viewportPos.y
        );

        targetPos = canvasPos;
        moving = true;
    }
}