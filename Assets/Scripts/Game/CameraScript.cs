using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    [Tooltip("Объект, за которым нужно смотреть")]
    public Transform SeeTarget;
    [Tooltip("Максимальная верхняя точка")]
    public float maxCamPos = 6.0f;
    [Tooltip("Максимальная нижняя точка")]
    public float minCamPos = 1.0f;
    [Tooltip("Скорость перемещения камеры вокруг оси Y")]
    public float camSpeed = 0.05f;
    [Tooltip("Сглаженность камеры")]
    public float smoothSpeed = 0.2f;
    [Tooltip("Режим мышки")]
    public bool UseMouse = false;
    [Tooltip("Скорость вращения мышки")]
    public float rotateSpeed = 5f;
    private Vector3 offset;
    private float lastY = 2.15f;

    // Use this for initialization
    void Start()
    {
        offset = SeeTarget.transform.position - transform.position;
    }


    // Update is called once per frame
    void LateUpdate() {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UseMouse = !UseMouse;
        }

        if (UseMouse) MouseCon(); else KeyCon();
    }

    void KeyCon()
    {
        gameObject.transform.localPosition = new Vector3(0.19f, lastY, -3.59f);
        transform.LookAt(SeeTarget);
        if (Input.GetAxis("Zoom") != 0)
        {
            if (Input.GetAxis("Zoom") > 0)
            {
                if (gameObject.transform.localPosition.y < maxCamPos)
                {
                    gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y + camSpeed, gameObject.transform.localPosition.z);
                }
            }

            if (Input.GetAxis("Zoom") < 0)
            {
                if (gameObject.transform.localPosition.y > minCamPos)
                {
                    gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y - camSpeed, gameObject.transform.localPosition.z);

                }
            }

            lastY = gameObject.transform.localPosition.y;
        }
    }

     
    void MouseCon() {

        
        //gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, 2, gameObject.transform.localPosition.z);
        float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
        SeeTarget.transform.Rotate(0, horizontal, 0);

        float desiredAngle = SeeTarget.transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
        transform.position = SeeTarget.transform.position - (rotation * offset);

        transform.LookAt(SeeTarget);
    }


    }

    

