using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CheckpointManager : MonoBehaviour
{
    public Transform player; // Reference to the player object
    public TMP_Text checkpointsText; // Reference to the UI text displaying checkpoints passed
    private int checkpointsPassed = 0; // Counter for checkpoints passed
    private Vector3 lastCheckpointPosition; // Position of the last checkpoint passed
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
        lastCheckpointPosition = player.position;
        playerRigidbody = player.GetComponent<Rigidbody>();
        UpdateCheckpointsText();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            lastCheckpointPosition = other.transform.position;
            checkpointsPassed++;
            UpdateCheckpointsText();
        }
        else if (other.CompareTag("Obstacle") && !isBlinking && !isRespawning)
        {
            isRespawning = true;

            // Trigger screen shake
            StartCoroutine(ShakeScreen());

            // Move player back to the last checkpoint
            playerRigidbody.isKinematic = true; // Make player kinematic
            player.position = lastCheckpointPosition;

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
        checkpointsText.text = "" + checkpointsPassed;
    }
}
