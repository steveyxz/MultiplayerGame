using Client.Config;
using Unity.Netcode;
using UnityEngine;

namespace Client
{
    public class AbilityCaster : NetworkBehaviour
    {
        
        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                // Cast basic ability
            }

            if (Input.GetKeyDown(ClientKeybinds.GetKeybind(Keybind.AbilityQ)))
            {
                
            }
            
            
        }
        
    }
}