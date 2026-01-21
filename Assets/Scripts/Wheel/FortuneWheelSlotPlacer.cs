using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FortuneWheelSlotPlacer : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int numberOfSlots = 8;
    [SerializeField] private float radius = 250f;
    [SerializeField] private float startAngleOffset = 90f;
    [SerializeField] private List<GameObject> createdSlots = new List<GameObject>();

    [ContextMenu("Generate Slots")]
    public void GenerateSlots()
    {
        ClearSlots();

        float angleStep = 360f / numberOfSlots;

        for (int i = 0; i < numberOfSlots; i++)
        {
            float currentAngleDegrees = startAngleOffset - (i * angleStep);
            float currentAngleRad = currentAngleDegrees * Mathf.Deg2Rad;

            float x = Mathf.Cos(currentAngleRad) * radius;
            float y = Mathf.Sin(currentAngleRad) * radius;
            Vector3 position = new Vector3(x, y, 0);

            GameObject slotObj = Instantiate(slotPrefab, transform);
            slotObj.name = $"UI_Slot_{i}";

            RectTransform rect = slotObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = position;
            }
            else
            {
                slotObj.transform.localPosition = position;
            }

            createdSlots.Add(slotObj);
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(this);
        }
#endif
        Debug.Log($"<color=green>{numberOfSlots} slot successfully placed.</color>");
    }

    [ContextMenu("Clear Slots")]
    public void ClearSlots()
    {
        foreach (var slot in createdSlots)
        {
            if (slot != null)
            {
                if (Application.isPlaying) 
                    Destroy(slot);
                else
                    DestroyImmediate(slot);
            }
        }
        createdSlots.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
