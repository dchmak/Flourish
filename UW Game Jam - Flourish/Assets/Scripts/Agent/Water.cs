/*
* Created by Daniel Mak
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Water : MonoBehaviour {

    #region singleton
    public static Water instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            Debug.LogWarning("More than one instance! Removed...");
        }
    }
    #endregion

    [Range(0f, 2f)] public float handHeight;
    [Range(0f, 90f)] public float sprayAngle;
    [Range(0f, 5f)] public float waterSize = 1;
    [Range(0f, 10f)] public float waterStregth = 2f;

    [Range(0f, 100f)] public float maxWaterStorage;
    [Range(0f, 100f)] public float waterUsage;

    [Header("Water Storage UI")]
    [Range(0f, 1f)] public float waterRatioThreshold;
    public Image WaterUIBackground;
    public Image WaterUIFill;

    public GameObject waterSplashParticlePrefab;

    private Vector3 waterHitPos;
    public Vector3 WaterHitPos {
        get {
            return waterHitPos;
        }
        private set {
            waterHitPos = value;
        }
    }

    private bool isWatering;
    public bool IsWatering {
        get {
            return isWatering;
        }
        private set {
            isWatering = value;
        }
    }

    private AudioManager audioManager;
    private GameObject waterSplashParticle;
    private float waterStorage;

    private bool isBlinking = false;

    public void Refill(float amount) {
        if (!audioManager.IsPlaying("Fill Water Start") &&
            !audioManager.IsPlaying("Fill Water Continuous") &&
            waterStorage < maxWaterStorage) {
            audioManager.Play("Fill Water Continuous");
        }
        if (waterStorage >= maxWaterStorage) {
            audioManager.Stop("Fill Water Start");
            audioManager.Stop("Fill Water Continuous");
        }

        waterStorage += amount;
        if (waterStorage > maxWaterStorage) waterStorage = maxWaterStorage;
    }

    public void Drain(float amount) {
        waterStorage -= amount;
        if (waterStorage < 0) waterStorage = 0;
    }

    private void Start() {
        audioManager = AudioManager.instance;

        waterStorage = maxWaterStorage;
    }

    private void Update() {
        if (GameManager.instance.isPlaying) {
            if (Input.GetMouseButton(0)) {
                //print("Shooting a ray...");
                if (waterStorage > 0) {
                    int layerMask = 1 << 8;
                    layerMask = ~layerMask;

                    Vector3 forward = transform.forward;
                    float down = forward.magnitude * Mathf.Tan(sprayAngle * Mathf.Deg2Rad);
                    Vector3 dir = forward + Vector3.down * down;

                    //Debug.DrawRay(transform.position + Vector3.up * handHeight, dir, Color.blue);

                    RaycastHit hit;
                    if (Physics.Raycast(transform.position + Vector3.up * handHeight, dir, out hit, Mathf.Infinity, layerMask)) {
                        //print(hit.point.x.ToString() + " | " + hit.point.z.ToString());
                        TextureUpdate texture = hit.collider.GetComponent<TextureUpdate>();
                        if (texture != null) {
                            //texture.Paint(hit.point.x, hit.point.z);
                            texture.Paint(hit.point.x, hit.point.z);
                        }

                        if (waterSplashParticle == null) waterSplashParticle = Instantiate(waterSplashParticlePrefab, hit.point, Quaternion.identity);
                        else waterSplashParticle.transform.position = hit.point;

                        WaterHitPos = hit.point;
                    }

                    Drain(waterUsage * Time.deltaTime);
                    IsWatering = true;
                }
            }
            if (Input.GetMouseButtonUp(0) || waterStorage <= 0) {
                if (waterSplashParticle != null) {
                    waterSplashParticle.GetComponent<ParticleSystem>().Stop();
                    Destroy(waterSplashParticle, 5f);
                    waterSplashParticle = null;
                }
                IsWatering = false;
            }

            if (IsWatering) {
                if (!audioManager.IsPlaying("Watering")) audioManager.Play("Watering");
            } else
                audioManager.Stop("Watering");

            if (waterStorage / maxWaterStorage < waterRatioThreshold && !isBlinking) StartCoroutine(LowOnWaterBlink());

        }
    }

    private void LateUpdate() {
        WaterUIFill.fillAmount = waterStorage / maxWaterStorage;
    }

    private IEnumerator LowOnWaterBlink() {
        isBlinking = true;
        while (waterStorage / maxWaterStorage < waterRatioThreshold) {
            WaterUIBackground.color = new Color(0.8f, 0.2f, 0.2f, 0.35f);
            yield return new WaitForSecondsRealtime(0.5f);
            WaterUIBackground.color = new Color(0f, 0f, 0f, 0.35f);
            yield return new WaitForSecondsRealtime(1f);
        }
        isBlinking = false;
    }
}