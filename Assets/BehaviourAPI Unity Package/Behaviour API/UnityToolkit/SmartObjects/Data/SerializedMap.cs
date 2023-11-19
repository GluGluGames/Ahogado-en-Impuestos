using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    [System.Serializable]
    public class SerializedMap<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] protected List<TKey> keys = new List<TKey>();
        [SerializeField] protected List<TValue> values = new List<TValue>();

        public void OnAfterDeserialize()
        {
            Clear();

            var max = Mathf.Min(keys.Count, values.Count);
            for (int i = 0; i < max; i++)
            {
                if (!TryAdd(keys[i], values[i]))
                {
                    Debug.LogWarning("Invalid or repeated key: " + keys[i]);
                }
            }
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (KeyValuePair<TKey, TValue> kvp in this)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }
    }
}
