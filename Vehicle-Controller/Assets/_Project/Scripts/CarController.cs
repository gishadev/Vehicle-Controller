using UnityEngine;

namespace Gisha.VehiclePrototype
{
    public class CarController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private float topSpeed = 150f;
        [SerializeField] private float motorForce;
        [SerializeField] private float maxSteerAngle;

        [Header("Wheels")]
        [SerializeField] private GameObject[] wheels;
        [SerializeField] private WheelCollider[] wheelColliders;

        Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Steer(h);
            Accelerate(v);

            UpdateWheelPoses();
        }

        private void Accelerate(float accel)
        {
            // 2 and 3 are rear wheels.
            wheelColliders[2].motorTorque = accel * motorForce;
            wheelColliders[3].motorTorque = accel * motorForce;
        }

        private void Steer(float steering)
        {
            // 0 and 1 are front wheels.
            float steerAngle = steering * maxSteerAngle;
            wheelColliders[0].steerAngle = steerAngle;
            wheelColliders[1].steerAngle = steerAngle;
        }

        private void UpdateWheelPoses()
        {
            for (int i = 0; i < 4; i++)
            {
                wheelColliders[i].GetWorldPose(out Vector3 pos, out Quaternion quat);
                wheels[i].transform.rotation = quat;
                wheels[i].transform.position = pos;
            }
        }
    }
}