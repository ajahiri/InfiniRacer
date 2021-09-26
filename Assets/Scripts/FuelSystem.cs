using UnityEngine;
using UnityEngine.UI;
public class FuelSystem : MonoBehaviour
{
    public Text FuelText;
    public Image FuelBar;
    public Image[] FuelPoints;

    float Fuel, maxFuel = 100;
    float FuelConsumptionRate;

    private void Start()
    {
        Fuel = maxFuel;
    }

    private void Update()
    {
        FuelText.text = "Fuel: " + Fuel + "%";
        if (Fuel > maxFuel) Fuel = maxFuel;

        FuelConsumptionRate = 3f * Time.deltaTime;

        FuelBarFiller();
        ColorChanger();
    }

    void FuelBarFiller()
    {
        FuelBar.fillAmount = Mathf.Lerp(FuelBar.fillAmount, (Fuel / maxFuel), FuelConsumptionRate);
        for (int i = 0; i < FuelPoints.Length; i++)
        {
            FuelPoints[i].enabled = !DisplayFuelPoint(Fuel, i);
        }
    }
    void ColorChanger()
    {
        Color FuelColor = Color.Lerp(Color.red, Color.green, (Fuel / maxFuel));
        FuelBar.color = FuelColor;

    }

    bool DisplayFuelPoint(float _Fuel, int pointNumber)
    {
        return ((pointNumber * 10) >= _Fuel);
    }
    public void Damage(float FuelPoints)
    {
        if (Fuel > 0)
            Fuel -= FuelPoints;
    }
    public void Heal(float FuelPoints)
    {
        if (Fuel < maxFuel)
            Fuel += FuelPoints;
    }
}

