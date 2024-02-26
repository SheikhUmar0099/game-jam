using UnityEngine;

namespace COMP30019.Project2
{
    public class CameraFollow : MonoBehaviour
    {
        [Tooltip("The height of the camera above the snowboarder")]
        public float height = 20.0f;

        [Tooltip("The distance of the camera behind the snowboarder")]
        public float distance = 10.0f;

        [Tooltip("How quickly the camera moves to its target position")]
        public float positionSmoothTime = 0.3f;

        [Tooltip("How quickly the camera rotates to its target rotation")]
        public float rotationSmoothTime = 0.3f;

        private Vector3 velocity = Vector3.zero;
        private Quaternion targetRotation;
        private float rotationVelocity;

        private GameObject player;
        private Transform cameraTransform;

        void Start()
        {
            GameObject cameraGameObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (cameraGameObject != null)
            {
                cameraTransform = cameraGameObject.transform;
            }
            else
            {
                Debug.LogError("Camera with tag 'MainCamera' not found.");
            }

            player = GameObject.FindGameObjectWithTag("Player");
        }

        void LateUpdate()
        {
            if (player == null)
            {
                Debug.LogError("Player not found.");
                return;
            }

            // Calculate the target position to the left of the player
            // This positions the camera to the left by using the negative right vector (-player.transform.right) of the player
            // Adjusted to include height and distance to properly position the camera to the left and above the player
            Vector3 targetPosition = player.transform.position + (-player.transform.right * distance) + Vector3.up * height;

            // Smoothly move the camera towards the target position
            cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetPosition, ref velocity, positionSmoothTime);

            // Calculate the target rotation to look at the player
            Quaternion lookAtRotation = Quaternion.LookRotation(player.transform.position - cameraTransform.position);

            // Smoothly rotate the camera to face the player
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, lookAtRotation, rotationSmoothTime * Time.deltaTime);
        }

    }
}
