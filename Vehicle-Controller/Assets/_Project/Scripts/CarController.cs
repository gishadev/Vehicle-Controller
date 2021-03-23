using UnityEngine;

namespace Gisha.VehiclePrototype
{
    public class CarController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private float motorForce;
        [SerializeField] private float reverseMotorForce;
        [SerializeField] private float brakeForce;
        [SerializeField] private float maxSteerAngle;

        [Header("Wheels")]
        [SerializeField] private Transform[] wheelTransforms;
        [SerializeField] private WheelCollider[] wheelColliders;

        float _horizontalInput, _verticalInput;
        bool _isHandBraking;

        Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            GetInput();

            Steer();
            HandleMotor();

            UpdateWheelPoses();
        }

        private void GetInput()
        {
            _horizontalInput = Input.GetAxis("Horizontal");
            _verticalInput = Input.GetAxis("Vertical");
            _isHandBraking = Input.GetKey(KeyCode.Space);
        }

        private void HandleMotor()
        {
            float footbrake = -1 * Mathf.Clamp(_verticalInput, -1, 0);
            float handbrake = _isHandBraking ? 1f : 0f;

            if (handbrake > 0f)
            {
                // 2 and 3 are rear wheels.
                var brakeTorque = handbrake * brakeForce * 2f;
                wheelColliders[2].brakeTorque = wheelColliders[3].brakeTorque = brakeTorque;
            }

            for (int i = 0; i < 4; i++)
            {
                float thrustTorque = _verticalInput * (motorForce / 2f);
                wheelColliders[i].motorTorque = thrustTorque;

                if (Vector3.Angle(transform.forward, rb.velocity) < 50f)
                    wheelColliders[i].brakeTorque = footbrake * brakeForce;
                
                else if (footbrake > 0)
                {
                    wheelColliders[i].brakeTorque = 0f;
                    wheelColliders[i].motorTorque = -footbrake * reverseMotorForce;
                }
            }
        }

        private void Steer()
        {
            // 0 and 1 are front wheels.
            float steerAngle = _horizontalInput * maxSteerAngle;
            wheelColliders[0].steerAngle = steerAngle;
            wheelColliders[1].steerAngle = steerAngle;
        }

        private void UpdateWheelPoses()
        {
            for (int i = 0; i < 4; i++)
            {
                wheelColliders[i].GetWorldPose(out Vector3 pos, out Quaternion quat);
                wheelTransforms[i].rotation = quat;
                wheelTransforms[i].position = pos;
            }
        }
    }
}