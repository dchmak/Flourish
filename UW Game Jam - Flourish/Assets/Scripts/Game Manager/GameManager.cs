/*
* Created by Daniel Mak
*/

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour {

    #region singleton
    public static GameManager instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            Debug.LogWarning("More than one instance! Removed...");
        }
    }
    #endregion

    [Header("Timing")]
    public TextMeshProUGUI timerText;
    [Range(0, 500)] public int initialTime;
    public GameObject bonusTimeTextObject;

    [Header("Scoring")]
    [Range(0f, 50000f)] public float greenScoreScale = 100f;
    [Range(0f, 50000f)] public float flowerScale = 200f;

    [Header("Gameover")]
    public GameObject gameoverPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI ratioText;
    public TextMeshProUGUI flowerText;

    //[HideInInspector] public float greenPixel;

    private int timeLeft;
    public bool isPlaying { get; private set; }

    private AudioManager audioManager;
    private GameObject ground;

    public void AddTime(int timeAdded) {
        bonusTimeTextObject.GetComponent<TextMeshProUGUI>().text = "+" + timeAdded + "s";
        bonusTimeTextObject.SetActive(true);
        //print(bonusTimeTextObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        StartCoroutine(RemoveBonusTimeText());
        timeLeft += timeAdded;
    }

    private void Start () {
        audioManager = AudioManager.instance;

        isPlaying = true;

        timeLeft = initialTime;

        StartCoroutine("TimeCountDown");
	}

    private void LateUpdate() {
        if (isPlaying) {
            timerText.text = "Time: " + timeLeft + "s";

            if (timeLeft <= 0) {
                isPlaying = false;
                audioManager.Stop();
                audioManager.Play("Time Up");
                GameOver();
            }
        }
    }

    private IEnumerator TimeCountDown() {
        while (timeLeft > 0) {
            yield return new WaitForSecondsRealtime(1); 
            timeLeft--;
        }
    }

    private void GameOver() {
        //print("Game Over!");

        GameObject ground = GameObject.FindGameObjectWithTag("Ground");

        // count green "enough" pixels
        float greenPixelRatio = ground.GetComponent<TextureUpdate>().GetGreenPixelRatio();
        //print(greenPixelRatio * greenScoreScale);
        ratioText.text = "Green Ratio: " + greenPixelRatio * 100 + "%";
        int flowerCount = ground.GetComponent<CreateSeeds>().GetNumFlower();
        //print(flowerCount);
        flowerText.text = "Flower Grown: " + flowerCount;
        scoreText.text = "Total Score: " + (greenPixelRatio * greenScoreScale + flowerCount * flowerScale).ToString("F0");

        StartCoroutine(CameraZoomOut(new Vector3(ground.transform.localScale.x / 2, 100, ground.transform.localScale.z / 2)));

        //SceneManager.LoadSceneAsync("Gameover");
    }

    private IEnumerator CameraZoomOut(Vector3 endPos) {
        //print(endPos);

        Camera cam = Camera.main;

        while ( (cam.transform.position - endPos).magnitude > 0.05f ) {
            Vector3 vel = Vector3.zero;
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, endPos, ref vel, 0.1f);
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, Quaternion.Euler(90, 0, 0), 0.1f);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        gameoverPanel.SetActive(true);
    }

    private IEnumerator RemoveBonusTimeText() {
        yield return new WaitForSecondsRealtime(bonusTimeTextObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        bonusTimeTextObject.SetActive(false);
    }
}