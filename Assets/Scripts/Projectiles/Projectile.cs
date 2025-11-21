using UnityEngine;
using TanxClone.Terrain;

namespace TanxClone.Projectiles
{
    /// <summary>
    /// Projectile physics using realistic ballistics
    /// Handles gravity, wind, and special object interactions
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(TrailRenderer))]
    public class Projectile : MonoBehaviour
    {
        private Rigidbody2D rb;
        private TrailRenderer trail;
        private TerrainGenerator terrain;

        [Header("Physics")]
        private float gravity;
        private Vector2 windForce;
        private Vector2 velocity;

        [Header("Settings")]
        [SerializeField] private float explosionRadius = 30f;
        [SerializeField] private GameObject explosionPrefab;

        private bool hasExploded = false;

        public System.Action<Vector2> OnExplosion;
        public System.Action<GameObject> OnTargetHit;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            trail = GetComponent<TrailRenderer>();
        }

        /// <summary>
        /// Fire the projectile with given angle and velocity
        /// </summary>
        public void Fire(float angleDegrees, float speed, float gravityValue, Vector2 wind, TerrainGenerator terrainGen)
        {
            terrain = terrainGen;
            gravity = gravityValue;
            windForce = wind;

            // Convert angle to radians
            float angleRad = angleDegrees * Mathf.Deg2Rad;

            // Calculate initial velocity components
            velocity.x = speed * Mathf.Cos(angleRad);
            velocity.y = speed * Mathf.Sin(angleRad);

            // Set rigidbody to kinematic - we'll handle physics manually for accuracy
            rb.isKinematic = true;
            rb.gravityScale = 0;
        }

        private void FixedUpdate()
        {
            if (hasExploded) return;

            // Apply gravity
            velocity.y -= gravity * Time.fixedDeltaTime;

            // Apply wind force
            velocity += windForce * Time.fixedDeltaTime;

            // Update position
            Vector2 newPosition = rb.position + velocity * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);

            // Check for collisions
            CheckCollisions();
        }

        private void CheckCollisions()
        {
            // Check if hit terrain
            if (terrain != null && terrain.IsOnTerrain(rb.position))
            {
                Explode(false);
                return;
            }

            // Check if out of bounds
            if (rb.position.y < -GameSettings.LANDSCAPE_HEIGHT ||
                rb.position.x < -100 || rb.position.x > GameSettings.LANDSCAPE_WIDTH + 100)
            {
                Explode(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (hasExploded) return;

            // Check what we hit
            if (collision.CompareTag("Tank"))
            {
                Explode(true, collision.gameObject);
            }
            else if (collision.CompareTag("Target"))
            {
                OnTargetHit?.Invoke(collision.gameObject);
                Explode(false);
            }
        }

        private void Explode(bool hitTank, GameObject target = null)
        {
            if (hasExploded) return;
            hasExploded = true;

            // Spawn explosion effect
            if (explosionPrefab != null)
            {
                GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                Destroy(explosion, 2f);
            }

            // Deform terrain
            if (terrain != null)
            {
                terrain.DeformTerrain(rb.position, explosionRadius);
            }

            // Notify game manager
            OnExplosion?.Invoke(rb.position);

            // Destroy projectile
            Destroy(gameObject, 0.1f);
        }

        /// <summary>
        /// Apply external force (for fans, pushers, pullers)
        /// </summary>
        public void ApplyExternalForce(Vector2 force)
        {
            velocity += force;
        }

        /// <summary>
        /// Get current velocity for visual feedback
        /// </summary>
        public Vector2 GetVelocity()
        {
            return velocity;
        }
    }
}
