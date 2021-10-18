using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onCollision : MonoBehaviour
{
    [SerializeField] private GameObject[] effects;

    public void Start()
    {
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "boost")
        {
            FindObjectOfType<AudioManager>().Play("boost");
            Instantiate(effects[2], col.gameObject.transform.position, col.gameObject.transform.rotation);
            Destroy(col.gameObject);
            gameObject.GetComponent<CarController>().GetBoost(3f);
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
        if (col.gameObject.tag == "banana")
        {
            gameObject.GetComponent<CarController>().Drift();
            FindObjectOfType<AudioManager>().Play("banana");
            Destroy(col.gameObject);
            
        }
        if (col.gameObject.tag == "coin")
        {
            FindObjectOfType<AudioManager>().Play("coin");
            Destroy(col.gameObject);
            FindObjectOfType<Scoring>().CoinPickup();
        }
        if (col.gameObject.tag == "fuel")
        {
            FindObjectOfType<AudioManager>().Play("fuel");
            Destroy(col.gameObject);
            GameObject.Find("FuelBar").gameObject.transform.GetComponent<FuelSystem>().FuelPickUp(15.0f);
        }
    }
}
