// Ayush Kanwal 13403187 (29/09/2021); fuel system works now.
// needs another if to stop car when fuel is 0.
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
        FuelBarFiller();
        FuelText.text = "Fuel: " + Fuel + "%";


    }
    void FuelBarFiller()
    {
        FuelBar.fillAmount = Mathf.Lerp(FuelBar.fillAmount, (Fuel / maxFuel), FuelConsumptionRate);
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
}

