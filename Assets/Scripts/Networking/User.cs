using UnityEngine;
using System.Collections;



public class User : MonoBehaviour
{

    public bool isPlayer = false;
    public bool InGame = false;
    bool IsGrounded = false;
    public Vector3 pos;
   

   void Awake()
    {
       
    }

   public User()
    {
       
    }


    void FixedUpdate()
    {


        if (isPlayer)
        {
            pos = gameObject.transform.position;

            float accelerate = Input.GetAxis("Vertical");
            float steer = Input.GetAxis("Horizontal");

            TankController_v1 tk = gameObject.GetComponent<TankController_v1>();
            tk.UpdateWheels(accelerate, steer);
            //Move here

            
        }

    }



}
