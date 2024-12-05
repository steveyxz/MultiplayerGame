using System;
using Client.Stats;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Global
{

    public enum Character
    {
        TestSquare
    }

    public enum Stat
    {
        MaxHp,
        Hp,
        ResourceRegenAmount,
        ResourceRegenDelay,
        MaxResource,
        Resource,
        Damage,
        Speed,
        Armor,
        AttackSpeed,
        CooldownReduction,
    }
    
    public static class StatExtensions
    {
        public static float GetDefault(this Stat stat)
        {
            return stat switch
            {
                Stat.MaxHp => 100,
                Stat.Hp => 100,
                Stat.ResourceRegenAmount => 1,
                Stat.ResourceRegenDelay => 1,
                Stat.MaxResource => 100,
                Stat.Resource => 100,
                Stat.Damage => 10,
                Stat.Speed => 1,
                Stat.Armor => 0,
                Stat.AttackSpeed => 1,
                Stat.CooldownReduction => 0,
                _ => 0
            };
        }
    }

    /**
     * Represents one damaging hit
     * Percentage is true if the damage is a percentage of the target's health (value will be 0-1)
     * Percentage is false if the damage is a fixed value
     */
    [Serializable]
    public struct DamageEntry
    {
        public bool percentage;
        public float value;
    }

    [Serializable]
    public struct IndexedPrefab
    {
        public string id;
        public GameObject prefab;
    }

    [Serializable]
    public struct IndexedDamageInstance
    {
        public string id;
        public DamageEntry[] damageInstance;
    }

    [Serializable]
    public struct CharacterStats
    {
        public string id;
        public Character character;
        public StatValue[] stats;
        public IndexedDamageInstance[] damageValues;
    }
    
    public struct ModifierSet : INetworkSerializable, IEquatable<ModifierSet>
    {
        public ulong clientId;
        public StatModifier[] modifiers;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref clientId);
            if (serializer.IsReader) {
                var size = modifiers.Length;
                serializer.SerializeValue(ref size);
                modifiers = new StatModifier[size];
                for (int i = 0; i < size; i++)
                {
                    modifiers[i] = new StatModifier();
                }
                foreach (var modifier in modifiers)
                {
                    modifier.NetworkSerialize(serializer);
                }
            } else {
                var size = modifiers.Length;
                serializer.SerializeValue(ref size);
                foreach (var modifier in modifiers)
                {
                    modifier.NetworkSerialize(serializer);
                }
            }
            
        }

        public bool Equals(ModifierSet other)
        {
            if (clientId != other.clientId)
            {
                return false;
            }

            if (modifiers.Length != other.modifiers.Length)
            {
                return false;
            }

            for (int i = 0; i < modifiers.Length; i++)
            {
                if (!modifiers[i].Equals(other.modifiers[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }

    [Serializable]
    public struct StatValue : INetworkSerializable
    {
        public Stat stat;
        public float value;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref stat);
            serializer.SerializeValue(ref value);
        }
    }

    public struct CharacterInformation : INetworkSerializable
    {

        public FixedString64Bytes name;
        public Character character;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref name);
            serializer.SerializeValue(ref character);
        }
    }
    
}