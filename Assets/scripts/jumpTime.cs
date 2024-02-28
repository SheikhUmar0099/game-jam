using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JumpTime : MonoBehaviour
{
    public Text jumpTimeText;
    private bool isJumping;
    private float jumpStartTime;
    private Color originalColor;

    void Start()
    {
        // Initially, jump text is not active
        jumpTimeText.gameObject.SetActive(false);
        // Store the original color
        originalColor = jumpTimeText.color;
    }

    void Update()
    {
        // Check if the player is jumping
        if (isJumping)
        {
            // Calculate jump time
            float jumpTime = Time.time - jumpStartTime;

            // Display jump time only if it exceeds 1 second
            if (jumpTime > 1f)
            {
                jumpTimeText.gameObject.SetActive(true); // Ensure text is active
                                                         // Display jump time in seconds and milliseconds
                jumpTimeText.text = jumpTime.ToString("F2") + " s";
            }
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        // Check if collided with terrain
        if (collision.gameObject.CompareTag("Terrain"))
        {
            // If the player was jumping, stop the jump
            if (isJumping)
            {
                isJumping = false;
                // Fade out the jump time text
                StartCoroutine(FadeOutJumpText());
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Check if left terrain
        if (collision.gameObject.CompareTag("Terrain"))
        {
            // Start recording jump time
            isJumping = true;
            jumpStartTime = Time.time;
            // Activate the jump time text
            //jumpTimeText.gameObject.SetActive(true);
            // Set original color to red
            Color redColor = Color.red;
            redColor.a = 1.0f; // Set alpha to 100%
            jumpTimeText.color = redColor;
        }
    }

    IEnumerator FadeOutJumpText()
    {
        // Gradually decrease alpha of jump text
        float duration = 1f;
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            Color fadeColor = jumpTimeText.color;
            fadeColor.a = Mathf.Lerp(1.0f, 0.0f, t);
            jumpTimeText.color = fadeColor;
            yield return null;
        }

        // Reset text and deactivate
        jumpTimeText.color = originalColor;
        jumpTimeText.text = "0.00 s";
        jumpTimeText.gameObject.SetActive(false);
    }
}
