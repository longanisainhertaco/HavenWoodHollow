namespace HavenwoodHollow.Core
{
    /// <summary>
    /// Central reference for all confirmed design decisions and game-wide constants.
    /// These values were locked in during the pre-Unity planning phase.
    /// Update this file if any design decisions change.
    /// </summary>
    public static class GameConstants
    {
        // ─────────────────────────────────────────────
        // Q7.1 — Resolution & Pixel Scale (RESOLVED)
        // ─────────────────────────────────────────────

        /// <summary>Tile size in pixels. All tilesets use this grid.</summary>
        public const int TileSize = 16;

        /// <summary>Character sprite width in pixels (1 tile wide).</summary>
        public const int CharacterWidth = 16;

        /// <summary>Character sprite height in pixels (2 tiles tall).</summary>
        public const int CharacterHeight = 32;

        /// <summary>Base render resolution width.</summary>
        public const int BaseResolutionWidth = 320;

        /// <summary>Base render resolution height.</summary>
        public const int BaseResolutionHeight = 180;

        /// <summary>Pixels per Unity unit for sprite imports.</summary>
        public const int PixelsPerUnit = 16;

        // ─────────────────────────────────────────────
        // Q2.1 & Q2.2 — Time & Seasons (RESOLVED)
        // ─────────────────────────────────────────────

        /// <summary>Real-time seconds per in-game day (14 minutes, Stardew Valley standard).</summary>
        public const float DayLengthInSeconds = 840f;

        /// <summary>Number of in-game days per season.</summary>
        public const int DaysPerSeason = 28;

        /// <summary>Total days per in-game year (4 seasons × 28 days).</summary>
        public const int DaysPerYear = DaysPerSeason * 4;

        /// <summary>Hour the day begins (6:00 AM).</summary>
        public const float DayStartHour = 6f;

        // ─────────────────────────────────────────────
        // Q3.1 — Combat Style (RESOLVED)
        // ─────────────────────────────────────────────

        /// <summary>
        /// Combat uses Stardew-style simplified swing.
        /// Single attack button, auto-aim nearest enemy, Physics2D.OverlapCircle hitboxes.
        /// </summary>
        public const string CombatStyle = "Simplified";

        // ─────────────────────────────────────────────
        // Q4.1 — NPC Count (RESOLVED)
        // ─────────────────────────────────────────────

        /// <summary>Minimum number of unique NPCs in the game world.</summary>
        public const int MinNPCCount = 30;

        /// <summary>Maximum/target number of unique NPCs in the game world.</summary>
        public const int MaxNPCCount = 50;

        // ─────────────────────────────────────────────
        // Q6.2 — Creature Utility (RESOLVED)
        // ─────────────────────────────────────────────

        /// <summary>
        /// Creatures serve both combat and farming roles.
        /// Trait expression determines whether a creature excels in combat, farming, or both.
        /// </summary>
        public const string CreatureUtilityMode = "Both";

        // ─────────────────────────────────────────────
        // Q1.1 — Farm Size (RESOLVED)
        // ─────────────────────────────────────────────

        /// <summary>Farm tilemap width in tiles (matching Stardew Valley standard farm).</summary>
        public const int FarmWidth = 80;

        /// <summary>Farm tilemap height in tiles (matching Stardew Valley standard farm).</summary>
        public const int FarmHeight = 65;

        /// <summary>Total plantable tile count on the farm.</summary>
        public const int FarmTotalTiles = FarmWidth * FarmHeight;

        // ─────────────────────────────────────────────
        // Movement (from Plan Section 3.1)
        // ─────────────────────────────────────────────

        /// <summary>Player walking speed in tiles per second.</summary>
        public const float PlayerWalkSpeed = 2.0f;

        /// <summary>Player running speed in tiles per second.</summary>
        public const float PlayerRunSpeed = 5.0f;
    }
}
