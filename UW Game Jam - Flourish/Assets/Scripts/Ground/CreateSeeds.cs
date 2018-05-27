/*
* Created by Daniel Mak
*/

using UnityEngine;

public class CreateSeeds : MonoBehaviour {

    public GameObject seedPrefab;
    public Transform seedsHolder;

    [Range(0, 10000)] public int numSeeds;
    [Range(0f, 10f)] public float seedsSpacing;
    [Range(0f, 10f)] public float awayFromWell;

    //private Vector2 groundSize;
    private GameObject[] seeds;

    private void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
    }

    public int GetNumFlower() {
        int count = 0;
        foreach (GameObject seed in seeds) {
            if (seed.GetComponent<SeedManager>().hasFlower) {
                count++;
            }
        }
        return count;
    }

    private void Start() {
        seeds = new GameObject[numSeeds];

        //groundSize = new Vector2(transform.localScale.x, transform.localScale.z);

        for (int i = 0; i < numSeeds; i++) {
            bool success = false;
            Vector3 newPos;
            do {
                /*
                float randX = Random.Range(0, groundSize.x);
                float randY = Random.Range(0, groundSize.y);
                */

                float radius = transform.localScale.x / 2f;
                float angle = Random.Range(0f, 2f * Mathf.PI);
                float distance = Random.Range(0.1f, 1f) * radius;

                float randX = distance * Mathf.Cos(angle) + radius;
                float randY = distance * Mathf.Sin(angle) + radius;

                newPos = new Vector3(randX, 0, randY);

                if (newPos.magnitude > awayFromWell) {
                    bool overlap = false;
                    int count = 0;
                    while (count < i && !overlap) {
                        //print(count.ToString() + " " + i.ToString());
                        overlap = (seeds[count].transform.position - newPos).magnitude < seedsSpacing;
                        count++;
                    }

                    if (!overlap) success = true;
                }
            } while (!success);

            GameObject newSeed = Instantiate(seedPrefab);
            newSeed.transform.SetParent(seedsHolder);
            newSeed.transform.position = newPos;

            seeds[i] = newSeed;
        }
    }
}