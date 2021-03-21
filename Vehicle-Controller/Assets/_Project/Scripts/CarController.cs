using UnityEngine;

namespace Gisha.VehiclePrototype
{
    public class CarController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private float motorForce;
        [SerializeField] private float brakeForce;
        [SerializeField] private float maxSteerAngle;

        [Header("Wheels")]
        [SerializeField] private Transform[] wheelTransforms;
        [SerializeField] private WheelCollider[] wheelColliders;

        float _horizontalInput, _verticalInput;
        bool _isHandBraking;

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
            // 0 and 1 are front wheels.
            wheelColliders[0].motorTorque = _verticalInput * motorForce;
            wheelColliders[1].motorTorque = _verticalInput * motorForce;

            ApplyBraking();
        }

        private void ApplyBraking()
        {
            float brakeInput = _isHandBraking ? 1f : 0f;

            for (int i = 0; i < 4; i++)
                wheelColliders[i].brakeTorque = brakeInput * brakeForce;
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