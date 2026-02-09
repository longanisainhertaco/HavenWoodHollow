using UnityEngine;

namespace HavenwoodHollow.Creatures
{
    /// <summary>
    /// Handles creature breeding using a Punnett Square simulation.
    /// Offspring receive one allele from each parent randomly,
    /// with a configurable mutation chance.
    /// Reference: Plan Section 6.2 - The Inheritance Algorithm.
    /// </summary>
    public class BreedingSystem : MonoBehaviour
    {
        public static BreedingSystem Instance { get; private set; }

        [Header("Breeding Settings")]
        [Tooltip("Chance per breeding of a random mutation (0-1)")]
        [SerializeField] private float mutationRate = 0.05f;

        /// <summary>All possible traits for mutation selection.</summary>
        private static readonly GeneticTrait[] AllTraits = new GeneticTrait[]
        {
            GeneticTrait.NightVision,
            GeneticTrait.Wings,
            GeneticTrait.HardenedScale,
            GeneticTrait.Venomous,
            GeneticTrait.Bioluminescence,
            GeneticTrait.ShadowForm
        };

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Breeds two creatures and produces an offspring genome.
        /// Crossover: offspring receives one allele from each parent randomly.
        /// Mutation: small chance of a random trait being added.
        /// </summary>
        public CreatureGenome Breed(CreatureGenome father, CreatureGenome mother)
        {
            // Crossover: randomly pick one allele from each parent
            GeneticTrait alleleFromFather = (Random.value > 0.5f) ? father.GeneA : father.GeneB;
            GeneticTrait alleleFromMother = (Random.value > 0.5f) ? mother.GeneA : mother.GeneB;

            var child = new CreatureGenome(alleleFromFather, alleleFromMother);

            // Mutation check
            if (Random.value < mutationRate)
            {
                child.GeneA |= GenerateRandomMutation();
                Debug.Log("[BreedingSystem] Mutation occurred in offspring!");
            }

            return child;
        }

        /// <summary>
        /// Generates a random mutation trait.
        /// </summary>
        private GeneticTrait GenerateRandomMutation()
        {
            int index = Random.Range(0, AllTraits.Length);
            return AllTraits[index];
        }

        /// <summary>
        /// Predicts possible trait outcomes for a breeding pair (informational).
        /// Returns the union of all traits that could appear in offspring.
        /// </summary>
        public GeneticTrait GetPossibleTraits(CreatureGenome father, CreatureGenome mother)
        {
            return father.GeneA | father.GeneB | mother.GeneA | mother.GeneB;
        }
    }
}
