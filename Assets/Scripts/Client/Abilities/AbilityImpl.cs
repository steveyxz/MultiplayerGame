using System.Collections.Generic;
using UnityEngine;

namespace Client.Abilities
{
    public class AbilityImpl : IAbility
    {
        public string Identifier { get; }
        
        protected Dictionary<string, object> StoredData = new();
        
        public AbilityImpl(string identifier)
        {
            Identifier = identifier;
        }
        
        public virtual void Cast(GameObject owner, Vector3 target)
        {
        }

        public virtual void Update(GameObject owner, float delta)
        {
        }

        public T GetStoredData<T>(string key, ulong clientUid)
        {
            var obj = StoredData[key];
            if (obj == null) return default;
            if (obj is T t) return t;
            return default;
        }

        public void SetStoredData<T>(string key, T value, ulong clientUid)
        {
            if (value is object obj) StoredData[key] = obj;
        }
    }
}