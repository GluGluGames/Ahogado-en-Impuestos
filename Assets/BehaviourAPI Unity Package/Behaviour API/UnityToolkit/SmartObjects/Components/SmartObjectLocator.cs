using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Component that looks for nearby SmartObjects
    /// </summary>
    public class SmartObjectLocator : MonoBehaviour
    {
        /// <summary>
        /// If true, the objects will be searched automatically
        /// </summary>
        public bool autoCheck = true;

        /// <summary>
        /// The time interval between searches
        /// </summary>
        public float intervalTime = 1f;

        /// <summary>
        /// The maximum distance at which objects will be searched
        /// </summary>
        public float maxDistance = 10f;

        [Header("Gizmos")]
        [SerializeField] bool drawAlways;
        [SerializeField] Color gizmoColor;

        List<SmartObject> _availableSmartObjects;

        float _timer;

        private void Start()
        {
            _availableSmartObjects = new List<SmartObject>();
        }

        private void Update()
        {
            if (autoCheck) return;

            // Actualizar el temporizador
            _timer += Time.deltaTime;

            // Verificar si ha pasado el tiempo suficiente para realizar una comprobación
            if (_timer >= intervalTime)
            {
                LocateObjects();                
            }
        }

        /// <summary>
        /// Look for nearby object and store them in available smart object list.
        /// </summary>
        public void LocateObjects()
        {
            _availableSmartObjects.Clear();

            SmartObject[] allSmartObjects = FindObjectsOfType<SmartObject>();

            foreach (SmartObject obj in allSmartObjects)
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance <= maxDistance)
                {
                    _availableSmartObjects.Add(obj);
                }
            }
            _timer = 0; 
        }

        /// <summary>
        /// Gets the list of located smart objects
        /// </summary>
        /// <returns>The list of available smart objects</returns>
        public List<SmartObject> GetSmartObjects() => _availableSmartObjects;

        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = gizmoColor;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, maxDistance);
        }

        private void OnDrawGizmos()
        {
            if (!drawAlways) return;

            OnDrawGizmosSelected();
        }
    }
}
