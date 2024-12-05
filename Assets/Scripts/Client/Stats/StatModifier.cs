using System;
using System.Collections.Generic;
using Global;
using Unity.Netcode;
using UnityEngine;

namespace Client.Stats
{
    public enum ModifierType
    {
        BaseAdditive,
        BaseMultiplicative,
        FinalAdditive,
        FinalMultiplicative
    }

    public class StatModifier : INetworkSerializable
    {
        public ulong OwnerId;
        public ModifierType ModifierType;
        public float Value;
        public string Identifier;
        public Stat Type;
        public float Duration;
        public float TimeRemaining;
        public float DecayRate;

        public StatModifier()
        {
        }

        public StatModifier(ulong ownerId, ModifierType modifierType, float value, string identifier, Stat type,
            float duration = float.NegativeInfinity, float decayRate = 0)
        {
            OwnerId = ownerId;
            ModifierType = modifierType;
            Value = value;
            Identifier = identifier;
            Type = type;
            Duration = duration;
            TimeRemaining = duration;
            DecayRate = decayRate;
        }

        public StatModifier(StatModifier previous)
        {
            OwnerId = previous.OwnerId;
            ModifierType = previous.ModifierType;
            Value = previous.Value;
            Identifier = previous.Identifier;
            Type = previous.Type;
            Duration = previous.Duration;
            TimeRemaining = previous.TimeRemaining;
            DecayRate = previous.DecayRate;
        }

        public void UpdateState(ClientStatHandler handler, float delta)
        {
            Value -= DecayRate * delta;
            if (float.IsNegativeInfinity(Duration))
            {
                return;
            }

            TimeRemaining -= delta;
            if (TimeRemaining <= 0)
            {
                handler.RemoveModifier(Identifier);
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref OwnerId);
            serializer.SerializeValue(ref ModifierType);
            serializer.SerializeValue(ref Value);
            serializer.SerializeValue(ref Identifier);
            serializer.SerializeValue(ref Type);
            serializer.SerializeValue(ref Duration);
            serializer.SerializeValue(ref TimeRemaining);
            serializer.SerializeValue(ref DecayRate);
        }


        public override bool Equals(object obj)
        {
            return obj is StatModifier modifier &&
                   OwnerId == modifier.OwnerId &&
                   ModifierType == modifier.ModifierType &&
                   Mathf.Approximately(Value, modifier.Value) &&
                   Identifier == modifier.Identifier &&
                   Type == modifier.Type &&
                   Mathf.Approximately(Duration, modifier.Duration) &&
                   Mathf.Approximately(TimeRemaining, modifier.TimeRemaining) &&
                   Mathf.Approximately(DecayRate, modifier.DecayRate);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OwnerId, (int)ModifierType, Value, Identifier, (int)Type, Duration, TimeRemaining, DecayRate);
        }

        public int GetHashCode(StatModifier obj)
        {
            return HashCode.Combine(obj.OwnerId, (int)obj.ModifierType, obj.Value, obj.Identifier, (int)obj.Type,
                obj.Duration, obj.TimeRemaining, obj.DecayRate);
        }
    }
}