using UnityEngine;

namespace TanxClone.Effects
{
    /// <summary>
    /// Simple explosion visual effect
    /// </summary>
    public class ExplosionEffect : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float lifetime = 1f;
        [SerializeField] private float expandSpeed = 2f;
        [SerializeField] private float fadeSpeed = 2f;
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private SpriteRenderer explosionSprite;

        private float timer = 0f;
        private Vector3 initialScale;
        private Color initialColor;

        private void Start()
        {
            if (explosionSprite != null)
            {
                initialScale = transform.localScale;
                initialColor = explosionSprite.color;
            }

            // Play particle system if available
            if (particles != null)
            {
                particles.Play();
            }

            // Play explosion sound
            if (TanxClone.Audio.AudioManager.Instance != null)
            {
                TanxClone.Audio.AudioManager.Instance.PlayExplosion(transform.position);
            }

            // Auto-destroy after lifetime
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            timer += Time.deltaTime;
            float progress = timer / lifetime;

            if (explosionSprite != null)
            {
                // Expand
                transform.localScale = initialScale * (1f + progress * expandSpeed);

                // Fade out
                Color color = initialColor;
                color.a = Mathf.Lerp(1f, 0f, progress * fadeSpeed);
                explosionSprite.color = color;
            }
        }
    }
}
