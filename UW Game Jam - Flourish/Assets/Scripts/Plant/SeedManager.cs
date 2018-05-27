/*
* Created by Daniel Mak
*/

using UnityEngine;

public class SeedManager : MonoBehaviour {

    public GameObject flowerPrefab;

    public float minGrowthHeight = 0.01f;
    public float maxGrowthHeight = 3f;
    public float maxGrowthState = 10f;

    [Range(0, 50)] public int bonusTime = 10;

    public bool hasFlower {
        get {
            return flower != null;
        }
    }

    private Water water;
    private float growthState = 0f;

    private GameObject flower;

	public void Grow(float growthValue) {
        growthState += growthValue * Time.deltaTime;
        if (growthState > maxGrowthState) growthState = maxGrowthState;

        UpdateObject();
    }

    public void Trample(float trampleValue) {
        growthState -= trampleValue * Time.deltaTime;
        if (growthState < 0) growthState = 0;

        UpdateObject();
    }

    private void Start() {
        water = Water.instance;

        UpdateObject();
    }

    private void Update() {
        GetComponent<MeshRenderer>().enabled = growthState > 0;

        if ( (water.WaterHitPos - transform.position).magnitude < water.waterSize && water.IsWatering) {
            //print(water.WaterHitPos + " | " + transform.position);
            //print("Getting watered!");
            Grow(water.waterStregth);
        }
    }

    private void UpdateObject() {
        transform.localScale = new Vector3(
            transform.localScale.x,
            transform.localScale.y,
            Mathf.Lerp(minGrowthHeight, maxGrowthHeight, growthState / maxGrowthState));

        //print(growthState);
        // fully grown
        if (growthState >= maxGrowthState) {
            if (flower == null) {
                flower = Instantiate(flowerPrefab);
                flower.transform.SetParent(transform);
                flower.transform.localPosition = new Vector3(0, 0, 2);

                GameManager.instance.AddTime(bonusTime);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 1);
    }
}