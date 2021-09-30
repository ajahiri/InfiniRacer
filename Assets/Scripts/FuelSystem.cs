// Ayush Kanwal 13403187 (29/09/2021); fuel system works now.
// needs another if to stop car when fuel is 0.

// Wai Yan Myint Thu 13334483 (30/09/21) refueling upon collision with gas tanks

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
            //
        } 
        ColorChanger();
        FuelBarFiller();
        FuelText.text = "Fuel: " + Fuel + "%";

    }
    public void FuelBarFiller()
    {
        FuelBar.fillAmount = Mathf.Lerp(FuelBar.fillAmount, (Fuel / maxFuel), FuelConsumptionRate);
    }
    void ColorChanger()
    {
        Color FuelColor = Color.Lerp(Color.red, Color.green, (Fuel / maxFuel));
        FuelBar.color = FuelColor;

    }
    public void FuelPickUp(float fuelCan)
    {
        if (Fuel < maxFuel)
        {
            if (Fuel == 28)
            {
                fuelCan = 2;
                Fuel += fuelCan;
            } 
            else if (Fuel == 29)
            {
                fuelCan = 1;
                Fuel += fuelCan;
            } 
            else if (Fuel == 30)
            {
                fuelCan = 0;
                Fuel += fuelCan;
            } 
            else
            {
                Fuel += fuelCan;
            }
        }           
    }  
}

