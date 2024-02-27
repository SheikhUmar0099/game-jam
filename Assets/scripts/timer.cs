using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText;
    public TMP_Text timerTextOnMapEnd;

    public TMP_Text recordText;
    public TMP_Text recordTextOnMapEnd;

    public Transform endPoint;
    public GameObject mapCompletedPanel;
    public GameObject uiPanel;

    private Rigidbody rb;
    private bool timerRunning = false;
    private float startTime;
    private float bestTime = Mathf.Infinity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mapCompletedPanel.SetActive(false); 
        uiPanel.SetActive(true);
        rb.isKinematic = true;
        UpdateRecordText();
    }

    void Update()
    {
        if (!timerRunning && Input.anyKeyDown)
        {
            StartTimer();
            rb.isKinematic = false;
        }

        if (timerRunning)
        {
            UpdateTimer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == endPoint)
        {
            StopTimer();
            rb.isKinematic = true;
            uiPanel.SetActive(false);
            mapCompletedPanel.SetActive(true);
        }
    }

    void StartTimer()
    {
        timerRunning = true;
        startTime = Time.time;
    }

    void StopTimer()
    {
        timerRunning = false;
        float elapsedTime = Time.time - startTime;

        if (elapsedTime < bestTime)
        {
            bestTime = elapsedTime;
            UpdateRecordText();
        }
    }

    void UpdateTimer()
    {
        float elapsedTime = Time.time - startTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 1000f) % 1000f);

        string minutesString = Mathf.Clamp(minutes, 0, 99).ToString("00");
        string secondsString = Mathf.Clamp(seconds, 0, 59).ToString("00");
        string millisecondsString = Mathf.Clamp(milliseconds, 0, 999).ToString("00");

        timerText.text = string.Format("{0}:{1}:{2}", minutesString, secondsString, millisecondsString);
        timerTextOnMapEnd.text = timerText.text;
    }

    void UpdateRecordText()
    {
        int minutes = Mathf.FloorToInt(bestTime / 60f);
        int seconds = Mathf.FloorToInt(bestTime % 60f);
        int milliseconds = Mathf.FloorToInt((bestTime * 1000f) % 1000f);

        string minutesString = Mathf.Clamp(minutes, 0, 99).ToString("00");
        string secondsString = Mathf.Clamp(seconds, 0, 59).ToString("00");
        string millisecondsString = Mathf.Clamp(milliseconds, 0, 999).ToString("00");

        string recordTimeString = string.Format("{0}:{1}:{2}", minutesString, secondsString, millisecondsString);

        // Update UI text
        recordText.text = recordTimeString;
        recordTextOnMapEnd.text = recordTimeString;

        // Save record time to PlayerPrefs
        PlayerPrefs.SetString("RecordTime", recordTimeString);
        PlayerPrefs.Save(); // Make sure to call Save() after setting PlayerPrefs
    }

}
