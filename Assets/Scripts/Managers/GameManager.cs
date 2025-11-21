using UnityEngine;
using System.Collections.Generic;
using TanxClone.Terrain;
using TanxClone.Tanks;
using TanxClone.Projectiles;
using TanxClone.Objects;

namespace TanxClone.Managers
{
    /// <summary>
    /// Main game manager handling turn-based gameplay and game state
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private TerrainGenerator terrain;
        [SerializeField] private GameObject tankPrefab;
        [SerializeField] private GameObject targetPrefab;
        [SerializeField] private GameObject fanPrefab;
        [SerializeField] private GameObject pusherPrefab;
        [SerializeField] private GameObject pullerPrefab;

        [Header("Game Settings")]
        [SerializeField] private GameSettings settings = new GameSettings();

        // Game state
        private TankController[] tanks = new TankController[2];
        private int currentPlayerIndex = 0;
        private bool gameInProgress = false;
        private int[] playerScores = new int[2];

        // Physics state
        private float currentGravity;
        private Vector2 currentWind;
        private int currentWindStrength; // 0-9 for display

        // Objects
        private List<InteractiveObject> activeObjects = new List<InteractiveObject>();
        private bool grantExtraShot = false;

        // Events
        public System.Action<int> OnPlayerTurnStart;
        public System.Action<int> OnPlayerWin;
        public System.Action OnGameStart;
        public System.Action OnGameEnd;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Start a new game with current settings
        /// </summary>
        public void StartNewGame()
        {
            CleanupGame();
            GenerateGameWorld();
            SpawnTanks();
            SpawnObjects();
            SetupPhysics();

            gameInProgress = true;
            currentPlayerIndex = Random.Range(0, 2); // Random starting player

            OnGameStart?.Invoke();
            OnPlayerTurnStart?.Invoke(currentPlayerIndex);
        }

        private void GenerateGameWorld()
        {
            if (terrain == null)
            {
                Debug.LogError("Terrain generator not assigned!");
                return;
            }

            LandscapeType landscapeType = settings.landscapeType;
            if (landscapeType == LandscapeType.Random)
            {
                landscapeType = (LandscapeType)Random.Range(0, 2); // Mountains or FootHills
            }

            terrain.GenerateTerrain(landscapeType);
        }

        private void SpawnTanks()
        {
            if (tankPrefab == null)
            {
                Debug.LogError("Tank prefab not assigned!");
                return;
            }

            // Get random positions on terrain for both tanks
            float width = GameSettings.LANDSCAPE_WIDTH;
            float player1X = Random.Range(width * 0.1f, width * 0.3f);
            float player2X = Random.Range(width * 0.7f, width * 0.9f);

            // Spawn player 1 tank
            Vector3 pos1 = new Vector3(player1X, terrain.GetTerrainHeightAt(player1X) + 20f, 0);
            GameObject tank1GO = Instantiate(tankPrefab, pos1, Quaternion.identity);
            tanks[0] = tank1GO.GetComponent<TankController>();
            tanks[0].Initialize(1, settings.player1Name, terrain);

            // Spawn player 2 tank
            Vector3 pos2 = new Vector3(player2X, terrain.GetTerrainHeightAt(player2X) + 20f, 0);
            GameObject tank2GO = Instantiate(tankPrefab, pos2, Quaternion.identity);
            tanks[1] = tank2GO.GetComponent<TankController>();
            tanks[1].Initialize(2, settings.player2Name, terrain);

            // Randomly assign which player goes first
            if (Random.value > 0.5f)
            {
                currentPlayerIndex = 1;
            }
        }

        private void SpawnObjects()
        {
            activeObjects.Clear();

            // Spawn targets
            if (settings.enableTargets)
            {
                SpawnObjectsOfType(targetPrefab, Random.Range(2, GameSettings.MAX_TARGETS));
            }

            // Spawn fans
            if (settings.enableFans)
            {
                SpawnObjectsOfType(fanPrefab, Random.Range(1, GameSettings.MAX_FANS));
            }

            // Spawn pushers
            if (settings.enablePushers)
            {
                SpawnObjectsOfType(pusherPrefab, Random.Range(1, GameSettings.MAX_PUSHERS));
            }

            // Spawn pullers
            if (settings.enablePullers)
            {
                SpawnObjectsOfType(pullerPrefab, Random.Range(1, GameSettings.MAX_PULLERS));
            }
        }

