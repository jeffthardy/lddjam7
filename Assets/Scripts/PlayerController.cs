using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LDDJAM7
{
    public class PlayerController : MonoBehaviour
    {
        public float maxMoveSpeed = 10;
        public float maxSpinSpeed = 10;

        public float moveSpeed = 1000;
        public float jumpHeight = 10;
        public float rotateSpeed = 10;
        public float spinSpeed = 1000;
        public GameObject humanoidChild;


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

        // This might change depending on space
        private float maxFallSpeed = 2;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            humanoidMeshAnimator = humanoidChild.GetComponent<Animator>();
            isGrounded = false;
            humanoidMeshAnimator.SetBool("IsGrounded", false);
            forwardSpin = 0.0f;
            backwardSpin = 0.0f;


            lastDirection = transform.TransformDirection(Vector3.forward);
            totalForwardRotationCount = 0;
            totalBackwardRotationCount = 0;
        }

        // Update is called once per frame
        void Update()
        {
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
                if((rotation != 0.0f))
                    rb.AddTorque(transform.right * rotateSpeed * -rotation);
                if(forwardSpin != 0)
                    rb.AddTorque(transform.right * spinSpeed* forwardSpin);
                if (backwardSpin != 0)
                    rb.AddTorque(transform.right * -spinSpeed * backwardSpin);
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
            v.y = Vector3.ClampMagnitude(new Vector3(0, yd, 0), maxFallSpeed).y;
            rb.velocity = v;


            var va = rb.angularVelocity;
            va = Vector3.ClampMagnitude(va, maxSpinSpeed);
            rb.angularVelocity = va;



            CountRotations();
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
                    //Debug.Log("totalForwardRotationCount: " + totalForwardRotationCount);
                }

                while (totalBackwardRotationAngle > 360)
                {
                    totalBackwardRotationAngle -= 360;
                    totalBackwardRotationCount++;
                    //Debug.Log("totalBackwardRotationCount: " + totalBackwardRotationCount);
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
            rotation = context.ReadValue<float>();
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
            //Debug.Log("Jump!");
        }

        public void SetFallSpeed(float speed)
        {
            maxFallSpeed = speed;
        }

        void OnCollisionStay(Collision collisionInfo)
        {
            isGrounded = true;
            humanoidMeshAnimator.SetBool("IsGrounded", isGrounded);
        }
        void OnCollisionExit(Collision collisionInfo)
        {
            isGrounded = false;
            humanoidMeshAnimator.SetBool("IsGrounded", isGrounded);
        }
    }
}