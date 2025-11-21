using UnityEngine;
using TanxClone.Terrain;
using TanxClone.Projectiles;

namespace TanxClone.Tanks
{
    /// <summary>
    /// Controls tank movement, aiming, and firing
    /// </summary>
    public class TankController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform barrelPivot;
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private SpriteRenderer tankSprite;
        [SerializeField] private SpriteRenderer barrelSprite;

        [Header("Settings")]
        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private float tankWidth = 20f;

        private TerrainGenerator terrain;
        private int playerNumber;
        private string playerName;

        // Current aiming parameters
        private float currentAngle = 45f;
        private float currentVelocity = 100f;

        // Tank position tracking
        private float minX;
        private float maxX;
        private bool canMoveLeft = true;
        private bool canMoveRight = true;

        public float CurrentAngle => currentAngle;
        public float CurrentVelocity => currentVelocity;

        private void Update()
        {
            // Stick tank to terrain
            StickToTerrain();
            UpdateBarrelRotation();
        }

        public void Initialize(int player, string name, TerrainGenerator terrainGen)
        {
            playerNumber = player;
            playerName = name;
            terrain = terrainGen;

            // Set tank color based on player
            if (tankSprite != null)
            {
                tankSprite.color = player == 1 ? Color.blue : Color.red;
            }

            // Initialize position boundaries
            FindMovementBoundaries();
        }

        /// <summary>
        /// Set the aiming angle (-90 to 150 degrees)
        /// </summary>
        public void SetAngle(float angle)
        {
            currentAngle = Mathf.Clamp(angle, GameSettings.MIN_ANGLE, GameSettings.MAX_ANGLE);
        }

        /// <summary>
        /// Set the firing velocity (0 to 199)
        /// </summary>
        public void SetVelocity(float velocity)
        {
            currentVelocity = Mathf.Clamp(velocity, GameSettings.MIN_VELOCITY, GameSettings.MAX_VELOCITY);
        }

        /// <summary>
        /// Adjust angle by increment
        /// </summary>
        public void AdjustAngle(float delta)
        {
            SetAngle(currentAngle + delta);
        }

        /// <summary>
        /// Adjust velocity by increment
        /// </summary>
        public void AdjustVelocity(float delta)
        {
            SetVelocity(currentVelocity + delta);
        }

        /// <summary>
        /// Move tank left
        /// </summary>
        public void MoveLeft(float distance)
        {
            if (!canMoveLeft) return;

            Vector3 newPos = transform.position + Vector3.left * distance;

            // Check if still within movement boundaries
            if (newPos.x >= minX)
            {
                transform.position = newPos;
                FindMovementBoundaries();
            }
        }

        /// <summary>
        /// Move tank right
        /// </summary>
        public void MoveRight(float distance)
        {
            if (!canMoveRight) return;

            Vector3 newPos = transform.position + Vector3.right * distance;

            // Check if still within movement boundaries
            if (newPos.x <= maxX)
            {
                transform.position = newPos;
                FindMovementBoundaries();
            }
        }

        /// <summary>
        /// Fire projectile with current angle and velocity
        /// </summary>
        public Projectile Fire(float gravity, Vector2 wind)
        {
            if (projectilePrefab == null || firePoint == null)
            {
                Debug.LogError("Projectile prefab or fire point not set!");
                return null;
            }

            // Instantiate projectile at fire point
            GameObject projGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectile = projGO.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.Fire(currentAngle, currentVelocity, gravity, wind, terrain);
            }

            return projectile;
        }

        private void UpdateBarrelRotation()
        {
            if (barrelPivot != null)
            {
                // Flip barrel based on player side
                float displayAngle = currentAngle;
                if (playerNumber == 2)
                {
                    // Player 2 tank faces left, so mirror the angle
                    displayAngle = 180f - currentAngle;
                }

                barrelPivot.rotation = Quaternion.Euler(0, 0, displayAngle);
            }
        }

        private void StickToTerrain()
        {
            if (terrain == null) return;

            float terrainHeight = terrain.GetTerrainHeightAt(transform.position.x);
            Vector3 pos = transform.position;
            pos.y = terrainHeight + tankWidth * 0.5f; // Offset by half tank height
            transform.position = pos;
        }

        private void FindMovementBoundaries()
        {
            if (terrain == null) return;

            float currentX = transform.position.x;
            float currentY = transform.position.y;
            float tolerance = 2f; // Height tolerance for "flat" terrain

            // Find left boundary
            minX = currentX;
            for (float x = currentX - 1f; x >= 0; x -= 1f)
            {
                float heightDiff = Mathf.Abs(terrain.GetTerrainHeightAt(x) - currentY);
                if (heightDiff > tolerance)
                {
                    minX = x + 1f;
                    break;
                }
                minX = x;
            }

            // Find right boundary
            maxX = currentX;
            for (float x = currentX + 1f; x <= GameSettings.LANDSCAPE_WIDTH; x += 1f)
            {
                float heightDiff = Mathf.Abs(terrain.GetTerrainHeightAt(x) - currentY);
                if (heightDiff > tolerance)
                {
                    maxX = x - 1f;
                    break;
                }
                maxX = x;
            }

            canMoveLeft = currentX > minX + tankWidth;
            canMoveRight = currentX < maxX - tankWidth;
        }

        /// <summary>
        /// Check if tank was hit by projectile
        /// </summary>
        public bool IsHit(Vector2 position, float radius)
        {
            return Vector2.Distance(transform.position, position) < radius;
        }

        public int GetPlayerNumber() => playerNumber;
        public string GetPlayerName() => playerName;
    }
}
