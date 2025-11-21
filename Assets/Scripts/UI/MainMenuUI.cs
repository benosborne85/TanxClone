using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TanxClone.Managers;

namespace TanxClone.UI
{
    /// <summary>
    /// Main menu and options screen UI
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Main Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject titlePanel;

        [Header("Player Name Inputs")]
        [SerializeField] private TMP_InputField player1NameInput;
        [SerializeField] private TMP_InputField player2NameInput;

        [Header("Wind Settings")]
        [SerializeField] private Toggle[] windStrengthToggles; // None, Light, Medium, Strong, Random
        [SerializeField] private Toggle[] windDirectionToggles; // Same, Random

        [Header("Gravity Settings")]
        [SerializeField] private Toggle[] gravityToggles; // Light, Medium, Strong, Random

        [Header("Landscape Settings")]
        [SerializeField] private Toggle[] landscapeToggles; // Mountains, FootHills, Random

        [Header("Object Settings")]
        [SerializeField] private Toggle targetToggle;
        [SerializeField] private Toggle fanToggle;
        [SerializeField] private Toggle pusherToggle;
        [SerializeField] private Toggle pullerToggle;

        [Header("Sound Settings")]
        [SerializeField] private Toggle soundToggle;

        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button aboutButton;
        [SerializeField] private Button quitButton;

        [Header("About Panel")]
        [SerializeField] private GameObject aboutPanel;
        [SerializeField] private Button aboutCloseButton;

        private GameSettings currentSettings;

        private void Start()
        {
            // Show title first
            if (titlePanel != null)
            {
                titlePanel.SetActive(true);
            }

            // Setup button listeners
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayClicked);

            if (aboutButton != null)
                aboutButton.onClick.AddListener(OnAboutClicked);

            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);

            if (aboutCloseButton != null)
                aboutCloseButton.onClick.AddListener(OnAboutClose);

            // Load default settings
            LoadDefaultSettings();

            // Setup toggle listeners
            SetupToggles();
        }

        private void LoadDefaultSettings()
        {
            currentSettings = new GameSettings();

            // Set player names
            if (player1NameInput != null)
                player1NameInput.text = currentSettings.player1Name;

            if (player2NameInput != null)
                player2NameInput.text = currentSettings.player2Name;

            // Set toggles to default values
            UpdateTogglesFromSettings();
        }

        private void SetupToggles()
        {
            // Wind strength toggles
            for (int i = 0; i < windStrengthToggles.Length; i++)
            {
                int index = i;
                if (windStrengthToggles[i] != null)
                {
                    windStrengthToggles[i].onValueChanged.AddListener((value) =>
                    {
                        if (value) currentSettings.windStrength = (WindStrength)index;
                    });
                }
            }

            // Wind direction toggles
            for (int i = 0; i < windDirectionToggles.Length; i++)
            {
                int index = i;
                if (windDirectionToggles[i] != null)
                {
                    windDirectionToggles[i].onValueChanged.AddListener((value) =>
                    {
                        if (value) currentSettings.windDirection = (WindDirection)index;
                    });
                }
            }

            // Gravity toggles
            for (int i = 0; i < gravityToggles.Length; i++)
            {
                int index = i;
                if (gravityToggles[i] != null)
                {
                    gravityToggles[i].onValueChanged.AddListener((value) =>
                    {
                        if (value) currentSettings.gravityStrength = (GravityStrength)index;
                    });
                }
            }

            // Landscape toggles
            for (int i = 0; i < landscapeToggles.Length; i++)
            {
                int index = i;
                if (landscapeToggles[i] != null)
                {
                    landscapeToggles[i].onValueChanged.AddListener((value) =>
                    {
                        if (value) currentSettings.landscapeType = (LandscapeType)index;
                    });
                }
            }

            // Object toggles
            if (targetToggle != null)
                targetToggle.onValueChanged.AddListener((value) => currentSettings.enableTargets = value);

            if (fanToggle != null)
                fanToggle.onValueChanged.AddListener((value) => currentSettings.enableFans = value);

            if (pusherToggle != null)
                pusherToggle.onValueChanged.AddListener((value) => currentSettings.enablePushers = value);

            if (pullerToggle != null)
                pullerToggle.onValueChanged.AddListener((value) => currentSettings.enablePullers = value);

            // Sound toggle
            if (soundToggle != null)
                soundToggle.onValueChanged.AddListener((value) => currentSettings.soundEnabled = value);
        }

        private void UpdateTogglesFromSettings()
        {
            // Wind strength
            if (windStrengthToggles != null && windStrengthToggles.Length > (int)currentSettings.windStrength)
            {
                windStrengthToggles[(int)currentSettings.windStrength].isOn = true;
            }

            // Wind direction
            if (windDirectionToggles != null && windDirectionToggles.Length > (int)currentSettings.windDirection)
            {
                windDirectionToggles[(int)currentSettings.windDirection].isOn = true;
            }

            // Gravity
            if (gravityToggles != null && gravityToggles.Length > (int)currentSettings.gravityStrength)
            {
                gravityToggles[(int)currentSettings.gravityStrength].isOn = true;
            }

            // Landscape
            if (landscapeToggles != null && landscapeToggles.Length > (int)currentSettings.landscapeType)
            {
                landscapeToggles[(int)currentSettings.landscapeType].isOn = true;
            }

            // Objects
            if (targetToggle != null) targetToggle.isOn = currentSettings.enableTargets;
            if (fanToggle != null) fanToggle.isOn = currentSettings.enableFans;
            if (pusherToggle != null) pusherToggle.isOn = currentSettings.enablePushers;
            if (pullerToggle != null) pullerToggle.isOn = currentSettings.enablePullers;

            // Sound
            if (soundToggle != null) soundToggle.isOn = currentSettings.soundEnabled;
        }

        public void OnTitleClick()
        {
            // Hide title, show main menu
            if (titlePanel != null)
                titlePanel.SetActive(false);

            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(true);
        }

        private void OnPlayClicked()
        {
            // Update player names
            if (player1NameInput != null)
                currentSettings.player1Name = player1NameInput.text.Substring(0, Mathf.Min(3, player1NameInput.text.Length));

            if (player2NameInput != null)
                currentSettings.player2Name = player2NameInput.text.Substring(0, Mathf.Min(3, player2NameInput.text.Length));

            // Check if names changed - reset scores
            GameSettings oldSettings = GameManager.Instance.GetSettings();
            if (oldSettings.player1Name != currentSettings.player1Name ||
                oldSettings.player2Name != currentSettings.player2Name)
            {
                GameManager.Instance.ResetScores();
            }

            // Apply settings
            GameManager.Instance.SetSettings(currentSettings);

            // Start game
            GameManager.Instance.StartNewGame();

            // Hide menu
            gameObject.SetActive(false);
        }

        private void OnAboutClicked()
        {
            if (aboutPanel != null)
            {
                aboutPanel.SetActive(true);
            }
        }

        private void OnAboutClose()
        {
            if (aboutPanel != null)
            {
                aboutPanel.SetActive(false);
            }
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void Show()
        {
            gameObject.SetActive(true);
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(true);
        }
    }
}
