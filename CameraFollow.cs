using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    public float smoothSpeed = 0f;
    public Vector3 offset;
    public GameObject crosshair;

    void Start(){
        Cursor.visible = false;
    }

    void Update(){
        crosshair.transform.position = transform.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5));
    }

    void FixedUpdate(){
        if(target != null){
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed*Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}
