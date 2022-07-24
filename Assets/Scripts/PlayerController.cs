using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace LDDJAM7
{
    public class PlayerController : MonoBehaviour
    {
        public float maxMoveSpeed = 10;
        public float maxSpinSpeed = 10;
        public float maxFallSpeed = 10;

        public float moveSpeed = 1000;
        public float jumpHeight = 10;
        public float rotateSpeed = 10;
        public float spinSpeed = 1000;
        public float oxygenCapacity = 5.0f;
        public GameObject humanoidChild;
        public Slider oxygenSlider;
        public Slider sicknessSlider;
        public ScoreKeeper scoreKeeper;
        public TextMeshProUGUI depthValueText;
        public TextMeshProUGUI multiplierText;


        public AudioClip startClip;
        public AudioClip stopClip;
        public AudioClip ughClip;
        public AudioClip woohooClip;
        public AudioClip weeClip;
        public AudioClip slurpClip;
        public AudioClip pukeClip;


        private bool jumpPending;
        private Vector2 direction;
        private float rotation;
        private Rigidbody rb;
        private Animator humanoidMeshAnimator;
        private bool isGrounded;
        private float forwardSpin;
        private float backwardSpin;
        private Vector3 lastDirection;
        private float totalForwardRotationAngle;
        private float totalBackwardRotationAngle;
        private int totalForwardRotationCount;
        private int totalBackwardRotationCount;

        private ParticleSystem pukeSystem;
        private AudioSource audioSource;

        // This might change depending on space
        private float speedScaleFactor = 1;
        private bool areaHasOxygen = true;
        private float currentOxygenLevel;
        private float comboStartTime;

        private bool hasPuked;
        private int currentMultiplier;


        private bool playerEnabled;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            humanoidMeshAnimator = humanoidChild.GetComponent<Animator>();
            isGrounded = false;
            humanoidMeshAnimator.SetBool("IsGrounded", false);
            forwardSpin = 0.0f;
            backwardSpin = 0.0f;

            pukeSystem = GetComponentInChildren<ParticleSystem>();
            audioSource = GetComponent<AudioSource>();

            lastDirection = transform.TransformDirection(Vector3.forward);
            totalForwardRotationCount = 0;
            totalBackwardRotationCount = 0;
            oxygenSlider.maxValue = oxygenCapacity;
            oxygenSlider.value = oxygenCapacity;
            sicknessSlider.value = 0;
            comboStartTime = Time.time;
            currentMultiplier = 1;
            multiplierText.text = "1X";

            playerEnabled = true;
            hasPuked = false;


            StartCoroutine(PlayClipAfter(startClip,1.0f));
        }




        // Update is called once per frame
        void Update()
        {
            if (playerEnabled)
            {
                // Display Depth
                depthValueText.text = (int)(transform.position.y) + "m";

                // Check if we are out of Oxygen
                if (!areaHasOxygen)
                {
                    currentOxygenLevel -= Time.deltaTime;
                    oxygenSlider.value = currentOxygenLevel;
                    if (currentOxygenLevel < 0)
                    {
                        Debug.Log("You have died! Oxygen is " + currentOxygenLevel);
                        scoreKeeper.SaveFinalScore((int)(transform.position.y));
                        playerEnabled = false;
                        StartCoroutine(PlayClipAfter(stopClip, 0f));

                    }
                    //else
                    //    Debug.Log("currentOxygenLevel : " + currentOxygenLevel);
                }


                // Mostly fixed direction 
                Vector3 moveVector = (new Vector3(0, 0, 1) * -direction.x * (moveSpeed * Time.deltaTime));
                //rb.MovePosition(transform.position + moveVector);

                rb.AddForce(moveVector, ForceMode.Acceleration);


                if (isGrounded && jumpPending)
                {
                    rb.AddForce(transform.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
                    jumpPending = false;
                }

                if (!isGrounded)
                {
                    if ((forwardSpin != 0) || (backwardSpin != 0))
                    {
                        rb.AddTorque(transform.right * spinSpeed * forwardSpin * Time.deltaTime);
                        rb.AddTorque(transform.right * -spinSpeed * backwardSpin * Time.deltaTime);
                    }
                    else
                    {
                        if ((rotation != 0.0f))
                            rb.AddTorque(transform.right * rotateSpeed * -rotation * Time.deltaTime);

                    }
                }

                if (moveVector != new Vector3(0, 0, 0))
                {
                    //Debug.Log("Moving by " + moveVector);
                    humanoidMeshAnimator.SetBool("IsMoving", true);
                }
                else
                {
                    humanoidMeshAnimator.SetBool("IsMoving", false);
                }


                // Clamp max speeds except on falling axis
                var v = rb.velocity;
                var yd = v.y;
                v.y = 0f;
                v = Vector3.ClampMagnitude(v, maxMoveSpeed);
                v.y = Vector3.ClampMagnitude(new Vector3(0, yd, 0), maxFallSpeed * speedScaleFactor).y;
                rb.velocity = v;


                var va = rb.angularVelocity;
                va = Vector3.ClampMagnitude(va, maxSpinSpeed * speedScaleFactor);
                rb.angularVelocity = va;



                CountRotations();
                CalculateMultiplier();
            }
            else
            {
                // Play should be stopped now.
                rb.isKinematic = true;
            }
        }

        private void CalculateMultiplier()
        {

            currentMultiplier = 0;
            if (totalForwardRotationCount > 0)
                currentMultiplier++;
            if (totalBackwardRotationCount > 0)
                currentMultiplier++;
            if (Time.time > comboStartTime +10)
                currentMultiplier++;
            if (hasPuked)
                currentMultiplier++;

            if (currentMultiplier == 0)
                currentMultiplier = 1;
            multiplierText.text = currentMultiplier +  "X";
        }

        private void CountRotations()
        {

            Vector3 facing = transform.TransformDirection(Vector3.forward);
            //facing.y = 0;

            // Direction has changed
            if (lastDirection != facing)
            {

                float angle = Vector3.Angle(lastDirection, facing);

                // Moving Backwards so reset forwardflip
                var cross = Vector3.Cross(lastDirection, facing);
                //if (cross != new Vector3(0, 0, 0))
                //    Debug.Log("cross: " + cross);
                if (cross.x < 0)
                {
                    totalBackwardRotationAngle += angle;
                    if (angle != 0)
                    {
                        totalForwardRotationAngle = 0;
                        //Debug.Log("back angle: " + angle);
                    }
                }
                else
                {
                    totalForwardRotationAngle += angle;
                    if (angle != 0)
                    {
                        totalBackwardRotationAngle = 0;
                        //aDebug.Log("forward angle: " + angle);
                    }
                }

                lastDirection = facing;

                while (totalForwardRotationAngle > 360)
                {
                    totalForwardRotationAngle -= 360;
                    totalForwardRotationCount++;
                    audioSource.PlayOneShot(weeClip);
                }

                while (totalBackwardRotationAngle > 360)
                {
                    totalBackwardRotationAngle -= 360;
                    totalBackwardRotationCount++;
                    audioSource.PlayOneShot(weeClip);


                    //Debug.Log("totalBackwardRotationCount: " + totalBackwardRotationCount);
                }

                var spinDiff = Mathf.Abs(totalBackwardRotationCount - totalForwardRotationCount);
                sicknessSlider.value = spinDiff;
                if (sicknessSlider.value == sicknessSlider.maxValue)
                {
                    if(!hasPuked)
                        triggerPuke();
                }
            }
        }


        public void Move(InputAction.CallbackContext context)
        {
            direction = context.ReadValue<Vector2>();
            humanoidMeshAnimator.SetFloat("direction", -direction.x);
            //Debug.Log("Direction " + direction);
        }
        public void Rotate(InputAction.CallbackContext context)
        {
            rotation = Mathf.Clamp(context.ReadValue<float>(), -1, 1);
            //Debug.Log("rotation " + rotation);
        }
        public void ForwardSpin(InputAction.CallbackContext context)
        {
            forwardSpin = context.ReadValue<float>();
            //Debug.Log("forwardSpin " + forwardSpin);
        }
        public void BackwardSpin(InputAction.CallbackContext context)
        {
            backwardSpin = context.ReadValue<float>();
            //Debug.Log("backwardSpin " + backwardSpin);
        }
        public void Jump(InputAction.CallbackContext context)
        {
            jumpPending = true;
            if(isGrounded)
                audioSource.PlayOneShot(woohooClip);
            //Debug.Log("Jump!");
        }

        public void SetOxygenPresence(bool oxygenStatus)
        {
            if (areaHasOxygen && !oxygenStatus)
                currentOxygenLevel = oxygenCapacity;
            areaHasOxygen = oxygenStatus;
        }

        public void SetFallSpeedScale(float speed)
        {
            speedScaleFactor = speed;
            Debug.Log("New speed scale factor is " + speed);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag == "Oxygen")
            {
                currentOxygenLevel += 1;
                audioSource.PlayOneShot(slurpClip);
                if (currentOxygenLevel > oxygenCapacity)
                    currentOxygenLevel = oxygenCapacity;

                Destroy(other.transform.gameObject);
            }
        }

        void OnCollisionStay(Collision collisionInfo)
        {
            if (collisionInfo.gameObject.tag == "Ground")
            {
                if (isGrounded == false)
                    audioSource.PlayOneShot(ughClip);

                isGrounded = true;
                humanoidMeshAnimator.SetBool("IsGrounded", isGrounded);
                EndCombo();
            }
        }
        void OnCollisionExit(Collision collisionInfo)
        {
            if (collisionInfo.gameObject.tag == "Ground")
            {
                isGrounded = false;
                humanoidMeshAnimator.SetBool("IsGrounded", isGrounded);
                comboStartTime = Time.time;
            }
        }


        void EndCombo()
        {
            // Don't try to score instant combos
            if (Time.time - comboStartTime > 0.1f)
                scoreKeeper.EndCombo(totalForwardRotationCount, totalBackwardRotationCount, Time.time - comboStartTime, hasPuked);

            //Reset Combo
            totalForwardRotationCount = 0;
            totalBackwardRotationCount = 0;
            comboStartTime = Time.time;
            hasPuked = false;
        }

        public void ExitDive()
        {
            SceneManager.LoadScene("Title");
        }

        IEnumerator ReloadScene(int secs)
        {
            yield return new WaitForSeconds(secs);
            SceneManager.LoadScene("Dive");
        }


        IEnumerator PlayClipAfter(AudioClip clip, float secs)
        {
            yield return new WaitForSeconds(secs);
            audioSource.PlayOneShot(clip);
        }


        void triggerPuke()
        {
            Debug.Log("Triggering Sickness");
            hasPuked = true;
            audioSource.PlayOneShot(pukeClip);
            StartCoroutine(PukeForXSeconds(0.5f));
        }
        IEnumerator PukeForXSeconds(float secs)
        {
            pukeSystem.Play();
            while (sicknessSlider.value > 0)
            {
                yield return new WaitForSeconds(secs);
                sicknessSlider.value--;
            }
            pukeSystem.Stop();
        }
    

    }
}