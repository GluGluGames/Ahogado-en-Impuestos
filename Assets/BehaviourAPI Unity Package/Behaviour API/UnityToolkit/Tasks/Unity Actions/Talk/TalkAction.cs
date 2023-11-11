using System.Collections;
using System.Collections.Generic;
using BehaviourAPI.Core;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Display a text in the talk component, and then waits to remove the text.
    /// </summary>
    public class TalkAction : UnityAction
    {
        /// <summary>
        /// The text displayed in the talk component.
        /// </summary>
        [SerializeField] string text;

        /// <summary>
        /// The number of seconds that the action will wait after display the text.
        /// </summary>
        [SerializeField] float delay;

        float _currentDelay;

        public TalkAction()
        {
        }

        public TalkAction(string text, float delay)
        {
            this.text = text;
            this.delay = delay;
        }

        public override void Start()
        {
            context.Talk.StartTalk(text);
        }

        public override void Stop()
        {
            context.Talk.CancelTalk();
        }

        public override void Pause()
        {
            context.Talk.PauseTalk();
        }

        public override void Unpause()
        {
            context.Talk.ResumeTalk();
        }

        public override Status Update()
        {
            if (context.Talk.IsTalking())
            {
                return Status.Running;
            }
            else
            {
                _currentDelay += Time.deltaTime;
                if (_currentDelay > delay)
                {
                    return Status.Success;
                }
                else
                {
                    return Status.Running;
                }
            }
        }
    }
}
