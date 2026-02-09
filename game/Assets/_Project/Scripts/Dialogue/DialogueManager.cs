using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace HavenwoodHollow.Dialogue
{
    /// <summary>
    /// Singleton manager for the dialogue system.
    /// Handles displaying dialogue lines with a typewriter effect,
    /// player choices, and game pausing during conversations.
    /// Reference: Plan Section 9 Phase 4 - Integrate Dialogue System (Ink planned for later).
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        #region Serialized Fields

        [Header("UI References")]
        [Tooltip("Root panel shown during active dialogue")]
        [SerializeField] private GameObject dialoguePanel;
        [Tooltip("Text element displaying the speaker's name")]
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [Tooltip("Text element displaying the dialogue line")]
        [SerializeField] private TextMeshProUGUI dialogueText;

        [Header("Choices")]
        [Tooltip("Parent transform for choice button instances")]
        [SerializeField] private Transform choicesContainer;
        [Tooltip("Prefab instantiated for each dialogue choice")]
        [SerializeField] private GameObject choiceButtonPrefab;

        [Header("Settings")]
        [Tooltip("Seconds between each character during the typewriter effect")]
        [SerializeField] private float textSpeed = 0.03f;

        #endregion

        #region Events

        /// <summary>Fired when a dialogue conversation begins.</summary>
        public event Action OnDialogueStarted;

        /// <summary>Fired when the current dialogue conversation ends.</summary>
        public event Action OnDialogueEnded;

        /// <summary>Fired when the player selects a choice. Parameter is the choice index.</summary>
        public event Action<int> OnChoiceMade;

        #endregion

        #region Private Fields

        private bool isDialogueActive;
        private DialogueData currentDialogue;
        private int currentLineIndex;
        private Coroutine typewriterCoroutine;

        #endregion

        #region Properties

        /// <summary>Whether a dialogue conversation is currently in progress.</summary>
        public bool IsDialogueActive => isDialogueActive;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts a new dialogue conversation from the given data.
        /// Pauses the game by setting Time.timeScale to 0.
        /// </summary>
        public void StartDialogue(DialogueData data)
        {
            if (data == null || data.LineCount == 0) return;

            currentDialogue = data;
            currentLineIndex = 0;
            isDialogueActive = true;

            Time.timeScale = 0f;

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }

            OnDialogueStarted?.Invoke();
            DisplayLine(currentDialogue.GetLine(currentLineIndex));
        }

        /// <summary>
        /// Advances to the next line in the dialogue. If the typewriter effect
        /// is still running, completes it instantly instead. If there are no
        /// more lines, ends the dialogue.
        /// </summary>
        public void AdvanceLine()
        {
            if (!isDialogueActive) return;

            // If typewriter is still running, complete the current line instantly
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;

                DialogueLine currentLine = currentDialogue.GetLine(currentLineIndex);
                if (dialogueText != null)
                {
                    dialogueText.text = currentLine.text;
                }

                ShowChoices(currentLine.choices);
                return;
            }

            currentLineIndex++;

            if (currentLineIndex >= currentDialogue.LineCount)
            {
                EndDialogue();
                return;
            }

            DisplayLine(currentDialogue.GetLine(currentLineIndex));
        }

        /// <summary>
        /// Ends the current dialogue conversation and restores normal time.
        /// </summary>
        public void EndDialogue()
        {
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }

            isDialogueActive = false;
            currentDialogue = null;
            currentLineIndex = 0;

            Time.timeScale = 1f;

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }

            ClearChoices();
            OnDialogueEnded?.Invoke();
        }

        /// <summary>
        /// Selects a dialogue choice by index. Fires the OnChoiceMade event
        /// and starts the linked dialogue if one exists, otherwise ends.
        /// </summary>
        public void SelectChoice(int index)
        {
            if (!isDialogueActive || currentDialogue == null) return;

            DialogueLine line = currentDialogue.GetLine(currentLineIndex);

            if (line.choices == null || index < 0 || index >= line.choices.Length)
                return;

            OnChoiceMade?.Invoke(index);

            DialogueData nextDialogue = line.choices[index].nextDialogue;

            if (nextDialogue != null)
            {
                StartDialogue(nextDialogue);
            }
            else
            {
                EndDialogue();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Displays a dialogue line with the typewriter effect.
        /// </summary>
        private void DisplayLine(DialogueLine line)
        {
            ClearChoices();

            if (speakerNameText != null)
            {
                speakerNameText.text = line.speakerName ?? string.Empty;
            }

            if (dialogueText != null)
            {
                dialogueText.text = string.Empty;
            }

            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
            }

            typewriterCoroutine = StartCoroutine(TypewriterEffect(line));
        }

        /// <summary>
        /// Coroutine that reveals dialogue text one character at a time.
        /// Uses unscaled time so it works while the game is paused.
        /// </summary>
        private IEnumerator TypewriterEffect(DialogueLine line)
        {
            string fullText = line.text ?? string.Empty;

            for (int i = 0; i < fullText.Length; i++)
            {
                if (dialogueText != null)
                {
                    dialogueText.text = fullText.Substring(0, i + 1);
                }

                yield return new WaitForSecondsRealtime(textSpeed);
            }

            typewriterCoroutine = null;
            ShowChoices(line.choices);
        }

        /// <summary>
        /// Instantiates choice buttons for the current line's choices.
        /// </summary>
        private void ShowChoices(DialogueChoice[] choices)
        {
            ClearChoices();

            if (choices == null || choices.Length == 0 || choicesContainer == null || choiceButtonPrefab == null)
                return;

            for (int i = 0; i < choices.Length; i++)
            {
                GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesContainer);
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

                if (buttonText != null)
                {
                    buttonText.text = choices[i].choiceText;
                }

                int choiceIndex = i;
                UnityEngine.UI.Button button = buttonObj.GetComponent<UnityEngine.UI.Button>();

                if (button != null)
                {
                    button.onClick.AddListener(() => SelectChoice(choiceIndex));
                }
            }
        }

        /// <summary>
        /// Destroys all currently instantiated choice buttons.
        /// </summary>
        private void ClearChoices()
        {
            if (choicesContainer == null) return;

            for (int i = choicesContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(choicesContainer.GetChild(i).gameObject);
            }
        }

        #endregion
    }
}
