using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onCollision : MonoBehaviour
{
    CarController speed;
    [SerializeField] private GameObject[] effects;

    public void Start()
    {
        speed = GetComponent<CarController>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "boost")
        {
            FindObjectOfType<AudioManager>().Play("boost");
            Instantiate(effects[2], col.gameObject.transform.position, col.gameObject.transform.rotation);
            Destroy(col.gameObject);
            GameObject.Find("FuelBar").gameObject.transform.GetComponent<FuelSystem>().FuelPickUp(3.0f);
            speed.GetBoost(2);
        }
        if (col.gameObject.tag == "rock")
        {
            FindObjectOfType<AudioManager>().Play("hit2");
            Instantiate(effects[1], col.gameObject.transform.position, col.gameObject.transform.rotation);
            Destroy(col.gameObject);
        }
        if (col.gameObject.tag == "bomb")
        {
            FindObjectOfType<AudioManager>().Play("bomb");
            Instantiate(effects[0], col.gameObject.transform.position, col.gameObject.transform.rotation);
            Destroy(col.gameObject);
        }
    }
}
