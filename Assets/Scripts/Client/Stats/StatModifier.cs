using Global;
using Unity.Netcode;

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
        
        public StatModifier(ulong ownerId, ModifierType modifierType, float value, string identifier, Stat type, float duration = float.NegativeInfinity, float decayRate = 0)
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
    }
}