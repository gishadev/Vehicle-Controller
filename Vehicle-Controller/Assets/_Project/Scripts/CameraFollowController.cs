using UnityEngine;

namespace Gisha.VehiclePrototype
{
    public class CameraFollowController : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private Vector3 followOffset;
        [SerializeField] private float lookHeight;
        [Space]
        [SerializeField] private float followSpeed;
        [SerializeField] private float rotateSpeed;

        public Vector3 MoveTargetPoint => followTarget.position 
                        + followTarget.forward * followOffset.z
                        + followTarget.up * followOffset.y
                        + followTarget.right * followOffset.x;
        public Vector3 DirectionToTarget => (followTarget.position - transform.position + lookHeight * Vector3.up).normalized;

        private void OnValidate()
        {
            followSpeed = Mathf.Max(followSpeed, 0f);
            rotateSpeed = Mathf.Max(rotateSpeed, 0f);

            transform.position = MoveTargetPoint;
            transform.rotation = Quaternion.LookRotation(DirectionToTarget, Vector3.up);
        }

        private void FixedUpdate()
        {
            MoveToTarget();
            RotateToTarget();
        }

        private void MoveToTarget()
        {
            transform.position = Vector3.Lerp(transform.position, MoveTargetPoint, followSpeed * Time.deltaTime);
        }

        private void RotateToTarget()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(DirectionToTarget, Vector3.up), rotateSpeed * followSpeed);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(MoveTargetPoint, 0.15f);
            Gizmos.DrawRay(transform.position, DirectionToTarget);
        }
    }
}
