using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGG.Classes.Dialogue
{
    [System.Serializable]
    public class DialogueText
    {
        [Tooltip("Texts of the dialogue")]
        [SerializeField, TextArea] private List<string> DialogueLines;
        [Tooltip("Characters that will appear on the dialogue")]
        [SerializeField] private List<Sprite> AvatarList;
        [Tooltip("Name of the characters that will appear on the dialogue")]
        [SerializeField] private List<string> CharacterNames;

        private int _currentDialogue;
        private int _currentAvatar;
        private int _currentName;

        public string GetNextDialogue()
        {
            if (_currentDialogue >= DialogueLines.Count)
                throw new Exception("No more dialogue");

            return DialogueLines[_currentDialogue++];
        }

        public string GetPreviousDialogue()
        {
            if (_currentDialogue - 1 < 0)
                throw new Exception("No previous dialogue");

            return DialogueLines[_currentDialogue - 1];
        }

        public Sprite GetNextAvatar()
        {
            if (_currentAvatar >= AvatarList.Count)
                throw new Exception("No more avatars");

            return AvatarList[_currentAvatar++];
        }
        
        public string GetNextName()
        {
            if (_currentName >= CharacterNames.Count)
                throw new Exception("No more avatars");

            return CharacterNames[_currentName++];
        }

        public bool DialogueEnd() => _currentDialogue >= DialogueLines.Count;

        public bool IsMoreAvatars() => _currentAvatar < AvatarList.Count;

        public bool IsMoreNames() => _currentName < CharacterNames.Count;

        public void ResetDialogue() => _currentDialogue = _currentAvatar = _currentName = 0;
    }
}
