using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TanxClone.Managers;
using TanxClone.Tanks;

namespace TanxClone.UI
{
    /// <summary>
    /// In-game HUD with controls for angle, velocity, firing, etc.
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject controlPanel;
        [SerializeField] private CanvasGroup controlPanelGroup;

        [Header("Player Display")]
        [SerializeField] private TextMeshProUGUI currentPlayerText;

        [Header("Angle Controls")]
        [SerializeField] private TextMeshProUGUI angleText;
        [SerializeField] private Slider angleSlider;
        [SerializeField] private Button angleUpButton;
        [SerializeField] private Button angleDownButton;
        [SerializeField] private Button angleUpFastButton;
        [SerializeField] private Button angleDownFastButton;

        [Header("Velocity Controls")]
        [SerializeField] private TextMeshProUGUI velocityText;
        [SerializeField] private Slider velocitySlider;
        [SerializeField] private Button velocityUpButton;
        [SerializeField] private Button velocityDownButton;
        [SerializeField] private Button velocityUpFastButton;
        [SerializeField] private Button velocityDownFastButton;

        [Header("Action Buttons")]
        [SerializeField] private Button fireButton;
        [SerializeField] private Button quitButton;

        [Header("Tank Movement")]
        [SerializeField] private Button tankLeftButton;
        [SerializeField] private Button tankRightButton;

        [Header("Camera Scroll")]
        [SerializeField] private Button scrollLeftButton;
        [SerializeField] private Button scrollRightButton;
        [SerializeField] private Button scrollUpButton;
        [SerializeField] private Button scrollDownButton;

        [Header("Wind Display")]
        [SerializeField] private TextMeshProUGUI windStrengthText;
        [SerializeField] private Image windDirectionArrow;
        [SerializeField] private Slider windMeter;

        [Header("Gravity Display")]
        [SerializeField] private TextMeshProUGUI gravityText;

        private TankController currentTank;
        private Camera mainCamera;
        private bool isPlayerTurn = true;

