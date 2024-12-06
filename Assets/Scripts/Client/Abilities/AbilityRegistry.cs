using System;
using System.Collections.Generic;
using Global;
using Unity.Netcode;
using UnityEngine;

namespace Client.Abilities
{
    public class AbilityRegistry : NetworkBehaviour
    {
        
        private static Dictionary<string, IAbility> _abilities = new();
        private static Dictionary<string, AbilityInfo> _abilityData = new();

        public AbilityRegistryData data;
        
        public static AbilityRegistry Instance { get; private set; }

        private void Awake()
        {
            // Register abilities here
            Instance = this;
            foreach (var ability in data.abilityData)
            {
                _abilityData.Add(ability.identifier, ability);
            }
        }
        
        public static void RegisterAbility(IAbility ability)
        {
            _abilities.Add(ability.Identifier, ability);
        }
        
        public static IAbility GetAbility(string identifier)
        {
            return _abilities[identifier];
        }
        
        public static DamageEntry[] GetDamageInstance(string ability, string id)
        {
            var found = _abilityData.TryGetValue(ability, out var abilityInfo);
            if (!found) return null;
            foreach (var damageInstance in abilityInfo.damageValues)
            {
                if (damageInstance.id == id)
                {
                    return damageInstance.damageInstance;
                }
            }

            return null;
        }
        
        public static float GetMagicValue(string ability, string id)
        {
            var found = _abilityData.TryGetValue(ability, out var abilityInfo);
            if (!found) return 0;
            foreach (var magicValue in abilityInfo.magicValues)
            {
                if (magicValue.id == id)
                {
                    return magicValue.value;
                }
            }

            return 0;
        }

        private void Update()
        {
            if (!IsServer) return;
            foreach (var ability in _abilities)
            {
                ability.Value.Update(gameObject, Time.deltaTime);
            }
        }
        
        [Rpc(SendTo.Server)]
        public void CastAbilityServerRpc(string identifier, ulong player, Vector3 target)
        {
            if (!IsServer) return;
            foreach (var client in NetworkManager.ConnectedClientsList)
            {
                if (client.ClientId != player) continue;
                _abilities[identifier].Cast(client.PlayerObject.gameObject, target);
                return;
            }
        }
    }
}