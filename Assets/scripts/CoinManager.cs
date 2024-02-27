using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public TMP_Text coinCountText; // Assign in the inspector
    public TMP_Text coinTextOnMapCompleted;
    private int coinCount = 0;

    public GameObject plusOneTextPrefab; // Assign your prefab in the inspector
    public Transform canvasTransform; // Assign your canvas transform here

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            coinCount++;
            UpdateCoinCountUI();

            // Deactivate the coin
            other.gameObject.SetActive(false);

            // Show the +1 text animation
            ShowPlusOneAnimation(other.transform.position);
        }
    }

    void UpdateCoinCountUI()
    {
        coinCountText.text = "" + coinCount.ToString();
        coinTextOnMapCompleted.text = coinCount.ToString();
    }

    void ShowPlusOneAnimation(Vector3 position)
    {
        // Convert the world position of the coin to a screen position
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);

        // Instantiate the +1 prefab at the coin's position
        GameObject plusOneText = Instantiate(plusOneTextPrefab, screenPosition, Quaternion.identity, canvasTransform);
        plusOneText.GetComponent<Text>().text = "+1"; // Ensure your prefab has a Text component

        // Start the animation
        StartCoroutine(AnimateText(plusOneText));
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

        Destroy(textObject); // Destroy the +1 text after animation is complete
    }
}
