using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController_v1 : MonoBehaviour
{
    //Forward/back
    [Tooltip("Максимальная мощность при езде прямо/назад")]
    public float maxAccel = 2500;
    [Tooltip("Максимальная мощность на поворотах")]
    public float maxSteer = 3000;
    [Tooltip("Тормозная сила")]
    public float maxBrake = 1000;

    public float accelerate = 0;
    public float steer = 0;


    //Rotating on place rot brake is 0
    [Tooltip("Максимальная мощность при повороте на месте")]
    public float maxRotAccel = 2500;
    [Tooltip("Минимальное боковое трение при повороте на месте")]
    public float minOnStayStiffness = 0.06f; 
    [Tooltip("Множитель крутящего момента при повороте во время движения")]
    public float rotateOnMoveMultiply = 2.0f; 
    [Tooltip("Тормозная сила при повороте во время движения.")]
    public float rotateOnMoveBrakeTorque = 2000.0f; 
    [Tooltip("Минимальное боковое трение при повороте во время движения")]
    public float minOnMoveStiffness = 0.05f; 

    [Tooltip("Перфаб колеса")]
    public GameObject WheelPerfab;
    [Tooltip("Центер тяжести")]
    public Transform CoM;
    [Tooltip("Радиус колеса")]
    public float WheelRadius = 0.3f;

    [Tooltip("Скорость движения текстуры")]
    public float TrackTextureSpeed = 2.5f;

    [Tooltip("Леваые гусиницы")]
    public GameObject LeftTracks;
    [Tooltip("Левые колеса")]
    public GameObject[] LeftWheels;
    [Tooltip("Правые гусиницы")]
    public GameObject RightTracks;
    [Tooltip("Правые колеса")]
    public GameObject[] RightWheels;

    public class WheelData
    {
        public Transform wheelTransform;
        public Transform boneTransform;
        public WheelCollider col;
        public Vector3 wheelStartPos;
        public Vector3 boneStartPos;
        public float rotation = 0.0f;
        public Quaternion startWheelAngle;
    }

    protected WheelData[] leftTrackWheelData;
    protected WheelData[] rightTrackWheelData;

    protected float LStartTrackTextureOffset = 0.0f;
    protected float RstartTrackTextureOffset = 0.0f;

    void Awake()
    {


        leftTrackWheelData = new WheelData[LeftWheels.Length];
        rightTrackWheelData = new WheelData[RightWheels.Length];

        for (int i = 0; i < LeftWheels.Length; i++)
        {
            leftTrackWheelData[i] = SetupWheels(LeftWheels[i].transform);
        }

        for (int i = 0; i < RightWheels.Length; i++)
        {
            rightTrackWheelData[i] = SetupWheels(RightWheels[i].transform);
        }

        Vector3 offset = transform.position;
        offset.z += 0.01f;
        transform.position = offset;

    }


    WheelData SetupWheels(Transform wheel)
    {
        WheelData result = new WheelData();

        GameObject go = new GameObject("Collider_" + wheel.name);
        go.transform.parent = transform;
        go.transform.position = wheel.position;
        go.transform.localRotation = Quaternion.Euler(0, wheel.localRotation.y, 0);

        WheelCollider col = (WheelCollider)go.AddComponent(typeof(WheelCollider));
        WheelCollider colPref = WheelPerfab.GetComponent<WheelCollider>();

        col.mass = colPref.mass;
        col.center = colPref.center;
        col.radius = colPref.radius;
        col.suspensionDistance = colPref.suspensionDistance;
        col.suspensionSpring = colPref.suspensionSpring;
        col.forwardFriction = colPref.forwardFriction;
        col.sidewaysFriction = colPref.sidewaysFriction;

        result.wheelTransform = wheel;
        result.col = col;
        result.wheelStartPos = wheel.transform.localPosition;
        result.startWheelAngle = wheel.transform.localRotation;

        return result;
    }

    void Start()
    {
        gameObject.GetComponent<Rigidbody>().centerOfMass = CoM.localPosition;
    }

    void FixedUpdate()
    {
        float accelerate = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");
        UpdateWheels(accelerate, steer);
    }


    public void UpdateWheels(float acc, float st)
    {
 
        float delta = Time.fixedDeltaTime;
        float rpm = SmoothRPM(leftTrackWheelData);

        foreach(WheelData w in leftTrackWheelData)
        {
            CalcMotorForce(w.col, acc, st);

            w.rotation = Mathf.Repeat(w.rotation + delta * rpm * 360.0f / 60.0f, 360.0f);
            w.wheelTransform.localRotation = Quaternion.Euler(w.rotation, w.startWheelAngle.y, w.startWheelAngle.z);
        }
        LStartTrackTextureOffset = Mathf.Repeat(LStartTrackTextureOffset + delta * rpm * TrackTextureSpeed / 60.0f, 1.0f); 
        LeftTracks.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0, -LStartTrackTextureOffset)); 

        rpm = SmoothRPM(rightTrackWheelData);

        foreach (WheelData w in rightTrackWheelData)
        {
           
           CalcMotorForce(w.col, acc, -st);
           w.rotation = Mathf.Repeat(w.rotation + delta * rpm * 360.0f / 60.0f, 360.0f);
           w.wheelTransform.localRotation = Quaternion.Euler(w.rotation, w.startWheelAngle.y, w.startWheelAngle.z);
        }

        RstartTrackTextureOffset = Mathf.Repeat(RstartTrackTextureOffset + delta * rpm * TrackTextureSpeed / 60.0f, 1.0f);
        RightTracks.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0, -RstartTrackTextureOffset));
    }

    private void CalcMotorForce(WheelCollider col, float acc, float st)
    {
        WheelFrictionCurve fc = col.sidewaysFriction;
        if (acc == 0 && st == 0)
        {
            col.brakeTorque = maxBrake;
        }
        else if (acc == 0.0f)
        {  
                col.brakeTorque = 0;
                col.motorTorque = st * maxAccel;
                fc.stiffness = 1.0f + minOnStayStiffness - Mathf.Abs(st);
        }
        else
        {
            col.brakeTorque = 0;
            col.motorTorque = acc*maxAccel;
            if(st < 0)
            {
                col.brakeTorque = rotateOnMoveBrakeTorque;
                col.motorTorque = st * maxAccel * rotateOnMoveMultiply;
                fc.stiffness = 1.0f + minOnMoveStiffness - Mathf.Abs(st);
            }
            if(st > 0)
            {
                col.motorTorque = st * maxAccel * rotateOnMoveMultiply;
                fc.stiffness = 1.0f + minOnMoveStiffness - Mathf.Abs(st);
            }
        }

        if (fc.stiffness > 1.0f) fc.stiffness = 1.0f;		 
        col.sidewaysFriction = fc;

        if (col.rpm > 0 && acc < 0)
        { 
            col.brakeTorque = 0; 
        }
        else if (col.rpm < 0 && acc > 0)
        { 
            col.brakeTorque = 0; 
        }

    }

    private float SmoothRPM(WheelData[] w)
    {
        float rpm = 0.0f;

        List<int> grWheelsInd = new List<int>();

        for (int i = 0; i < w.Length; i++)
        { 
            if (w[i].col.isGrounded)
            { 
                grWheelsInd.Add(i); 
            }
        }

        if (grWheelsInd.Count == 0)
        {    
            foreach (WheelData wd in w)
            {  
                rpm += wd.col.rpm;  			 
            }

            rpm /= w.Length; 

        }
        else
        {  

            for (int i = 0; i < grWheelsInd.Count; i++)
            {  
                rpm += w[grWheelsInd[i]].col.rpm;	 
            }

            rpm /= grWheelsInd.Count;
        }

        return rpm;
    }

  
}