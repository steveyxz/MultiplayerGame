using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Global
{
    public class Registry : MonoBehaviour
    {
        
        public CharacterStats[] characterStats;
        public IndexedPrefab[] sharedPrefabs;
        
        Dictionary<Character, CharacterStats> indexedCharacterStats = new();
        Dictionary<string, GameObject> indexedPrefabs = new();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            foreach (var characterStat in characterStats)
            {
                indexedCharacterStats.Add(characterStat.character, characterStat);
            }

            foreach (var sharedPrefab in sharedPrefabs)
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
        
        public static DamageEntry[] GetDamageInstance(Character character, string id)
        {
            var damageInstances = GetCharacterStats(character).damageValues;
            foreach (var damageInstance in damageInstances)
            {
                if (damageInstance.id == id)
                {
                    return damageInstance.damageInstance;
                }
            }

            return Array.Empty<DamageEntry>();
        }
    }
}