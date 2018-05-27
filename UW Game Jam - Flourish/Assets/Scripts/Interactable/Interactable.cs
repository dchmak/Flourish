/*
* Created by Daniel Mak
*/

using UnityEngine;

public class Interactable : MonoBehaviour {

    [Range(0f, 5f)] public float interactRadius;

    public virtual void Interact() {
        print("Interacting with " + name);
    }
}