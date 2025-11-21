using UnityEngine;

namespace TanxClone
{
    /// <summary>
    /// Game configuration enums matching the original Amiga Tanx options
    /// </summary>

    public enum WindStrength
    {
        None,
        Light,
        Medium,
        Strong,
        Random
    }

    public enum WindDirection
    {
        Same,      // Fixed direction per game
        Random     // Changes during game
    }

    public enum GravityStrength
    {
        Light,
        Medium,
        Strong,
        Random
    }

    public enum LandscapeType
    {
        Mountains,
        FootHills,
        Random
    }

    public enum ObjectType
    {
        Target,
        Fan,
        Pusher,
        Puller
    }

    /// <summary>
    /// Game settings and constants
    /// </summary>
    [System.Serializable]
    public class GameSettings
    {
        // Wind settings
        public WindStrength windStrength = WindStrength.None;
        public WindDirection windDirection = WindDirection.Same;

        // Gravity settings
        public GravityStrength gravityStrength = GravityStrength.Medium;

        // Landscape settings
        public LandscapeType landscapeType = LandscapeType.FootHills;

        // Object settings
        public bool enableTargets = false;
        public bool enableFans = false;
        public bool enablePushers = false;
        public bool enablePullers = false;

        // Sound settings
        public bool soundEnabled = true;

        // Player names (3 characters each)
        public string player1Name = "P1_";
        public string player2Name = "P2_";

        // Game constants
        public const int MIN_VELOCITY = 0;
        public const int MAX_VELOCITY = 199;
        public const int MIN_ANGLE = -90;
        public const int MAX_ANGLE = 150;

        // Physics constants
        public const float LIGHT_GRAVITY = 4.9f;
        public const float MEDIUM_GRAVITY = 9.8f;  // Earth gravity
        public const float STRONG_GRAVITY = 19.6f;

        // Wind constants (force applied to projectile)
        public const float LIGHT_WIND = 2.0f;
        public const float MEDIUM_WIND = 5.0f;
        public const float STRONG_WIND = 10.0f;

        // Landscape constants
        public const float LANDSCAPE_WIDTH = 1920f;  // 2 screens wide (assuming 960px per screen)
        public const float LANDSCAPE_HEIGHT = 600f;
        public const int TERRAIN_POINTS = 50;  // Number of points to generate terrain

        // Object constants
        public const int MAX_TARGETS = 5;
        public const int MAX_FANS = 3;
        public const int MAX_PUSHERS = 3;
        public const int MAX_PULLERS = 3;
    }
}
