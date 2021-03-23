using UnityEngine;

namespace Gisha.VehiclePrototype
{
    public class CarController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private float topSpeed;
        [SerializeField] private float motorForce;
        [SerializeField] private float reverseMotorForce;
        [SerializeField] private float brakeForce;
        [SerializeField] private float maxSteerAngle;

        [Header("Wheels")]
        [SerializeField] private Transform[] wheelTransforms;
        [SerializeField] private WheelCollider[] wheelColliders;

        float _horizontalInput, _verticalInput;
        bool _isHandBraking;

        Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            GetInput();

            Steer();
            HandleMotor();
            LimitSpeed();

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
            // 0 and 1 are front wheels.
            float thrustTorque = _verticalInput * motorForce;
            wheelColliders[0].motorTorque = wheelColliders[1].motorTorque = thrustTorque;

            ApplyBraking();
        }

        private void ApplyBraking()
        {
            float footbrake = -1 * Mathf.Clamp(_verticalInput, -1, 0);
            float handbrake = _isHandBraking ? 1f : 0f;

            for (int i = 0; i < 4; i++)
            {
                if (handbrake > 0f)
                {
                    var brakeTorque = handbrake * brakeForce;
                    wheelColliders[i].brakeTorque = brakeTorque;
                }
                else
                    wheelColliders[i].brakeTorque = 0f;
            }

            if (Vector3.Angle(transform.forward, _rb.velocity) < 50f)
                wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = footbrake * brakeForce;

            if (footbrake > 0)
            {
                wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = 0f;
                wheelColliders[0].motorTorque = wheelColliders[1].motorTorque = -footbrake * reverseMotorForce;
            }

        }

        private void LimitSpeed()
        {
            if (_rb.velocity.magnitude > topSpeed)
                _rb.velocity = _rb.velocity.normalized * topSpeed;
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