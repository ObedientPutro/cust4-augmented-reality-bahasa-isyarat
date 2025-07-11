using UnityEngine;

/// <summary>
/// This script handles pinch-to-zoom functionality for a 3D object in an AR environment.
/// It works with two-finger touch gestures on mobile devices and simulates zoom
/// using the mouse scroll wheel when running in the Unity Editor.
/// </summary>
public class PinchToZoomController : MonoBehaviour
{
    [Tooltip("The speed at which the object zooms in or out. Adjust for sensitivity.")]
    public float zoomSpeed = 0.01f;

    [Tooltip("The minimum allowed scale for the object.")]
    public float minScale = 0.1f;

    [Tooltip("The maximum allowed scale for the object.")]
    public float maxScale = 5.0f;

    private Mesh objectMesh;

    private void Start()
    {
        // First, try to find a SkinnedMeshRenderer, common for animated models from Mixamo.
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null)
        {
            // For SkinnedMeshRenderer, we use the sharedMesh.
            objectMesh = skinnedMeshRenderer.sharedMesh;
        }
        else
        {
            // If no SkinnedMeshRenderer is found, fall back to looking for a MeshFilter for static models.
            MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
            if (meshFilter != null)
            {
                objectMesh = meshFilter.mesh;
            }
        }

        // If after checking both types, we still haven't found a mesh, log an error.
        if (objectMesh == null)
        {
            Debug.LogError("PinchToZoomPivotCompensation: No SkinnedMeshRenderer or MeshFilter found on this GameObject or its children. Pivot compensation will not work.");
            // Disable this script if we can't find a mesh.
            this.enabled = false;
        }
    }

    void Update()
    {
        // --- Mobile Touch Input ---
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchOne.position - touchZero.position).magnitude;

            float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

            // Calculate the desired scale change
            Vector3 scaleChange = new Vector3(deltaMagnitudeDiff, deltaMagnitudeDiff, deltaMagnitudeDiff) * (zoomSpeed / 10);

            // Apply the scale with pivot compensation
            ApplyScaleWithPivotCompensation(scaleChange);
        }

        // --- Editor Mouse Input (for testing) ---
#if UNITY_EDITOR
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            // We multiply by a factor to make scroll wheel zoom more noticeable
            Vector3 scaleChange = new Vector3(scroll, scroll, scroll) * zoomSpeed * 10f;
            ApplyScaleWithPivotCompensation(scaleChange);
        }
#endif
    }

    /// <summary>
    /// Applies scaling to the object and then adjusts its position to keep the center point stationary.
    /// </summary>
    /// <param name="scaleChange">The amount to change the scale by.</param>
    private void ApplyScaleWithPivotCompensation(Vector3 scaleChange)
    {
        if (objectMesh == null) return; // Safety check

        // 1. Store the world position of the mesh's center BEFORE scaling.
        // transform.TransformPoint converts the local mesh center to a world space coordinate.
        Vector3 centerWorldPosBeforeScale = transform.TransformPoint(objectMesh.bounds.center);

        // 2. Calculate the new scale and clamp it within min/max bounds.
        Vector3 newScale = transform.localScale + scaleChange;
        newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
        newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
        newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale);

        // 3. Apply the new scale.
        transform.localScale = newScale;

        // 4. Calculate the world position of the mesh's center AFTER scaling.
        Vector3 centerWorldPosAfterScale = transform.TransformPoint(objectMesh.bounds.center);

        // 5. Calculate how much the center has moved (the displacement).
        Vector3 displacement = centerWorldPosAfterScale - centerWorldPosBeforeScale;

        // 6. Move the object's transform by the INVERSE of the displacement.
        // This moves the center back to its original position.
        transform.position -= displacement;
    }
}