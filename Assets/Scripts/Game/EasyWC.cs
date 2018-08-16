using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EasyWC : MonoBehaviour {


    [Tooltip("Скорость езды вперед/назад")]
    public float dSpeed = 2;
    [Tooltip("Скорость поворота")]
    public float tSpeed = 80;

    public float angleReg;
    Rigidbody rb;
    BoxCollider GroundChecker;
    bool Grounded = false;
    //Tracks
    [Tooltip("Модель левой гусеницы")]
    public GameObject LTrack;
    [Tooltip("Модель правой гусеницы")]
    public GameObject RTrack; 
    MeshRenderer Lmr, Rmr; // MeshRander of Tracks
    

    

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GroundChecker = GetComponent<BoxCollider>();
        Lmr = LTrack.GetComponent<MeshRenderer>();
        Rmr = RTrack.GetComponent<MeshRenderer>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
            Grounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Grounded = false;
    }

    public void FixedUpdate()
    {
        float mFB = Input.GetAxis("Vertical");
        float mLB = Input.GetAxis("Horizontal");
        float delta = Time.fixedDeltaTime;

        //LeftTracks Material & Offset
        Material Lmat = Lmr.material;
        Vector2 Loffset = Lmat.mainTextureOffset;

        //LeftTracks Material & Offset
        Material Rmat = Rmr.material;
        Vector2 Roffset = Rmat.mainTextureOffset;



        Vector3 rotation = new Vector3(0, mLB, 0) * tSpeed; 
        Vector3 movVR = mFB * transform.forward
;
        Vector3 velocity = movVR.normalized * dSpeed;



        if (Grounded)
        {
            if (velocity != Vector3.zero)
            {
                //Forward
                rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
                rb.AddForce(transform.forward * angleReg);
                
              

                //LeftTracks
                Loffset.y += Time.deltaTime * mFB * dSpeed;
                Lmat.mainTextureOffset = Loffset;
                //RightTracks
                Roffset.y += Time.deltaTime * mFB * dSpeed;
                Rmat.mainTextureOffset = Loffset;
            }

            //Rotation
            //LeftTracks
             Loffset.y += Time.deltaTime * mLB * (tSpeed / 50);
             Lmat.mainTextureOffset = Loffset;

                //RightTracks
             Roffset.y -= Time.deltaTime * mLB * (tSpeed / 50);
             Rmat.mainTextureOffset = Roffset;
                
            float turn = mLB * tSpeed * Time.deltaTime;
            Quaternion quat = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * quat);
        }
            
    }
}

