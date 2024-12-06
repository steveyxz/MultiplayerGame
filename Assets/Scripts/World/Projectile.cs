using Client.Stats;
using Unity.Netcode;
using UnityEngine;

namespace World
{
    public class Projectile : NetworkBehaviour
    {
        
        private Rigidbody2D _rigidbody2D;
        
        [Header("Movement")]
        [SerializeField] private float startSpeed = 10f;
        [SerializeField] private float maxSpeed = 20f;
        [SerializeField] private float minSpeed = 2f;
        [SerializeField] private float acceleration = 1f;
        
        [Header("Lifetime")]
        [SerializeField] private float lifeTime = 2f;
        [SerializeField] private int pierce = 0;
        
        [Header("Targeting")]
        [SerializeField] private LayerMask wallLayers;
	    [SerializeField] private LayerMask enemyLayers;
        
        [Header("Effects")]
        [SerializeField] public float damage;
        
        
        public ulong ownerClient;
        public Vector2 direction;

        public override void OnNetworkSpawn()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _rigidbody2D.linearVelocity = transform.right * startSpeed;
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            var linearVelocityMagnitude = _rigidbody2D.linearVelocity.magnitude;
            if (linearVelocityMagnitude < maxSpeed && linearVelocityMagnitude > minSpeed)
            {
                _rigidbody2D.linearVelocity += direction * (acceleration * Time.deltaTime);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (wallLayers == (wallLayers | (1 << other.gameObject.layer)))
            {
                Destroy(gameObject);
            }
            else if (enemyLayers == (enemyLayers | (1 << other.gameObject.layer)))
            {
                if (other.TryGetComponent(out ClientStatHandler health))
                {
                    if (health.OwnerClientId == ownerClient)
                    {
                        return;
                    }
                    health.AddHealthRpc(-1);
                }
                
                if (pierce <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    pierce--;
                }
            }
        }
        
    }
}