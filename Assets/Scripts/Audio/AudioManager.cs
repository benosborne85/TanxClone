using UnityEngine;
using System.Collections.Generic;

namespace TanxClone.Audio
{
    /// <summary>
    /// Audio manager for playing stereo sound effects
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource leftChannel;
        [SerializeField] private AudioSource rightChannel;
        [SerializeField] private AudioSource centerChannel;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip fireSound;
        [SerializeField] private AudioClip explosionSound;
        [SerializeField] private AudioClip targetHitSound;
        [SerializeField] private AudioClip tankMoveSound;
        [SerializeField] private AudioClip menuClickSound;
        [SerializeField] private AudioClip windSound;

        private bool soundEnabled = true;
        private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
        private const int POOL_SIZE = 10;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
                CreateAudioSourcePool();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAudioSources()
        {
            // Create stereo channels if not assigned
            if (leftChannel == null)
            {
                GameObject leftGO = new GameObject("Left Channel");
                leftGO.transform.SetParent(transform);
                leftChannel = leftGO.AddComponent<AudioSource>();
                leftChannel.spatialBlend = 0; // 2D sound
                leftChannel.panStereo = -1; // Full left
            }

            if (rightChannel == null)
            {
                GameObject rightGO = new GameObject("Right Channel");
                rightGO.transform.SetParent(transform);
                rightChannel = rightGO.AddComponent<AudioSource>();
                rightChannel.spatialBlend = 0;
                rightChannel.panStereo = 1; // Full right
            }

            if (centerChannel == null)
            {
                GameObject centerGO = new GameObject("Center Channel");
                centerGO.transform.SetParent(transform);
                centerChannel = centerGO.AddComponent<AudioSource>();
                centerChannel.spatialBlend = 0;
                centerChannel.panStereo = 0; // Center
            }
        }

        private void CreateAudioSourcePool()
        {
            for (int i = 0; i < POOL_SIZE; i++)
            {
                GameObject go = new GameObject($"PooledAudioSource_{i}");
                go.transform.SetParent(transform);
                AudioSource source = go.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 0;
                audioSourcePool.Enqueue(source);
            }
        }

        /// <summary>
        /// Play sound effect at a specific world position with stereo panning
        /// </summary>
        public void PlaySound(AudioClip clip, Vector3 worldPosition, float volume = 1f)
        {
            if (!soundEnabled || clip == null) return;

            // Calculate stereo pan based on world position
            float pan = CalculateStereoPan(worldPosition);

            AudioSource source = GetPooledAudioSource();
            if (source != null)
            {
                source.clip = clip;
                source.volume = volume;
                source.panStereo = pan;
                source.Play();

                // Return to pool after playing
                StartCoroutine(ReturnToPool(source, clip.length));
            }
        }

        /// <summary>
        /// Play fire sound
        /// </summary>
        public void PlayFire(Vector3 position)
        {
            PlaySound(fireSound, position, 0.7f);
        }

        /// <summary>
        /// Play explosion sound
        /// </summary>
        public void PlayExplosion(Vector3 position)
        {
            PlaySound(explosionSound, position, 1f);
        }

        /// <summary>
        /// Play target hit sound
        /// </summary>
        public void PlayTargetHit(Vector3 position)
        {
            PlaySound(targetHitSound, position, 0.8f);
        }

        /// <summary>
        /// Play tank movement sound
        /// </summary>
        public void PlayTankMove(Vector3 position)
        {
            PlaySound(tankMoveSound, position, 0.5f);
        }

        /// <summary>
        /// Play menu click sound (centered)
        /// </summary>
        public void PlayMenuClick()
        {
            if (!soundEnabled || menuClickSound == null) return;

            centerChannel.PlayOneShot(menuClickSound, 0.6f);
        }

        /// <summary>
        /// Play ambient wind sound
        /// </summary>
        public void PlayWindAmbient(float windStrength)
        {
            if (!soundEnabled || windSound == null) return;

            if (!centerChannel.isPlaying || centerChannel.clip != windSound)
            {
                centerChannel.clip = windSound;
                centerChannel.loop = true;
                centerChannel.volume = Mathf.Clamp01(windStrength / 10f) * 0.3f;
                centerChannel.Play();
            }
            else
            {
                centerChannel.volume = Mathf.Clamp01(windStrength / 10f) * 0.3f;
            }
        }

        /// <summary>
        /// Stop wind ambient sound
        /// </summary>
        public void StopWindAmbient()
        {
            if (centerChannel.clip == windSound)
            {
                centerChannel.Stop();
            }
        }

        private float CalculateStereoPan(Vector3 worldPosition)
        {
            Camera mainCam = Camera.main;
            if (mainCam == null) return 0f;

            // Get camera bounds
            float camWidth = mainCam.orthographicSize * 2f * mainCam.aspect;
            float camLeft = mainCam.transform.position.x - camWidth * 0.5f;
            float camRight = mainCam.transform.position.x + camWidth * 0.5f;

            // Normalize position to -1 (left) to 1 (right)
            float normalizedX = (worldPosition.x - camLeft) / camWidth;
            float pan = Mathf.Lerp(-1f, 1f, normalizedX);

            return Mathf.Clamp(pan, -1f, 1f);
        }

        private AudioSource GetPooledAudioSource()
        {
            if (audioSourcePool.Count > 0)
            {
                return audioSourcePool.Dequeue();
            }

            // Create new one if pool is empty
            GameObject go = new GameObject($"DynamicAudioSource");
            go.transform.SetParent(transform);
            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0;
            return source;
        }

        private System.Collections.IEnumerator ReturnToPool(AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (source != null)
            {
                source.Stop();
                audioSourcePool.Enqueue(source);
            }
        }

        public void SetSoundEnabled(bool enabled)
        {
            soundEnabled = enabled;

            if (!enabled)
            {
                // Stop all sounds
                leftChannel.Stop();
                rightChannel.Stop();
                centerChannel.Stop();
            }
        }

        public bool IsSoundEnabled() => soundEnabled;
    }
}
