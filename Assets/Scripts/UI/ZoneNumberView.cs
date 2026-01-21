using UnityEngine;
using TMPro;

public class ZoneNumberView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numberText;

    public void Setup(int number, Color typeColor)
    {
        numberText.text = number.ToString();
        numberText.color = typeColor;
    }
}