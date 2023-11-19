using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BehaviourAPI.UnityToolkit
{
    public class TextTalkComponent : MonoBehaviour, ITalkComponent
    {
        [SerializeField] Text textComponent;

        [SerializeField] float _textSpeed = 30f;

        bool isTalking;
        IEnumerator m_currentCoroutine;

        string m_currentText;

        bool isPaused;

        public void CancelTalk()
        {
            textComponent.text = "";
            StopCoroutine(m_currentCoroutine);
            m_currentCoroutine = null;
        }

        public void FinishCurrentTalkLine()
        {
            textComponent.text = m_currentText;
            StopCoroutine(m_currentCoroutine);
        }

        public bool IsTalking()
        {
            return isTalking;
        }

        public void StartTalk(string text)
        {
            m_currentText = text;
            m_currentCoroutine = DisplayText(text);
            StartCoroutine(m_currentCoroutine);
        }

        private IEnumerator DisplayText(string text)
        {
            isTalking = true;

            if (_textSpeed <= 0)
            {
                textComponent.text = text;
                yield return null;
            }
            else
            {
                textComponent.text = "";
                float delay = 1f / _textSpeed;

                int i = 0;
                while (i < text.Length)
                {
                    if (isPaused)
                    {
                        yield return null;
                    }
                    else
                    {
                        textComponent.text += text[i];
                        i++;
                        yield return new WaitForSeconds(delay);
                    }
                }
            }
            isTalking = false;
        }

        public void PauseTalk()
        {
            isPaused = true;
        }

        public void ResumeTalk()
        {
            isPaused = false;
        }
    }
}
