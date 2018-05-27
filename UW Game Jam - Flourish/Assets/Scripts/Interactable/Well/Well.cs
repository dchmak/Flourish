/*
* Created by Daniel Mak
*/

using UnityEngine;

public class Well : Interactable {

    [Range(0f, 100f)] public int waterSupply;

    private Water wateringCan;

    private void OnDrawGizmosSelected() {
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }

    private void Start () {
        wateringCan = Water.instance;
	}

    public override void Interact() {
        //base.Interact();

        //print("Refilling " + waterSupply * Time.deltaTime + "amount of water.");
        wateringCan.Refill(waterSupply * Time.deltaTime);
    }
}