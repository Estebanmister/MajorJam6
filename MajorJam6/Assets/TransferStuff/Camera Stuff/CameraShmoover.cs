using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a camera. It sees all. You cannot escape its gaze.


public class CameraShmoover : MonoBehaviour
{
    [SerializeField] GameObject cam;
    [SerializeField] float moveSpeed;
    [SerializeField] float zoomSpeed;
    [SerializeField] float minZoomDist;
    [SerializeField] float maxZoomDist;
    [SerializeField] float rotationSpeed;
    [SerializeField] float minX = 0;
    [SerializeField] float maxX = 80f;
    Ground ground;

    float X;
    float Y;
    bool lockedCam;
    float zoomAmt;
    GameObject lockTarget;


    // Start is called before the first frame update
    void Start()
    {
        zoomAmt = (minZoomDist + maxZoomDist) / 2;
        lockedCam = false;
        ground = GameObject.FindGameObjectWithTag("ground").GetComponent<Ground>();
    }

    // Update is called once per frame
    void Update()
    {
        cam.GetComponent<Camera>().orthographicSize = zoomAmt;
        //cam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = zoomAmt; //If using perspective, comment this line out.
        if (!lockedCam)
        {
            Move();
        } else
        {
            FocusOnPosition(lockTarget.transform.position);
        }
        Rotate();
        Zoom();
    }

    //Locked Cam is for if you want to focus on an element in the world, it locks the location of the camera while allowing you to rotate around it and zoom in and out
    public void CamLockStatus(bool newLock, GameObject newLockTarget)
    {
        lockedCam = newLock;
        lockTarget = newLockTarget;
       
    }
    void Move()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(transform.forward.x, 0, transform.forward.z) * zInput + new Vector3(cam.transform.right.x, transform.right.y, cam.transform.right.z) * xInput;
        transform.position += dir * moveSpeed * Time.deltaTime;
        if(transform.position.x > ground.size){
            Vector3 pos = transform.position;
            pos.x = ground.size;
            transform.position = pos;
        }
        if(transform.position.z > ground.size){
            Vector3 pos = transform.position;
            pos.z = ground.size;
            transform.position = pos;
        }
        if(transform.position.x < 0){
            Vector3 pos = transform.position;
            pos.x = 0;
            transform.position = pos;
        }
        if(transform.position.z < 0){
            Vector3 pos = transform.position;
            pos.z = 0;
            transform.position = pos;
        }
    }

    void Zoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        
        if (zoomAmt < minZoomDist && scrollInput < 0.0f)
            return;
        else if (zoomAmt > maxZoomDist && scrollInput > 0.0f)
            return;
        zoomAmt += scrollInput * zoomSpeed;
       

        //This is for if the cam is perspective 

        /*float dist = Vector3.Distance(transform.position, cam.transform.position);
        if (dist < minZoomDist && scrollInput > 0.0f)
            return;
        else if (dist > maxZoomDist && scrollInput < 0.0f)
            return;
        cam.transform.position += cam.transform.forward * scrollInput * zoomSpeed;
        */
    }

    public void FocusOnPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    void Rotate()
    {
        if (Input.GetMouseButton(1))
        {

            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * rotationSpeed, -Input.GetAxis("Mouse X") * rotationSpeed, 0));
            if (transform.rotation.eulerAngles.x <= maxX && transform.rotation.eulerAngles.x >= minX)
            {
                X = transform.rotation.eulerAngles.x;
            }
            Y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, Y, 0);

        }
    }

 
}
