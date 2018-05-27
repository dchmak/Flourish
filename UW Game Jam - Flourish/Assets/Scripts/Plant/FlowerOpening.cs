/*
* Created by Daniel Mak
*/

using UnityEngine;

public class FlowerOpening : MonoBehaviour {

    [Range(0f, 10f)] public float openTime;
    public Vector3 openedScale = new Vector3(0.5f, 1, 0.5f);

    private Vector3 closedScale = new Vector3(0.01f, 0.01f, 0.01f);
    private float timePassed = 0f;

    private void Start () {
        //print(transform.localScale);
        transform.localScale = closedScale;
    }

    private void Update() {
        if (timePassed < openTime) {
            //print(timePassed);
            transform.localScale = Vector3.Lerp(closedScale, openedScale, timePassed / openTime);
            timePassed += Time.deltaTime;
        }
    }
}