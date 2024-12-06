using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Global
{
    public class Registry : MonoBehaviour
    {

        public RegistryData data;
        
        Dictionary<Character, CharacterStats> indexedCharacterStats = new();
        Dictionary<string, GameObject> indexedPrefabs = new();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            foreach (var characterStat in data.characterStats)
            {
                indexedCharacterStats.Add(characterStat.character, characterStat);
            }

            foreach (var sharedPrefab in data.sharedPrefabs)
            {
                indexedPrefabs.Add(sharedPrefab.id, sharedPrefab.prefab);
            }
        }
        
        public static Registry Instance { get; private set; }
        
        public static CharacterStats GetCharacterStats(Character character)
        {
            return Instance.indexedCharacterStats[character];
        }
        
        public static GameObject GetPrefab(string id)
        {
            return Instance.indexedPrefabs[id];
        }
        
        public static float GetBaseStat(Character character, Stat stat)
        {
            var statVals = GetCharacterStats(character).stats;
            foreach (var statVal in statVals)
            {
                if (statVal.stat == stat)
                {
                    return statVal.value;
                }
            }

            return stat.GetDefault();

        }
    }
}