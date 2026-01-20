using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStashItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;

    public void Setup(Sprite icon, int amount)
    {
        iconImage.sprite = icon;
        amountText.text = amount.ToString();
    }
}