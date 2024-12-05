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
            var previousActualHp = GetAggregatedValue(Stat.Hp, previousV);
            var currentActualHp = GetAggregatedValue(Stat.Hp, newV);
            if (!Mathf.Approximately(previousActualHp, currentActualHp))
            {
                OnHealthChanged?.Invoke(previousActualHp, currentActualHp);
            }

            var previousActualResource = GetAggregatedValue(Stat.Resource, previousV);
            var currentActualResource = GetAggregatedValue(Stat.Resource, newV);
            if (!Mathf.Approximately(previousActualResource, currentActualResource))
            {
                OnResourceChanged?.Invoke(previousActualResource, currentActualResource);
            }

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
                    new StatModifier(OwnerClientId, ModifierType.BaseAdditive, stat.GetDefault(), stat + "-base",
                        stat));
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
            var valueModifiers = modifiers.Value.modifiers;
            var newModifiers = new StatModifier[valueModifiers.Length];

            StatModifier m = null;
            for (int i = 0; i < valueModifiers.Length; i++)
            {
                var valueModifier = valueModifiers[i];
                if (valueModifier.Identifier == "Hp-base")
                {
                    m = new StatModifier(valueModifier)
                    {
                        Value = newHealth
                    };
                    newModifiers[i] = m;
                } else {
                    newModifiers[i] = valueModifier;
                }
            }

            if (m == null) return;
            
            modifiers.Value = new ModifierSet
            {
                clientId = OwnerClientId,
                modifiers = newModifiers
            };
            

            // var newModifier = new StatModifier(m);
            // newModifier.Identifier = "dummy-hp";
            // AddModifier(newModifier);
            // RemoveModifier("dummy-hp");
        }

        private void RemoveModifier(StatModifier toDelete)
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

                if (modifier != toDelete)
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

        public float GetHealth()
        {
            var baseHp = GetModifier("Hp-base");
            //Debug.Log(baseHp);
            return baseHp?.Value ?? 0;
        }

        public void SetResource(float newResource)
        {
            var valueModifiers = modifiers.Value.modifiers;
            var newModifiers = new StatModifier[valueModifiers.Length];

            StatModifier m = null;
            for (int i = 0; i < valueModifiers.Length; i++)
            {
                var valueModifier = valueModifiers[i];
                if (valueModifier.Identifier == "Resource-base")
                {
                    m = new StatModifier(valueModifier)
                    {
                        Value = newResource
                    };
                    newModifiers[i] = m;
                } else {
                    newModifiers[i] = valueModifier;
                }
            }

            if (m == null) return;
            
            modifiers.Value = new ModifierSet
            {
                clientId = OwnerClientId,
                modifiers = newModifiers
            };
        }

        public float GetResource()
        {
            var baseResource = GetModifier("Resource-base");
            return baseResource?.Value ?? 0;
        }

        public void UpdateHealth(ModifierSet previous, ModifierSet current)
        {
            var previousHp = GetAggregatedValue(Stat.MaxHp, previous);
            var currentHp = GetAggregatedValue(Stat.MaxHp, current);
            if (!Mathf.Approximately(previousHp, currentHp))
            {
                if (previousHp == 0)
                {
                    SetHealth(currentHp);
                }
                else
                {
                    var percentage = currentHp / previousHp;
                    SetHealth(GetHealth() * percentage);
                }
            }

            var previousResource = GetAggregatedValue(Stat.MaxResource, previous);
            var currentResource = GetAggregatedValue(Stat.MaxResource, current);
            if (!Mathf.Approximately(previousResource, currentResource))
            {
                if (previousResource == 0)
                {
                    SetResource(currentResource);
                }
                else
                {
                    var percentage = currentResource / previousResource;
                    SetResource(GetResource() * percentage);
                }
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