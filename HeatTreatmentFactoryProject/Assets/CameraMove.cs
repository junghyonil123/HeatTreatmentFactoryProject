using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] float navigationSpeed = 2.4f;
    [SerializeField] float shiftMultiplier = 2f;
    [SerializeField] float sensitivity = 1.0f;
    [SerializeField] float panSensitivity = 0.5f;
    [SerializeField] float mouseWheelZoomSpeed = 1.0f;
    private Camera cam;
    private Vector3 anchorPoint;
    private Quaternion anchorRot;
    public float detectWallDistance = 100;

    private bool isPanning;
    private void Awake()
    {
        cam = GetComponent<Camera>();
        rig = GetComponent<Rigidbody>();
    
    }

    Rigidbody rig;

    void FixedUpdate()
    {
        MousePanning();
        if (isPanning)
        { return; }

        if (Input.GetMouseButton(1))
        {
            Vector3 move = Vector3.zero;
            rig.velocity = Vector3.zero;
            float speed = navigationSpeed * (Input.GetKey(KeyCode.LeftShift) ? shiftMultiplier : 1f) * Time.deltaTime * 9.1f;
            if (Input.GetKey(KeyCode.W))
            {
                rig.AddForce(transform.forward * speed * 1000);
            }
            if (Input.GetKey(KeyCode.S))
            {
                rig.AddForce(-transform.forward * speed * 1000);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rig.AddForce(transform.right * speed * 1000);
            }
            if (Input.GetKey(KeyCode.A))
            {
                rig.AddForce(-transform.right * speed * 1000);
            }

            //Debug.DrawRay(transform.position, move);

            //if (!Physics.Raycast(transform.position, move, detectWallDistance))
            //{

            //rig.velocity = Vector3.zero;
            //rig.AddForce(move * 1000);
            //transform.Translate(move);
        }
        if (Input.GetMouseButtonDown(1))
        {
            anchorPoint = new Vector3(Input.mousePosition.y, -Input.mousePosition.x);
            anchorRot = transform.rotation;
        }

        if (Input.GetMouseButton(1))
        {
            Quaternion rot = anchorRot;
            Vector3 dif = anchorPoint - new Vector3(Input.mousePosition.y, -Input.mousePosition.x);
            rot.eulerAngles += dif * sensitivity;
            transform.rotation = rot;
        }

        MouseWheeling();

    }

    //Zoom with mouse wheel
    void MouseWheeling()
    {
        float speed = 10 * (mouseWheelZoomSpeed * (Input.GetKey(KeyCode.LeftShift) ? shiftMultiplier : 1f) * Time.deltaTime * 9.1f);

        Vector3 pos = transform.position;
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            pos = pos - (transform.forward * speed);
            transform.position = pos;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            pos = pos + (transform.forward * speed);
            transform.position = pos;
        }
    }

    private float pan_x;
    private float pan_y;
    private Vector3 panComplete;

    void MousePanning()
    {

        pan_x = -Input.GetAxis("Mouse X") * panSensitivity;
        pan_y = -Input.GetAxis("Mouse Y") * panSensitivity;
        panComplete = new Vector3(pan_x, pan_y, 0);

        if (Input.GetMouseButtonDown(2))
        {
            isPanning = true;
        }

        if (Input.GetMouseButtonUp(2))
        {
            isPanning = false;
        }

        if (isPanning)
        {
            transform.Translate(panComplete);
        }
    }

}
