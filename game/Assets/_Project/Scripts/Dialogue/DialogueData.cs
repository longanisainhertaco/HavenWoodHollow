using UnityEngine;

namespace HavenwoodHollow.Dialogue
{
    /// <summary>
    /// Represents a single dialogue choice the player can select.
    /// Optionally links to another DialogueData for branching conversations.
    /// </summary>
    [System.Serializable]
    public struct DialogueChoice
    {
        [Tooltip("The text displayed on the choice button")]
        public string choiceText;

        [Tooltip("Next dialogue to trigger when this choice is selected (null to end)")]
        public DialogueData nextDialogue;
    }

    /// <summary>
    /// Represents a single line of dialogue with an optional set of player choices.
    /// </summary>
    [System.Serializable]
    public struct DialogueLine
    {
        [Tooltip("Name of the character speaking")]
        public string speakerName;

        [TextArea(2, 5)]
        [Tooltip("The dialogue text displayed to the player")]
        public string text;

        [Tooltip("Player choices presented after this line (empty for linear dialogue)")]
        public DialogueChoice[] choices;
    }

    /// <summary>
    /// ScriptableObject containing a sequence of dialogue lines.
    /// Supports branching conversations through DialogueChoice references.
    /// Reference: Plan Section 9 Phase 4 - Integrate Dialogue System.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "HavenwoodHollow/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        [SerializeField] private DialogueLine[] lines;

        /// <summary>Read-only access to all dialogue lines.</summary>
        public DialogueLine[] Lines => lines;

        /// <summary>Total number of lines in this dialogue.</summary>
        public int LineCount => lines != null ? lines.Length : 0;

        /// <summary>
        /// Returns the dialogue line at the specified index, or a default
        /// empty line if the index is out of bounds.
        /// </summary>
        public DialogueLine GetLine(int index)
        {
            if (lines == null || index < 0 || index >= lines.Length)
            {
                Debug.LogWarning($"[DialogueData] Line index {index} out of bounds on '{name}'.");
                return default;
            }

            return lines[index];
        }
    }
}
