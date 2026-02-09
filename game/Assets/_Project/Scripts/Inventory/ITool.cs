using UnityEngine;

namespace HavenwoodHollow.Inventory
{
    /// <summary>
    /// Tool type categories for interaction matching.
    /// </summary>
    public enum ToolType
    {
        None,
        Hoe,
        WateringCan,
        Axe,
        Pickaxe,
        Scythe,
        Sword,
        FishingRod
    }

    /// <summary>
    /// Interface for tools that can act on the world.
    /// Uses composition model instead of deep inheritance.
    /// Reference: Plan Section 3.3.1 - Clean Room Tool Logic.
    /// </summary>
    public interface ITool
    {
        /// <summary>The type of tool (Hoe, Axe, etc.)</summary>
        ToolType Type { get; }

        /// <summary>Tool tier level (Copper=1, Iron=2, Gold=3, Iridium=4).</summary>
        int TierLevel { get; }

        /// <summary>Stamina cost per use.</summary>
        float StaminaCost { get; }

        /// <summary>
        /// Uses the tool at the target grid position.
        /// Performs Physics2D.OverlapCircle or raycast at the target coordinate.
        /// </summary>
        void UseTool(Vector2 targetPosition, GameObject user);
    }
}
