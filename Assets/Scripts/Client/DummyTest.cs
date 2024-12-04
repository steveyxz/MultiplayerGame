using System;
using Client.Stats;
using Global;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
using Random = UnityEngine.Random;

namespace Client
{
    public class DummyTest : NetworkBehaviour
    {
        private ClientStatHandler _clientStats;
        private int index = 0;

        public override void OnNetworkSpawn()
        {
            _clientStats = GetComponent<ClientStatHandler>();
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }

            // if (Input.GetKeyDown(KeyCode.U))
            // {
            //     _clientStats.SetBaseValues(Character.TestSquare);
            // }
            // if (Input.GetKeyDown(KeyCode.I))
            // {
            //     _clientStats.AddModifier(new StatModifier(OwnerClientId, ModifierType.BaseAdditive, 10, "test" + index, Stat.Speed));
            //     index++;
            // }
            // if (Input.GetKeyDown(KeyCode.O))
            // {
            //     _clientStats.RemoveModifier("test" + (index - 1));
            //     index--;
            // }
            // if (Input.GetKeyDown(KeyCode.P))
            // {
            //     _clientStats.Test.Value++;
            // }
        }
    }
}