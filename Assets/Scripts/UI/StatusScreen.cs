using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TanxClone.Managers;

namespace TanxClone.UI
{
    /// <summary>
    /// Status screen showing match results and player scores
    /// </summary>
    public class StatusScreen : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject statusPanel;

        [Header("Winner Display")]
        [SerializeField] private TextMeshProUGUI winnerText;
        [SerializeField] private Image winnerBackground;

        [Header("Score Display")]
        [SerializeField] private TextMeshProUGUI player1NameText;
        [SerializeField] private TextMeshProUGUI player1ScoreText;
        [SerializeField] private TextMeshProUGUI player2NameText;
        [SerializeField] private TextMeshProUGUI player2ScoreText;

        [Header("Buttons")]
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            // Setup button listeners
            if (playAgainButton != null)
                playAgainButton.onClick.AddListener(OnPlayAgainClicked);

            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);

            // Subscribe to game events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerWin += OnPlayerWin;
            }

            // Hide initially
            Hide();
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerWin -= OnPlayerWin;
            }
        }

        private void OnPlayerWin(int winnerIndex)
        {
            Show(winnerIndex);
        }

        private void Show(int winnerIndex)
        {
            if (statusPanel != null)
                statusPanel.SetActive(true);

            if (GameManager.Instance == null) return;

            GameSettings settings = GameManager.Instance.GetSettings();
            int[] scores = GameManager.Instance.GetPlayerScores();

            // Update winner display
            string winnerName = winnerIndex == 0 ? settings.player1Name : settings.player2Name;
            Color winnerColor = winnerIndex == 0 ? Color.blue : Color.red;

            if (winnerText != null)
            {
                winnerText.text = $"{winnerName} WINS!";
                winnerText.color = winnerColor;
            }

            if (winnerBackground != null)
            {
                winnerBackground.color = new Color(winnerColor.r, winnerColor.g, winnerColor.b, 0.3f);
            }

            // Update score tally
            if (player1NameText != null)
                player1NameText.text = settings.player1Name;

            if (player1ScoreText != null)
                player1ScoreText.text = scores[0].ToString();

            if (player2NameText != null)
                player2NameText.text = settings.player2Name;

            if (player2ScoreText != null)
                player2ScoreText.text = scores[1].ToString();
        }

        private void Hide()
        {
            if (statusPanel != null)
                statusPanel.SetActive(false);
        }

        private void OnPlayAgainClicked()
        {
            Hide();

            // Start new game with same settings
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartNewGame();
            }
        }

        private void OnMainMenuClicked()
        {
            Hide();

            // Return to main menu
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitCurrentGame();
            }

            // Show main menu
            MainMenuUI mainMenu = FindObjectOfType<MainMenuUI>();
            if (mainMenu != null)
            {
                mainMenu.Show();
            }
        }
    }
}
