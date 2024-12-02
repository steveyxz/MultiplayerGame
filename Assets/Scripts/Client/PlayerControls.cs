using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace Client
{
    public class PlayerControls : NetworkBehaviour
    {
        
        private Rigidbody2D _networkRigidbody2D;

        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float startingSpeed = 7f;
        [SerializeField] private float acceleration = 0.8f;
        [SerializeField] private float friction = 0.7f;

        public override void OnNetworkSpawn()
        {
            _networkRigidbody2D = GetComponent<Rigidbody2D>();
        }
        
        void Update()
        {
            if (!IsLocalPlayer)
            {
                return;
            }
            
            // Move the player
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
           
            var impulse = new Vector2(x, y) * acceleration;
            _networkRigidbody2D.AddForce(impulse);
            
            if (_networkRigidbody2D.linearVelocity.magnitude > maxSpeed)
            {
                _networkRigidbody2D.linearVelocity = _networkRigidbody2D.linearVelocity.normalized * maxSpeed;
            }

            if (impulse.magnitude > 0 && _networkRigidbody2D.linearVelocity.magnitude < startingSpeed)
            {
                _networkRigidbody2D.linearVelocity = impulse.normalized * startingSpeed;
            }
            
        }

        private void FixedUpdate()
        {
            if (!IsLocalPlayer)
            {
                return;
            }
            
            // Apply friction
            _networkRigidbody2D.linearVelocity *= friction;
        }
    }
}