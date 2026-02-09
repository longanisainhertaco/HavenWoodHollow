namespace HavenwoodHollow.Creatures
{
    /// <summary>
    /// Bitmask flags representing genetic traits for creature breeding.
    /// Uses C# Flags Enum for efficient bitwise storage and manipulation.
    /// Reference: Plan Section 6.1, Table 3 - Genetic Traits Bitmask Structure.
    /// </summary>
    [System.Flags]
    public enum GeneticTrait
    {
        None            = 0,

        /// <summary>Bit 0 (0x01): Can see in dark without lantern. Recessive.</summary>
        NightVision     = 1 << 0,

        /// <summary>Bit 1 (0x02): Can fly over obstacles. Dominant.</summary>
        Wings           = 1 << 1,

        /// <summary>Bit 2 (0x04): +5 Defense. Dominant.</summary>
        HardenedScale   = 1 << 2,

        /// <summary>Bit 3 (0x08): Attacks apply poison. Recessive.</summary>
        Venomous        = 1 << 3,

        /// <summary>Bit 4 (0x10): Acts as a light source. Recessive.</summary>
        Bioluminescence = 1 << 4,

        /// <summary>Bit 5 (0x20): Invisible in darkness. Ultra-Rare.</summary>
        ShadowForm      = 1 << 5
    }

    /// <summary>
    /// Dominance classification for traits in the Punnett Square simulation.
    /// </summary>
    public enum TraitDominance
    {
        Dominant,
        Recessive,
        UltraRare
    }

    /// <summary>
    /// Static utility for querying trait metadata.
    /// </summary>
    public static class GeneticTraitInfo
    {
        /// <summary>
        /// Returns the dominance classification for a given trait.
        /// </summary>
        public static TraitDominance GetDominance(GeneticTrait trait)
        {
            switch (trait)
            {
                case GeneticTrait.Wings:
                case GeneticTrait.HardenedScale:
                    return TraitDominance.Dominant;
                case GeneticTrait.NightVision:
                case GeneticTrait.Venomous:
                case GeneticTrait.Bioluminescence:
                    return TraitDominance.Recessive;
                case GeneticTrait.ShadowForm:
                    return TraitDominance.UltraRare;
                default:
                    return TraitDominance.Recessive;
            }
        }
    }
}
