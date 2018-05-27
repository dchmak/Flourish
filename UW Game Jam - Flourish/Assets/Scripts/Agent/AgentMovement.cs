/*
* Created by Daniel Mak
*/

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour {
    
    public NavMeshAgent agent;
    
    [Header("Following")]
    [Range(0f, 2f)] public float followInterval;
    [Range(0f, 10f)] public float followSmoothTime;


    [Header("Animation")]
    [Range(0f, 1f)] public float animationSmoothTime;

    [Header("Watering")]
    [Range(0f, 10f)] public float noramlWalkingSpeed;
    [Range(0f, 10f)] public float wateringWalkingSpeed;

    private Camera cam;
    
    private Interactable selecting;
    private Interactable following;

    private Animator animator; // animator of the model
    private Water water; // the watering can instance
    private AudioManager audioManager;


    private bool startInteract = false;

    private void Start() {
        cam = Camera.main;

        animator = GetComponentInChildren<Animator>(); // get the animator component
        water = Water.instance; // get the watering can instance
        audioManager = AudioManager.instance;

        agent.speed = noramlWalkingSpeed;
    }

    private void Update () {
        if (GameManager.instance.isPlaying) {
            //print(water.IsWatering);
            if (water.IsWatering) agent.speed = wateringWalkingSpeed;
            else agent.speed = noramlWalkingSpeed;

            /*
            if (Input.GetMouseButtonUp(0)) {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit)) {
                    Interactable interactable = hit.collider.GetComponent<Interactable>();

                    if (selecting != null) selecting.OnDeselected();
                    if (interactable != null) {
                        interactable.OnSelected();
                    }
                    selecting = interactable;
                }
            } // If clicked on interactable, select it
            */

            if (Input.GetMouseButton(1)) {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit)) {
                    Interactable interactable = hit.collider.GetComponent<Interactable>();
                    following = interactable;

                    if (interactable == null) {
                        agent.stoppingDistance = 0f;
                        agent.SetDestination(hit.point);
                    } else {
                        agent.stoppingDistance = interactable.interactRadius;
                        StartCoroutine("FollowTarget");
                    }


                }
            } // move agent to clicked position

            if (following != null) {
                float distance = (transform.position - following.transform.position).magnitude;
                if (distance < following.interactRadius) {  // check if in range for interaction 

                    //print(distance);
                    if (!startInteract) {
                        audioManager.Play("Fill Water Start");
                        startInteract = true;
                    }
                    following.Interact();
                }

                agent.updateRotation = false; // handle rotation by myself when following something

                Vector3 dir = (following.transform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, followSmoothTime * Time.deltaTime);
            } else {
                agent.updateRotation = true; // handle rotation by the agent
                startInteract = false;
                audioManager.Stop("Fill Water Start");
                audioManager.Stop("Fill Water Continuous");
            }

            // set animation by determining the current speed
            float speedPercent = agent.velocity.magnitude / noramlWalkingSpeed;
            animator.SetFloat("speedPercent", speedPercent, animationSmoothTime, Time.deltaTime);
        }

        //print(agent.destination);
	}
    
    private IEnumerator FollowTarget() {
        while (following != null) { 
            agent.SetDestination(following.transform.position);

            yield return new WaitForSeconds(followInterval);
        }
    }
}