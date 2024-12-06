using UnityEngine;

namespace Global
{
    [CreateAssetMenu(fileName = "AbilityRegistryData", menuName = "Global/AbilityRegistryData", order = 0)]
    public class AbilityRegistryData : ScriptableObject
    {

        public AbilityInfo[] abilityData;

    }
}