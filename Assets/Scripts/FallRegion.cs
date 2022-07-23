using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDDJAM7
{
    public class FallRegion : MonoBehaviour
    {
        public float speedScaleFactor = 1.0f;

        public GameObject obstacleObjects;
        public float obstaclesPerMeter = 1.0f;


        public bool hasOxygen = true;

        // Things required to survive region
        public GameObject supportObjects;
        public float supportPerMeter = 1.0f;

        private float length;

        // Start is called before the first frame update
        void Start()
        {
            length = GetComponent<BoxCollider>().size.y;
            Debug.Log(this + " is " + length + " meters long.");

            var objectCount = (int)(length * obstaclesPerMeter);

            // Instantiate objects
            for(int i = 0; i < objectCount; i++)
            {
                Instantiate(obstacleObjects, RandomPointInBounds(GetComponent<BoxCollider>().bounds), Quaternion.identity);
            }

            objectCount = (int)(length * supportPerMeter);
            // Instantiate objects
            for (int i = 0; i < objectCount; i++)
            {
                Instantiate(supportObjects, RandomPointInBounds(GetComponent<BoxCollider>().bounds), Quaternion.identity);
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
                Debug.Log(this + " is setting new speed scale of " + speedScaleFactor);
                other.transform.GetComponent<PlayerController>().SetFallSpeedScale(speedScaleFactor);
                other.transform.GetComponent<PlayerController>().SetOxygenPresence(hasOxygen);
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