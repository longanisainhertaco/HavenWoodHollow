namespace HavenwoodHollow.Creatures
{
    /// <summary>
    /// Represents a creature's genome with two sets of alleles (GeneA, GeneB).
    /// The phenotype (expressed traits) is determined by dominance rules.
    /// Reference: Plan Section 6.2 - The Inheritance Algorithm (Punnett Simulator).
    /// </summary>
    [System.Serializable]
    public struct CreatureGenome
    {
        /// <summary>First set of alleles (from one parent).</summary>
        public GeneticTrait GeneA;

        /// <summary>Second set of alleles (from the other parent).</summary>
        public GeneticTrait GeneB;

        /// <summary>Cached trait values to avoid repeated Enum.GetValues allocations.</summary>
        private static readonly GeneticTrait[] AllTraitValues =
            (GeneticTrait[])System.Enum.GetValues(typeof(GeneticTrait));

        public CreatureGenome(GeneticTrait geneA, GeneticTrait geneB)
        {
            GeneA = geneA;
            GeneB = geneB;
        }

        /// <summary>
        /// Returns the phenotype (visually expressed traits) based on dominance rules.
        /// - Dominant traits: expressed if present in either allele.
        /// - Recessive traits: expressed only if present in both alleles.
        /// - Ultra-Rare traits: expressed only if present in both alleles.
        /// </summary>
        public GeneticTrait GetExpressedTraits()
        {
            GeneticTrait expressed = GeneticTrait.None;

            // Check each trait bit using cached values
            foreach (GeneticTrait trait in AllTraitValues)
            {
                if (trait == GeneticTrait.None) continue;

                bool inA = (GeneA & trait) != 0;
                bool inB = (GeneB & trait) != 0;
                TraitDominance dominance = GeneticTraitInfo.GetDominance(trait);

                switch (dominance)
                {
                    case TraitDominance.Dominant:
                        // Expressed if present in either allele
                        if (inA || inB) expressed |= trait;
                        break;
                    case TraitDominance.Recessive:
                    case TraitDominance.UltraRare:
                        // Expressed only if present in both alleles
                        if (inA && inB) expressed |= trait;
                        break;
                }
            }

            return expressed;
        }

        /// <summary>
        /// Checks if a specific trait is expressed in the phenotype.
        /// </summary>
        public bool HasTrait(GeneticTrait trait)
        {
            return (GetExpressedTraits() & trait) != 0;
        }

        /// <summary>
        /// Checks if a specific trait is carried (in at least one allele)
        /// even if not expressed.
        /// </summary>
        public bool CarriesTrait(GeneticTrait trait)
        {
            return ((GeneA | GeneB) & trait) != 0;
        }
    }
}
