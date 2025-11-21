using UnityEngine;
using TanxClone.Projectiles;

namespace TanxClone.Objects
{
    /// <summary>
    /// Base class for all interactive objects in the game
    /// </summary>
    public abstract class InteractiveObject : MonoBehaviour
    {
        [SerializeField] protected ObjectType objectType;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        protected bool isActive = true;

        public ObjectType Type => objectType;

        /// <summary>
        /// Apply effect to projectile passing nearby
        /// </summary>
        public abstract void ApplyEffect(Projectile projectile);

        /// <summary>
        /// Check if projectile is in range
        /// </summary>
        protected bool IsInRange(Vector2 projectilePos, float range)
        {
            return Vector2.Distance(transform.position, projectilePos) < range;
        }

        public virtual void Destroy()
        {
            isActive = false;
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Target - Can be destroyed for bonus shot
    /// </summary>
    public class Target : InteractiveObject
    {
        [SerializeField] private float detectionRange = 20f;

        private void Awake()
        {
            objectType = ObjectType.Target;
        }

        public override void ApplyEffect(Projectile projectile)
        {
            // Targets don't affect projectiles
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Projectile"))
            {
                // Target hit - will be handled by projectile
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.yellow; // Flash before destruction
                }
            }
        }
    }

    /// <summary>
    /// Fan - Blows projectiles in a direction
    /// </summary>
    public class Fan : InteractiveObject
    {
        [SerializeField] private float fanStrength = 15f;
        [SerializeField] private float fanRange = 50f;
        [SerializeField] private Vector2 blowDirection = Vector2.right;
        [SerializeField] private Transform fanBlades;
        [SerializeField] private float rotationSpeed = 360f;

        private void Awake()
        {
            objectType = ObjectType.Fan;
            // Randomize fan strength
            fanStrength = Random.Range(10f, 20f);
        }

        private void Update()
        {
            // Rotate fan blades for visual effect
            if (fanBlades != null)
            {
                fanBlades.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            }
        }

        public override void ApplyEffect(Projectile projectile)
        {
            if (!isActive) return;

            Vector2 projPos = projectile.transform.position;
            if (IsInRange(projPos, fanRange))
            {
                // Apply force based on distance (stronger when closer)
                float distance = Vector2.Distance(transform.position, projPos);
                float forceMagnitude = fanStrength * (1f - distance / fanRange);
                projectile.ApplyExternalForce(blowDirection.normalized * forceMagnitude);
            }
        }

        /// <summary>
        /// Set the direction the fan blows
        /// </summary>
        public void SetDirection(Vector2 direction)
        {
            blowDirection = direction.normalized;
            // Rotate sprite to match direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    /// <summary>
    /// Pusher - Pushes projectiles upward
    /// </summary>
    public class Pusher : InteractiveObject
    {
        [SerializeField] private float pushStrength = 25f;
        [SerializeField] private float pushRange = 40f;
        [SerializeField] private float pulseSpeed = 2f;
        private float pulseTimer = 0f;

        private void Awake()
        {
            objectType = ObjectType.Pusher;
        }

        private void Update()
        {
            // Pulsating effect
            pulseTimer += Time.deltaTime * pulseSpeed;
            float scale = 1f + Mathf.Sin(pulseTimer) * 0.2f;
            transform.localScale = Vector3.one * scale;

            // Color pulse (red pyramid)
            if (spriteRenderer != null)
            {
                float colorPulse = (Mathf.Sin(pulseTimer) + 1f) * 0.5f;
                spriteRenderer.color = Color.Lerp(Color.red, Color.yellow, colorPulse);
            }
        }

        public override void ApplyEffect(Projectile projectile)
        {
            if (!isActive) return;

            Vector2 projPos = projectile.transform.position;
            if (IsInRange(projPos, pushRange))
            {
                // Push upward strongly
                projectile.ApplyExternalForce(Vector2.up * pushStrength);
            }
        }
    }

    /// <summary>
    /// Puller - Pulls projectiles downward
    /// </summary>
    public class Puller : InteractiveObject
    {
        [SerializeField] private float pullStrength = 25f;
        [SerializeField] private float pullRange = 40f;
        [SerializeField] private float jawSpeed = 3f;
        private float jawTimer = 0f;

        private void Awake()
        {
            objectType = ObjectType.Puller;
        }

        private void Update()
        {
            // Opening/closing jaws effect
            jawTimer += Time.deltaTime * jawSpeed;
            float jawOpen = (Mathf.Sin(jawTimer) + 1f) * 0.5f;

            // Scale Y to simulate jaw opening
            Vector3 scale = Vector3.one;
            scale.y = 0.7f + jawOpen * 0.6f;
            transform.localScale = scale;

            // Color shift
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(Color.magenta, Color.blue, jawOpen);
            }
        }

        public override void ApplyEffect(Projectile projectile)
        {
            if (!isActive) return;

            Vector2 projPos = projectile.transform.position;
            if (IsInRange(projPos, pullRange))
            {
                // Pull downward strongly
                projectile.ApplyExternalForce(Vector2.down * pullStrength);
            }
        }
    }
}
