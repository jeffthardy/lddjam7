using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDDJAM7
{
    public class FallRegion : MonoBehaviour
    {
        public float fallSpeed = 10.0f;

        public GameObject gameObjects;
        public float objectsPerMeter = 1.0f;

        private float length;

        // Start is called before the first frame update
        void Start()
        {
            length = GetComponent<BoxCollider>().size.y;
            Debug.Log(this + " is " + length + " meters long.");

            var objectCount = (int)(length / objectsPerMeter);

            // Instantiate objects
            for(int i = 0; i < objectCount; i++)
            {
                Instantiate(gameObjects, RandomPointInBounds(GetComponent<BoxCollider>().bounds), Quaternion.identity);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            // Disable player movement and show dialog
            if (other.transform.tag == "Player")
            {
                other.transform.GetComponent<PlayerController>().SetFallSpeed(fallSpeed);
                // Clear freeze X rotation
                other.transform.GetComponent<Rigidbody>().constraints = other.transform.GetComponent<Rigidbody>().constraints & (~RigidbodyConstraints.FreezeRotationX) ;
            }
        }

        public static Vector3 RandomPointInBounds(Bounds bounds)
        {
            return new Vector3(
                0,
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

    }
}