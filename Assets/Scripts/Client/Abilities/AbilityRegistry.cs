using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Client.Abilities
{
    public class AbilityRegistry : NetworkBehaviour
    {
        
        private static Dictionary<string, IAbility> _abilities = new();
        
        public static AbilityRegistry Instance { get; private set; }

        private void Awake()
        {
            // Register abilities here
            Instance = this;
        }
        
        public static void RegisterAbility(IAbility ability)
        {
            _abilities.Add(ability.Identifier, ability);
        }
        
        public static IAbility GetAbility(string identifier)
        {
            return _abilities[identifier];
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