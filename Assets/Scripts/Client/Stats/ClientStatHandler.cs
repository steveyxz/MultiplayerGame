using System;
using System.Collections.Generic;
using Global;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Client.Stats
{
    public class ClientStatHandler : NetworkBehaviour
    {
        public NetworkVariable<ModifierSet> modifiers = new(new ModifierSet
        {
            clientId = 0,
            modifiers = Array.Empty<StatModifier>()
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public delegate void HealthChanged(float oldValue, float newValue);
        public delegate void ResourceChanged(float oldValue, float newValue);

        public HealthChanged OnHealthChanged;
        public ResourceChanged OnResourceChanged;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            modifiers.OnValueChanged += OnModifierSetChanged;
            if (IsOwner)
            {
                modifiers.Value = new ModifierSet
                {
                    clientId = OwnerClientId,
                    modifiers = Array.Empty<StatModifier>()
                };
            }
        }

        private void OnModifierSetChanged(ModifierSet previousV, ModifierSet newV)
        {
            if (IsOwner)
            {
                UpdateHealth(previousV, newV);
            }
        }

        public void SetBaseValues(Character character)
        {
            var characterStats = Registry.GetCharacterStats(character);
            foreach (Stat stat in Enum.GetValues(typeof(Stat)))
            {
                RemoveModifier(stat + "-base");
                AddModifier(
                    new StatModifier(OwnerClientId, ModifierType.BaseAdditive, stat.GetDefault(), stat + "-base", stat));
            }

            foreach (var stat in characterStats.stats)
            {
                RemoveModifier(stat.stat + "-base");
                AddModifier(new StatModifier(OwnerClientId, ModifierType.BaseAdditive, stat.value, stat.stat + "-base",
                    stat.stat));
            }
        }

        [Rpc(SendTo.Owner)]
        public void RemoteAddRpc(StatModifier modifier)
        {
            AddModifier(modifier);
        }

        [Rpc(SendTo.Owner)]
        public void RemoteRemoveRpc(string modifierId)
        {
            RemoveModifier(modifierId);
        }
        
        [Rpc(SendTo.Owner)]
        public void AddHealthRpc(float health)
        {
            SetHealth(GetHealth() + health);
        }
        
        [Rpc(SendTo.Owner)]
        public void AddResourceRpc(float resource)
        {
            SetResource(GetResource() + resource);
        }
        
        [Rpc(SendTo.Owner)]
        public void SetHealthRpc(float health)
        {
            SetHealth(health);
        }
        
        [Rpc(SendTo.Owner)]
        public void SetResourceRpc(float resource)
        {
            SetResource(resource);
        }

        public void SetHealth(float newHealth)
        {
            var baseHp = GetModifier("base-Hp");
            var oldHealth = baseHp != null ? baseHp.Value : 0;
            if (baseHp != null)
            {
                baseHp.Value = newHealth;
                OnHealthChanged?.Invoke(oldHealth, newHealth);
            }
        }
        
        public float GetHealth()
        {
            var baseHp = GetModifier("base-Hp");
            return baseHp != null ? baseHp.Value : 0;
        }
        
        public void SetResource(float newResource)
        {
            var baseResource = GetModifier("base-Resource");
            var oldResource = baseResource != null ? baseResource.Value : 0;
            if (baseResource != null)
            {
                baseResource.Value = newResource;
                OnResourceChanged?.Invoke(oldResource, newResource);
            }
        }
        
        public float GetResource()
        {
            var baseResource = GetModifier("base-Resource");
            return baseResource != null ? baseResource.Value : 0;
        }
        
        public void UpdateHealth(ModifierSet previous, ModifierSet current)
        {
            var previousHp = GetAggregatedValue(Stat.MaxHp, previous);
            var currentHp = GetAggregatedValue(Stat.MaxHp, current);
            if (!Mathf.Approximately(previousHp, currentHp))
            {
                var percentage = currentHp / previousHp;
                SetHealth(GetHealth() * percentage);
            }
        }
        
        public StatModifier GetModifier(string modifierId)
        {
            foreach (var modifier in modifiers.Value.modifiers)
            {
                if (modifier.Identifier == modifierId)
                {
                    return modifier;
                }
            }

            return null;
        }

        public void AddModifier(StatModifier modifier)
        {
            var valueModifiers = modifiers.Value.modifiers;
            var newModifiers = new StatModifier[valueModifiers.Length + 1];
            for (int i = 0; i < valueModifiers.Length; i++)
            {
                newModifiers[i] = valueModifiers[i];
            }

            newModifiers[valueModifiers.Length] = modifier;
            modifiers.Value = new ModifierSet
            {
                clientId = modifier.OwnerId,
                modifiers = newModifiers
            };
        }

        public void RemoveModifier(string modifierId)
        {
            var valueModifiers = modifiers.Value.modifiers;
            var newModifiers = new List<StatModifier>();
            ulong ownerId = 1;
            foreach (var modifier in valueModifiers)
            {
                if (ownerId == 1)
                {
                    ownerId = modifier.OwnerId;
                }

                if (modifier.Identifier != modifierId)
                {
                    newModifiers.Add(modifier);
                }
            }

            modifiers.Value = new ModifierSet
            {
                clientId = ownerId,
                modifiers = newModifiers.ToArray()
            };
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }

            foreach (var modifier in modifiers.Value.modifiers)
            {
                modifier.UpdateState(this, Time.deltaTime);
            }
        }

        public float GetAggregatedValue(Stat stat)
        {
            return GetAggregatedValue(stat, modifiers.Value);
        }

        public float GetAggregatedValue(Stat stat, ModifierSet modifierSet)
        {
            float value = 0;
            // stat calculation order will be
            // 1. base values + added base values
            // 2. multipliers on the base value
            // 3. added final values
            // 4. multipliers on the final value
            foreach (var modifier in modifierSet.modifiers)
            {
                if (modifier.ModifierType == ModifierType.BaseAdditive && modifier.Type == stat)
                {
                    value += modifier.Value;
                }
            }

            foreach (var modifier in modifierSet.modifiers)
            {
                if (modifier.ModifierType == ModifierType.BaseMultiplicative && modifier.Type == stat)
                {
                    value *= modifier.Value;
                }
            }

            foreach (var modifier in modifierSet.modifiers)
            {
                if (modifier.ModifierType == ModifierType.FinalAdditive && modifier.Type == stat)
                {
                    value += modifier.Value;
                }
            }

            foreach (var modifier in modifierSet.modifiers)
            {
                if (modifier.ModifierType == ModifierType.FinalMultiplicative && modifier.Type == stat)
                {
                    value *= modifier.Value;
                }
            }

            return value;
        }
    }
}