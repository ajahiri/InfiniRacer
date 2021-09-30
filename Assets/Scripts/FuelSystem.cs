// Ayush Kanwal 13403187 (29/09/2021); fuel system works now.
// needs another if to stop car when fuel is 0.

// Wai Yan Myint Thu 13334483 (30/09/21) refueling upon collision with gas tanks

using UnityEngine;
using UnityEngine.UI;
public class FuelSystem : MonoBehaviour
{
    public Text FuelText;
    public Image FuelBar;

    float Fuel;
    float maxFuel = 30f;
    float FuelConsumptionRate;
    public float baseInterval = 1f;
    public int lastTime;
    public float timer;

    [SerializeField] GameObject playerCar;


    private void Start()
    {
        FuelConsumptionRate = baseInterval;
        Fuel = maxFuel;
    }

    private void Update()
    {



        if (Fuel > 0)
        {
            if (FuelConsumptionRate > 0)
            {
                FuelConsumptionRate -= Time.deltaTime;
            }
            else
            {
                FuelConsumptionRate = baseInterval;
                Fuel -= 1f;
            }
        }
        else
        {
            // game over
        } 
        ColorChanger();
        //FuelBarFiller();
        FuelText.text = "Fuel: " + Fuel + "%";

    }
    void FuelBarFiller()
    {
        Fuel += 3.0f;
        Debug.Log("Fuel is " + Fuel);
        FuelBar.fillAmount = Mathf.Lerp(FuelBar.fillAmount, (Fuel / maxFuel), FuelConsumptionRate);
        ColorChanger();
    }
    void ColorChanger()
    {
        Color FuelColor = Color.Lerp(Color.red, Color.green, (Fuel / maxFuel));
        FuelBar.color = FuelColor;

    }
    public void fuelPickUp(float fuelCan)
    {
        if (Fuel < maxFuel)
            Fuel += fuelCan;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "boost")
        {
            FuelBarFiller();
            Debug.Log("Collision with gas tank. Fuel is " + Fuel);
        }
    }
}

