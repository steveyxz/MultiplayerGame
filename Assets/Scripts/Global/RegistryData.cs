using UnityEngine;

namespace Global
{
    [CreateAssetMenu(fileName = "RegistryData", menuName = "Global/RegistryData", order = 0)]
    public class RegistryData : ScriptableObject
    {
        
        public CharacterStats[] characterStats;
        public IndexedPrefab[] sharedPrefabs;
        
    }
}