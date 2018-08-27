using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlerTurret : MonoBehaviour {

    [Tooltip("Скорость поворота башни")]
    [SerializeField] private float _speedRotate = 7;

    public float SpeedRotateTurret
    {
        get { return _speedRotate; }
        set
        {
            if (value <= 100)
            {
                _speedRotate = value;
            }
            else
            {
                Debug.LogError("Слишком большая скорость вращения башни!!!");
            }
        }
    }

    [Tooltip("Доп.коэфицент скорости поворота башни")]
    public float Coef = 1f;

    private void Update()
    {
        if (Input.GetKey(KeyCode.X))
        {
            this.gameObject.transform.Rotate(0, -_speedRotate * Coef * Time.deltaTime, 0);
        }// Влево
        else if (Input.GetKey(KeyCode.C))
        {
            this.gameObject.transform.Rotate(0, _speedRotate * Coef * Time.deltaTime, 0);
        }// Вправо
    }
}
