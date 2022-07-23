using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDDJAM7
{
    public class CameraController : MonoBehaviour
    {
        public GameObject player;

        public Vector3 cameraOffset = new Vector3(5.0f, 0.0f, 0.0f);

        // Start is called before the first frame update
        void Start()
        {
            if (player == null)
                Debug.LogError("Player not connected in " + this);
        }

        // Update is called once per frame
        void Update()
        {
            // Keep camera at fixed offset and always look at player
            Camera.main.transform.position = player.transform.position + cameraOffset;
            Camera.main.transform.LookAt(player.transform);
        }
    }
}