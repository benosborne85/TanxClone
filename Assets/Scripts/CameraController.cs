using UnityEngine;

namespace TanxClone
{
    /// <summary>
    /// Camera controller for scrolling across the 2-screen-wide landscape
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float scrollSpeed = 100f;
        [SerializeField] private float edgeScrollThreshold = 50f;
        [SerializeField] private bool enableEdgeScrolling = true;
        [SerializeField] private bool enableKeyboardScrolling = true;

        [Header("Bounds")]
        [SerializeField] private float minX = 0f;
        [SerializeField] private float maxX = 1920f;
        [SerializeField] private float minY = -100f;
        [SerializeField] private float maxY = 600f;

        private Camera cam;

        private void Awake()
        {
            cam = GetComponent<Camera>();

            // Set bounds based on landscape
            maxX = GameSettings.LANDSCAPE_WIDTH;
            maxY = GameSettings.LANDSCAPE_HEIGHT;

            // Set camera to orthographic for 2D
            cam.orthographic = true;
            cam.orthographicSize = 300f; // Adjust based on desired zoom
        }

        private void Update()
        {
            HandleKeyboardScrolling();
            HandleEdgeScrolling();
        }

        private void HandleKeyboardScrolling()
        {
            if (!enableKeyboardScrolling) return;

            Vector3 movement = Vector3.zero;

            // Arrow keys or WASD
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                movement.x -= scrollSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                movement.x += scrollSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                movement.y += scrollSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                movement.y -= scrollSpeed * Time.deltaTime;

            MoveCamera(movement);
        }

        private void HandleEdgeScrolling()
        {
            if (!enableEdgeScrolling) return;

            Vector3 mousePos = Input.mousePosition;
            Vector3 movement = Vector3.zero;

            // Check if mouse is at screen edges
            if (mousePos.x < edgeScrollThreshold)
                movement.x -= scrollSpeed * Time.deltaTime;
            else if (mousePos.x > Screen.width - edgeScrollThreshold)
                movement.x += scrollSpeed * Time.deltaTime;

            if (mousePos.y < edgeScrollThreshold)
                movement.y -= scrollSpeed * Time.deltaTime;
            else if (mousePos.y > Screen.height - edgeScrollThreshold)
                movement.y += scrollSpeed * Time.deltaTime;

            MoveCamera(movement);
        }

        /// <summary>
        /// Move camera by offset with bounds checking
        /// </summary>
        public void MoveCamera(Vector3 offset)
        {
            Vector3 newPos = transform.position + offset;

            // Clamp to bounds
            newPos.x = Mathf.Clamp(newPos.x, minX + cam.orthographicSize * cam.aspect,
                                   maxX - cam.orthographicSize * cam.aspect);
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

            transform.position = newPos;
        }

        /// <summary>
        /// Focus camera on a specific position
        /// </summary>
        public void FocusOn(Vector3 position, float smoothTime = 0.5f)
        {
            Vector3 targetPos = position;
            targetPos.z = transform.position.z;

            // Clamp to bounds
            targetPos.x = Mathf.Clamp(targetPos.x, minX + cam.orthographicSize * cam.aspect,
                                      maxX - cam.orthographicSize * cam.aspect);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

            if (smoothTime > 0)
            {
                LeanTween.move(gameObject, targetPos, smoothTime).setEaseInOutQuad();
            }
            else
            {
                transform.position = targetPos;
            }
        }

        /// <summary>
        /// Scroll camera to position instantly
        /// </summary>
        public void ScrollTo(float x, float y)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(x, minX + cam.orthographicSize * cam.aspect,
                               maxX - cam.orthographicSize * cam.aspect);
            pos.y = Mathf.Clamp(y, minY, maxY);
            transform.position = pos;
        }

        public void SetEdgeScrolling(bool enabled)
        {
            enableEdgeScrolling = enabled;
        }

        public void SetKeyboardScrolling(bool enabled)
        {
            enableKeyboardScrolling = enabled;
        }
    }
}
