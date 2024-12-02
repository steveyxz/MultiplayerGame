using System;
using Unity.Netcode;
using UnityEngine;

namespace Client
{
    public class CameraController : NetworkBehaviour
    {

        private Camera camera;
        private GameObject player;
        [SerializeField] private float drift = 0.5f;
        
        public void Start()
        {
            if (!IsOwner)
            {
                return;
            }
            camera = Camera.main;

            player = gameObject;
        }

        public void FixedUpdate()
        {
            if (!IsOwner)
            {
                return;
            }

            if (player == null)
            {
                return;
            }

            var playerPosition = player.transform.position;
            var cameraPosition = camera.transform.position;
            var newPosition = Vector3.Lerp(cameraPosition, playerPosition, drift);
            camera.transform.position = new Vector3(newPosition.x, newPosition.y, cameraPosition.z);
            if ((cameraPosition - playerPosition).sqrMagnitude < 0.01f)
            {
                camera.transform.position = new Vector3(playerPosition.x, playerPosition.y, cameraPosition.z);
            }
        }
    }
}