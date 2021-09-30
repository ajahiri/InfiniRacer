using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onCollision : MonoBehaviour
{
    CarController speed;

    public void Start()
    {
        speed = GetComponent<CarController>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "boost")
        {
            FindObjectOfType<AudioManager>().Play("boost");
            Destroy(col.gameObject);
            GameObject.Find("FuelBar").gameObject.transform.GetComponent<FuelSystem>().FuelBarFiller();
            speed.GetBoost(2);
        }
        if (col.gameObject.tag == "rock")
        {
            FindObjectOfType<AudioManager>().Play("hit2");
            Destroy(col.gameObject);
        }
        if (col.gameObject.tag == "bomb")
        {
            FindObjectOfType<AudioManager>().Play("bomb");
            Destroy(col.gameObject);
        }
    }
}