        private void SpawnObjectsOfType(GameObject prefab, int count)
        {
            if (prefab == null) return;

            int attempts = 0;
            int maxAttempts = 100 * count; // Prevent infinite loop
            int spawned = 0;

            while (spawned < count && attempts < maxAttempts)
            {
                attempts++;

                // Random position on landscape
                float x = Random.Range(100f, GameSettings.LANDSCAPE_WIDTH - 100f);
                float y = terrain.GetTerrainHeightAt(x);

                Vector2 pos = new Vector2(x, y + 20f);

                // Check if position is valid (not too close to tanks)
                bool tooClose = false;
                foreach (var tank in tanks)
                {
                    if (tank != null && Vector2.Distance(pos, tank.transform.position) < 100f)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                {
                    GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
                    InteractiveObject interactiveObj = obj.GetComponent<InteractiveObject>();

                    if (interactiveObj != null)
                    {
                        activeObjects.Add(interactiveObj);

                        // Special setup for fans (random direction)
                        if (interactiveObj is Fan fan)
                        {
                            Vector2 randomDir = Random.value > 0.5f ? Vector2.right : Vector2.left;
                            fan.SetDirection(randomDir);
                        }
                    }

                    spawned++;
                }
            }
        }

        private void SetupPhysics()
        {
            // Set gravity
            GravityStrength gravType = settings.gravityStrength;
            if (gravType == GravityStrength.Random)
            {
                gravType = (GravityStrength)Random.Range(0, 3);
            }

            switch (gravType)
            {
                case GravityStrength.Light:
                    currentGravity = GameSettings.LIGHT_GRAVITY;
                    break;
                case GravityStrength.Medium:
                    currentGravity = GameSettings.MEDIUM_GRAVITY;
                    break;
                case GravityStrength.Strong:
                    currentGravity = GameSettings.STRONG_GRAVITY;
                    break;
            }

            // Set wind
            UpdateWind();
        }

        private void UpdateWind()
        {
            WindStrength windStr = settings.windStrength;

            if (windStr == WindStrength.None)
            {
                currentWind = Vector2.zero;
                currentWindStrength = 0;
                return;
            }

            // Determine wind strength
            float strength = 0f;
            if (windStr == WindStrength.Random)
            {
                int random = Random.Range(1, 4); // Light, Medium, or Strong
                windStr = (WindStrength)random;
            }

            switch (windStr)
            {
                case WindStrength.Light:
                    strength = GameSettings.LIGHT_WIND;
                    currentWindStrength = Random.Range(1, 4);
                    break;
                case WindStrength.Medium:
                    strength = GameSettings.MEDIUM_WIND;
                    currentWindStrength = Random.Range(4, 7);
                    break;
                case WindStrength.Strong:
                    strength = GameSettings.STRONG_WIND;
                    currentWindStrength = Random.Range(7, 10);
                    break;
            }

            // Determine wind direction
            float direction = currentWind.x >= 0 ? 1f : -1f;

            // Update direction if random mode
            if (settings.windDirection == WindDirection.Random || currentWind == Vector2.zero)
            {
                direction = Random.value > 0.5f ? 1f : -1f;
            }

            currentWind = new Vector2(direction * strength, 0);
        }

        /// <summary>
        /// Fire current player's tank
        /// </summary>
        public void FireCurrentTank()
        {
            if (!gameInProgress) return;

            TankController currentTank = tanks[currentPlayerIndex];
            if (currentTank == null) return;

            // Update wind if random per turn
            if (settings.windStrength == WindStrength.Random || settings.windDirection == WindDirection.Random)
            {
                UpdateWind();
            }

            // Fire projectile
            Projectile projectile = currentTank.Fire(currentGravity, currentWind);

            if (projectile != null)
            {
                projectile.OnExplosion += OnProjectileExploded;
                projectile.OnTargetHit += OnTargetHit;

                // Apply effects from interactive objects
                StartCoroutine(ApplyObjectEffects(projectile));
            }
        }

        private System.Collections.IEnumerator ApplyObjectEffects(Projectile projectile)
        {
            while (projectile != null)
            {
                foreach (var obj in activeObjects)
                {
                    if (obj != null)
                    {
                        obj.ApplyEffect(projectile);
                    }
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private void OnProjectileExploded(Vector2 position)
        {
            // Check if any tank was hit
            for (int i = 0; i < tanks.Length; i++)
            {
                if (tanks[i] != null && tanks[i].IsHit(position, 30f))
                {
                    // Tank hit! Other player wins
                    int winnerIndex = (i + 1) % 2;
                    playerScores[winnerIndex]++;
                    OnPlayerWin?.Invoke(winnerIndex);
                    gameInProgress = false;
                    return;
                }
            }

            // No hit - next turn (unless extra shot granted)
            if (!grantExtraShot)
            {
                NextTurn();
            }
            else
            {
                grantExtraShot = false;
                OnPlayerTurnStart?.Invoke(currentPlayerIndex); // Same player again
            }
        }

        private void OnTargetHit(GameObject target)
        {
            // Grant extra shot
            grantExtraShot = true;

            // Destroy target
            if (target != null)
            {
                InteractiveObject obj = target.GetComponent<InteractiveObject>();
                if (obj != null)
                {
                    activeObjects.Remove(obj);
                    obj.Destroy();
                }
            }
        }

        private void NextTurn()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % 2;
            OnPlayerTurnStart?.Invoke(currentPlayerIndex);
        }

        private void CleanupGame()
        {
            // Destroy all tanks
            foreach (var tank in tanks)
            {
                if (tank != null)
                {
                    Destroy(tank.gameObject);
                }
            }

            // Destroy all objects
            foreach (var obj in activeObjects)
            {
                if (obj != null)
                {
                    Destroy(obj.gameObject);
                }
            }
            activeObjects.Clear();

            grantExtraShot = false;
        }

        public void QuitCurrentGame()
        {
            gameInProgress = false;
            CleanupGame();
            OnGameEnd?.Invoke();
        }

        public TankController GetCurrentTank() => tanks[currentPlayerIndex];
        public int GetCurrentPlayerIndex() => currentPlayerIndex;
        public float GetCurrentGravity() => currentGravity;
        public Vector2 GetCurrentWind() => currentWind;
        public int GetCurrentWindStrength() => currentWindStrength;
        public int[] GetPlayerScores() => playerScores;
        public GameSettings GetSettings() => settings;
        public void SetSettings(GameSettings newSettings) => settings = newSettings;

        public void ResetScores()
        {
            playerScores[0] = 0;
            playerScores[1] = 0;
        }
    }
}
