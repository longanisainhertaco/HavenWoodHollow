namespace HavenwoodHollow.Farming
{
    /// <summary>
    /// Lightweight struct storing the dynamic state of a crop tile.
    /// Stored in the CropManager's Dictionary&lt;Vector3Int, CropState&gt;.
    /// Reference: Plan Section 3.2.2 - Data Storage.
    /// </summary>
    public struct CropState
    {
        /// <summary>Reference to the crop's static data.</summary>
        public CropData CropData;

        /// <summary>Current growth phase index.</summary>
        public int CurrentPhase;

        /// <summary>Days elapsed in the current phase.</summary>
        public int DaysInCurrentPhase;

        /// <summary>Whether the tile was watered today.</summary>
        public bool IsWatered;

        /// <summary>Whether the crop is fully grown and ready for harvest.</summary>
        public bool IsHarvestable;

        /// <summary>Fertilizer speed modifier (0 = none, 0.1 = Speed-Gro 10%, 0.25 = Deluxe).</summary>
        public float FertilizerModifier;

        /// <summary>Total days this crop has been growing.</summary>
        public int TotalDaysGrown;
    }
}
