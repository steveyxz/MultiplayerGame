using Client.Stats;
using Global;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class StatDisplay : NetworkBehaviour
    {
        
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Slider _resourceSlider;
        
        private ClientStatHandler _clientStats;
        
        public override void OnNetworkSpawn()
        {
            _clientStats = GetComponent<ClientStatHandler>();
            _clientStats.OnHealthChanged += OnHealthChanged;
            _clientStats.OnResourceChanged += OnResourceChanged;
        }

        private void OnResourceChanged(float oldvalue, float newvalue)
        {
            var maxResource = _clientStats.GetAggregatedValue(Stat.MaxResource);
            if (maxResource == 0)
            {
                return;
            }
            _resourceSlider.value = newvalue / maxResource;
            Debug.Log("Max Resource: " + maxResource);
            Debug.Log("Resource amount: " + newvalue);
        }

        private void OnHealthChanged(float oldvalue, float newvalue)
        {
            var maxHealth = _clientStats.GetAggregatedValue(Stat.MaxHp);
            if (maxHealth == 0)
            {
                return;
            }
            _healthSlider.value = newvalue / maxHealth;
            Debug.Log("Max Health: " + maxHealth);
            Debug.Log("Health amount: " + newvalue);
        }
    }
}