        private void Start()
        {
            mainCamera = Camera.main;

            // Setup button listeners
            SetupButtons();

            // Hide initially
            Hide();

            // Subscribe to game events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerTurnStart += OnPlayerTurnStart;
                GameManager.Instance.OnGameStart += OnGameStart;
                GameManager.Instance.OnGameEnd += OnGameEnd;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerTurnStart -= OnPlayerTurnStart;
                GameManager.Instance.OnGameStart -= OnGameStart;
                GameManager.Instance.OnGameEnd -= OnGameEnd;
            }
        }

        private void SetupButtons()
        {
            // Angle controls
            if (angleSlider != null)
                angleSlider.onValueChanged.AddListener(OnAngleSliderChanged);

            if (angleUpButton != null)
                angleUpButton.onClick.AddListener(() => AdjustAngle(1));

            if (angleDownButton != null)
                angleDownButton.onClick.AddListener(() => AdjustAngle(-1));

            if (angleUpFastButton != null)
                angleUpFastButton.onClick.AddListener(() => AdjustAngle(5));

            if (angleDownFastButton != null)
                angleDownFastButton.onClick.AddListener(() => AdjustAngle(-5));

            // Velocity controls
            if (velocitySlider != null)
                velocitySlider.onValueChanged.AddListener(OnVelocitySliderChanged);

            if (velocityUpButton != null)
                velocityUpButton.onClick.AddListener(() => AdjustVelocity(1));

            if (velocityDownButton != null)
                velocityDownButton.onClick.AddListener(() => AdjustVelocity(-1));

            if (velocityUpFastButton != null)
                velocityUpFastButton.onClick.AddListener(() => AdjustVelocity(10));

            if (velocityDownFastButton != null)
                velocityDownFastButton.onClick.AddListener(() => AdjustVelocity(-10));

            // Action buttons
            if (fireButton != null)
                fireButton.onClick.AddListener(OnFireClicked);

            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);

            // Tank movement
            if (tankLeftButton != null)
                tankLeftButton.onClick.AddListener(() => MoveTank(-5));

            if (tankRightButton != null)
                tankRightButton.onClick.AddListener(() => MoveTank(5));

            // Camera scroll
            if (scrollLeftButton != null)
                scrollLeftButton.onClick.AddListener(() => ScrollCamera(-50, 0));

            if (scrollRightButton != null)
                scrollRightButton.onClick.AddListener(() => ScrollCamera(50, 0));

            if (scrollUpButton != null)
                scrollUpButton.onClick.AddListener(() => ScrollCamera(0, 50));

            if (scrollDownButton != null)
                scrollDownButton.onClick.AddListener(() => ScrollCamera(0, -50));
        }

        private void Update()
        {
            if (!isPlayerTurn || currentTank == null) return;

            // Update displays
            UpdateAngleDisplay();
            UpdateVelocityDisplay();
            UpdateWindDisplay();
        }

        private void OnGameStart()
        {
            Show();
            UpdateWindDisplay();
            UpdateGravityDisplay();
        }

        private void OnGameEnd()
        {
            Hide();
        }

        private void OnPlayerTurnStart(int playerIndex)
        {
            isPlayerTurn = true;

            if (GameManager.Instance != null)
            {
                currentTank = GameManager.Instance.GetCurrentTank();

                if (currentPlayerText != null && currentTank != null)
                {
                    currentPlayerText.text = $"Player: {currentTank.GetPlayerName()}";
                    currentPlayerText.color = currentTank.GetPlayerNumber() == 1 ? Color.blue : Color.red;
                }

                // Focus camera on current tank
                if (mainCamera != null && currentTank != null)
                {
                    Vector3 camPos = mainCamera.transform.position;
                    camPos.x = currentTank.transform.position.x;
                    mainCamera.transform.position = camPos;
                }
            }

            UpdateAngleDisplay();
            UpdateVelocityDisplay();
            UpdateWindDisplay();
        }

        private void AdjustAngle(float delta)
        {
            if (currentTank == null) return;
            currentTank.AdjustAngle(delta);
            UpdateAngleDisplay();
        }

        private void AdjustVelocity(float delta)
        {
            if (currentTank == null) return;
            currentTank.AdjustVelocity(delta);
            UpdateVelocityDisplay();
        }

        private void OnAngleSliderChanged(float value)
        {
            if (currentTank == null) return;
            currentTank.SetAngle(value);
        }

        private void OnVelocitySliderChanged(float value)
        {
            if (currentTank == null) return;
            currentTank.SetVelocity(value);
        }

        private void MoveTank(float distance)
        {
            if (currentTank == null) return;

            if (distance < 0)
                currentTank.MoveLeft(Mathf.Abs(distance));
            else
                currentTank.MoveRight(distance);
        }

        private void ScrollCamera(float x, float y)
        {
            if (mainCamera == null) return;

            Vector3 camPos = mainCamera.transform.position;
            camPos.x = Mathf.Clamp(camPos.x + x, 0, GameSettings.LANDSCAPE_WIDTH);
            camPos.y = Mathf.Clamp(camPos.y + y, -100, 400);
            mainCamera.transform.position = camPos;
        }

        private void OnFireClicked()
        {
            if (!isPlayerTurn || currentTank == null) return;

            isPlayerTurn = false;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.FireCurrentTank();
            }
        }

        private void OnQuitClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitCurrentGame();
            }
        }

        private void UpdateAngleDisplay()
        {
            if (currentTank == null) return;

            float angle = currentTank.CurrentAngle;

            if (angleText != null)
                angleText.text = $"Angle: {angle:F0}°";

            if (angleSlider != null)
            {
                angleSlider.SetValueWithoutNotify(angle);
            }
        }

        private void UpdateVelocityDisplay()
        {
            if (currentTank == null) return;

            float velocity = currentTank.CurrentVelocity;

            if (velocityText != null)
                velocityText.text = $"Velocity: {velocity:F0}";

            if (velocitySlider != null)
            {
                velocitySlider.SetValueWithoutNotify(velocity);
            }
        }

        private void UpdateWindDisplay()
        {
            if (GameManager.Instance == null) return;

            Vector2 wind = GameManager.Instance.GetCurrentWind();
            int windStrength = GameManager.Instance.GetCurrentWindStrength();

            if (windStrengthText != null)
                windStrengthText.text = $"Wind: {windStrength}";

            if (windMeter != null)
                windMeter.value = windStrength / 9f;

            if (windDirectionArrow != null)
            {
                // Rotate arrow based on wind direction
                float angle = wind.x > 0 ? 0 : 180;
                windDirectionArrow.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        private void UpdateGravityDisplay()
        {
            if (GameManager.Instance == null) return;

            float gravity = GameManager.Instance.GetCurrentGravity();
            string gravityStr = "Medium";

            if (gravity <= GameSettings.LIGHT_GRAVITY)
                gravityStr = "Light";
            else if (gravity >= GameSettings.STRONG_GRAVITY)
                gravityStr = "Strong";

            if (gravityText != null)
                gravityText.text = $"Gravity: {gravityStr}";
        }

        public void Show()
        {
            if (controlPanel != null)
                controlPanel.SetActive(true);

            // Animate panel dropping down
            if (controlPanelGroup != null)
            {
                LeanTween.alphaCanvas(controlPanelGroup, 1f, 0.5f);
            }
        }

        public void Hide()
        {
            if (controlPanel != null)
                controlPanel.SetActive(false);
        }
    }
}
