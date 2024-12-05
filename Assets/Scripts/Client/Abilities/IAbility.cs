using UnityEngine;

namespace Client.Abilities
{
    // tehnically serverside i think? idrk
    public interface IAbility
    {

        public string Identifier { get; }
        
        public void Cast(GameObject owner, Vector3 target);
        
        void Update(GameObject owner, float delta);
        
        T GetStoredData<T>(string key, ulong clientUid);
        
        void SetStoredData<T>(string key, T value, ulong clientUid);

    }
}