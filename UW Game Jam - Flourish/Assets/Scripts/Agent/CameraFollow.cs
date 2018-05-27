/*
* Created by Daniel Mak
*/

using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [Header("Target to follow")]
    public Transform target;
    public Vector3 offset;
    public float followTime = 0.3f;

    [Header("Zoom setting")]
    public float minZoom;
    public float maxZoom;
    [Range(0f, 50f)] public float zoomSpeed;

    /*
    [Header("Yaw setting")]
    [Range(0f, 5f)] public float yawSpeed;

    [Header("Pitch setting")]
    public float minPitch;
    public float maxPitch;
    [Range(0f, 5f)] public float pitchSpeed;
    */

    private float targetHeight;

    private float zoom;

    /*
    private bool isDragging;
    private Vector2 lastDragPos;
    private float yaw;
    private float pitch;
    */

    private void Start() {
        Renderer renderer = target.GetComponentInChildren<Renderer>();
        if (renderer == null) print("No renderer exist!");
        else {
            targetHeight = renderer.bounds.size.y;
        }

        zoom = (minZoom + maxZoom) / 2;

        /*
        isDragging = false;
        yaw = 0f;
        */
    }

    private void Update() {
        // zoom
        zoom = Mathf.Clamp(zoom - Input.mouseScrollDelta.y * zoomSpeed, minZoom, maxZoom);

        /*
        // yaw & pitch
        if (Input.GetMouseButtonDown(0)) {
            lastDragPos = Input.mousePosition;
            isDragging = true;
        } // when start dragging...
        if (Input.GetMouseButton(0) && isDragging) {
            yaw += (Input.mousePosition.x - lastDragPos.x) * yawSpeed % 360;
            pitch = Mathf.Clamp(pitch - (Input.mousePosition.y - lastDragPos.y) * pitchSpeed, minPitch, maxPitch);
            
            lastDragPos = Input.mousePosition;
        } // during the drag...
        if (Input.GetMouseButtonUp(0) && isDragging) {
            isDragging = false;
        } //when the drag ends...
        */
    }

    private void LateUpdate() {
        // follow the target
        //transform.position = target.position + offset * zoom;
        if (GameManager.instance.isPlaying) { 
            Vector3 vel = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, target.position + offset * zoom, ref vel, followTime);

            /*
            // yaw
            transform.RotateAround(target.position, target.up, yaw);

            // pitch
            transform.position += new Vector3(0, pitch, 0);
            */

            //look at the target
            transform.LookAt(target.position + Vector3.up * targetHeight / 2);
        }
        
    }
}