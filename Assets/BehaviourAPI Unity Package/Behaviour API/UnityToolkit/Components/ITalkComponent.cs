using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Implement this interface to create a component that 
    /// </summary>
    public interface ITalkComponent
    {
        void StartTalk(string text);

        bool IsTalking();

        void FinishCurrentTalkLine();

        void PauseTalk();

        void ResumeTalk();

        void CancelTalk();
    }
}
