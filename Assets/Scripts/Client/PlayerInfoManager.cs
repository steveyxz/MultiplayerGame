using System;
using Global;
using Unity.Netcode;
using UnityEngine;

namespace Client
{
    public class PlayerInfoManager : NetworkBehaviour
    {
        private NetworkVariable<CharacterInformation> _characterInformation = new();

        public override void OnNetworkSpawn()
        {
            if (IsOwner) {
                _characterInformation.Value = new CharacterInformation
                {
                    name = "Player",
                    character = Character.TestSquare,
                };
            }
        }
    }
}