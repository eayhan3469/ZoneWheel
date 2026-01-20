using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WheelSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;

    public WheelEntry EntryData { get; private set; }

    private Quaternion _fixedRotation = Quaternion.identity;

    public void Setup(WheelEntry entry)
    {
        EntryData = entry;

        iconImage.sprite = entry.ItemData.Icon;
        amountText.text = entry.Amount.ToString();
    }

    private void LateUpdate()
    {
        transform.rotation = _fixedRotation;
    }
}
