using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CheckpointManager : MonoBehaviour
{
    public Transform player; // Reference to the player object
    public TMP_Text checkpointsText; // Reference to the UI text displaying checkpoints passed
    public Text checkpointTextPrefab; // Assign your prefab in the inspector
    public Transform canvasTransform; // Assign your canvas transform here
    public GameObject[] checkpoints; // Array of checkpoints
    private int currentCheckpointIndex = 0; // Index of the current checkpoint
    private Rigidbody playerRigidbody; // Reference to the player's Rigidbody component
    private bool isBlinking = false; // Flag to control blinking effect
    public float blinkDuration = 2f; // Duration of the blink effect
    public float blinkInterval = 0.2f; // Interval between blink toggles
    public float shakeMagnitude = 0.1f; // Magnitude of the screen shake
    public float shakeDuration = 0.2f; // Duration of the screen shake
    public float respawnDelay = 2f; // Delay before respawning after collision
    private bool isRespawning = false; // Flag to control respawning

    void Start()
    {
        playerRigidbody = player.GetComponent<Rigidbody>();
        UpdateCheckpointsText();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            for (int i = 0; i < checkpoints.Length; i++)
            {
                if (other.gameObject == checkpoints[i])
                {
                    currentCheckpointIndex = i;
                    UpdateCheckpointsText();
                    ShowCheckpointAnimation(other.transform.position, i + 1);
                    break;
                }
            }
        }
        else if (other.CompareTag("Obstacle") && !isBlinking && !isRespawning)
        {
            isRespawning = true;

            // Trigger screen shake
            StartCoroutine(ShakeScreen());

            // Move player back to the last checkpoint
            playerRigidbody.isKinematic = true; // Make player kinematic
            player.position = checkpoints[currentCheckpointIndex].transform.position;

            // Respawn player after a delay
            Invoke("RespawnPlayer", respawnDelay);
        }
    }

    void RespawnPlayer()
    {
        isBlinking = true;
        playerRigidbody.isKinematic = false; // Make player non-kinematic
        StartCoroutine(RespawnAndBlink());
    }

    IEnumerator RespawnAndBlink()
    {
        float elapsedTime = 0f;
        while (elapsedTime < blinkDuration)
        {
            // Toggle player visibility
            player.GetComponent<Renderer>().enabled = !player.GetComponent<Renderer>().enabled;
            yield return new WaitForSeconds(blinkInterval); // Adjust the blinking speed
            elapsedTime += blinkInterval;
        }
        player.GetComponent<Renderer>().enabled = true; // Ensure player is visible after blinking
        isBlinking = false;
        isRespawning = false;
    }

    IEnumerator ShakeScreen()
    {
        Vector3 originalPosition = Camera.main.transform.localPosition;

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            Camera.main.transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.localPosition = originalPosition; // Reset camera position after shake
    }

    void UpdateCheckpointsText()
    {
        checkpointsText.text = "" + (currentCheckpointIndex + 1);
    }

    void ShowCheckpointAnimation(Vector3 position, int checkpointNumber)
    {
        // Convert the world position of the checkpoint to a screen position
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);

        // Adjust the screen position to shift the text to the left side
        float xOffset = Screen.width * 0.3f; // You can adjust this value based on your preference
        screenPosition.x -= xOffset;

        // Instantiate the checkpoint text prefab at the adjusted screen position
        Text checkpointText = Instantiate(checkpointTextPrefab, screenPosition, Quaternion.identity, canvasTransform);
        checkpointText.text = "Checkpoint " + checkpointNumber;

        // Start the animation
        StartCoroutine(AnimateText(checkpointText.gameObject));
    }


    IEnumerator AnimateText(GameObject textObject)
    {
        float moveSpeed = 50f; // Speed of movement upwards
        float fadeDuration = 3f; // Duration of fade
        float elapsedTime = 0;
        Vector3 startPosition = textObject.transform.position;
        Text textComponent = textObject.GetComponent<Text>();

        while (elapsedTime < fadeDuration)
        {
            // Move text upwards
            textObject.transform.position = startPosition + new Vector3(0, elapsedTime * moveSpeed * Time.deltaTime, 0);

            // Fade text out
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(textObject); // Destroy the checkpoint text after animation is complete
    }
}
