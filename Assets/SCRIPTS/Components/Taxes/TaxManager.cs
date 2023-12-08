using System;
using GGG.Classes.Dialogue;
using GGG.Components.Dialogue;
using UnityEngine;

namespace GGG.Components.Taxes
{
    public class TaxManager : MonoBehaviour
    {
        [Tooltip("Interval, in minutes, between Poseidon asks you for taxes")]
        [SerializeField] private int TaxesInterval;
        [Tooltip("Dialogue that will be triggered")] 
        [SerializeField] private DialogueText Dialogue;

        private DialogueBox _dialogueBox;
        private TaxUI _taxUI;
        private static float _intervalDelta;
        private bool _stopInterval;

        private void Start()
        {
            _dialogueBox = FindObjectOfType<DialogueBox>();
            if (!_dialogueBox) throw new Exception("No dialogue box found");

            _taxUI = GetComponent<TaxUI>();

            _dialogueBox.DialogueStart += () => _stopInterval = true;
            _taxUI.OnOptionSelected += () => _stopInterval = false;
            
            _intervalDelta = TaxesInterval * 60;
            _dialogueBox.DialogueEnd += _taxUI.Open;
            _taxUI.OnOptionSelected += () => _dialogueBox.DialogueEnd -= _taxUI.Open;
        }

        private void Update()
        {
            if (_stopInterval) return;
            
            if (_intervalDelta > 0)
            {
                _intervalDelta -= Time.deltaTime;
                return;
            }
            
            TriggerTaxes();
        }

        public static float GetRemainingTime() => _intervalDelta;

        public void TriggerTaxes()
        {
            // TODO - Optional: Make different dialogues and chose one random dialogue.
            _dialogueBox.AddNewDialogue(Dialogue);
            _intervalDelta = TaxesInterval * 60;
        }
    }
}
