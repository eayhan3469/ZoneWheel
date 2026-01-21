using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WheelData))]
public class WheelDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("WheelName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("WheelBackground"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("WheelIndicator"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Wheel Entries (Auto-Balance: 100%)", EditorStyles.boldLabel);

        SerializedProperty entriesProp = serializedObject.FindProperty("WheelEntries");

        for (int i = 0; i < entriesProp.arraySize; i++)
        {
            SerializedProperty entry = entriesProp.GetArrayElementAtIndex(i);
            SerializedProperty itemData = entry.FindPropertyRelative("ItemData");
            SerializedProperty amount = entry.FindPropertyRelative("Amount");
            SerializedProperty dropChance = entry.FindPropertyRelative("DropChance");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            string header = itemData.objectReferenceValue != null ? (itemData.objectReferenceValue as ItemData).DisplayName : $"Element {i}";

            EditorGUILayout.LabelField(header, EditorStyles.miniBoldLabel);

            EditorGUILayout.PropertyField(itemData);
            EditorGUILayout.PropertyField(amount);

            EditorGUI.BeginChangeCheck();

            int oldVal = dropChance.intValue;
            int newVal = EditorGUILayout.IntSlider("Drop Chance (%)", oldVal, 0, 100);

            if (EditorGUI.EndChangeCheck())
            {
                dropChance.intValue = newVal;
                BalanceProportionally(entriesProp, i, newVal);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        if (GUILayout.Button("Add Wheel Entry"))
            entriesProp.InsertArrayElementAtIndex(entriesProp.arraySize);

        if (GUILayout.Button("Remove Last Entry") && entriesProp.arraySize > 0)
            entriesProp.DeleteArrayElementAtIndex(entriesProp.arraySize - 1);

        serializedObject.ApplyModifiedProperties();
    }

    private void BalanceProportionally(SerializedProperty list, int changedIndex, int newValue)
    {
        int totalCount = list.arraySize;
        if (totalCount <= 1) 
            return;

        int targetRemaining = 100 - newValue;
        float currentOthersTotal = 0f;

        for (int k = 0; k < totalCount; k++)
        {
            if (k == changedIndex)
                continue;

            currentOthersTotal += list.GetArrayElementAtIndex(k).FindPropertyRelative("DropChance").intValue;
        }

        int accumulated = 0;

        bool useEqualDistribution = (currentOthersTotal == 0);

        for (int k = 0; k < totalCount; k++)
        {
            if (k == changedIndex) 
                continue;

            SerializedProperty prop = list.GetArrayElementAtIndex(k).FindPropertyRelative("DropChance");
            int newVal = 0;

            if (useEqualDistribution)
            {
                newVal = targetRemaining / (totalCount - 1);
            }
            else
            {
                float ratio = (float)prop.intValue / currentOthersTotal;
                newVal = Mathf.RoundToInt(targetRemaining * ratio);
            }

            prop.intValue = newVal;
            accumulated += newVal;
        }

        int error = targetRemaining - accumulated;

        if (error != 0)
        {
            int bestCandidateIndex = -1;
            int maxVal = -1;

            for (int k = 0; k < totalCount; k++)
            {
                if (k == changedIndex) 
                    continue;

                int val = list.GetArrayElementAtIndex(k).FindPropertyRelative("DropChance").intValue;
                if (val > maxVal)
                {
                    maxVal = val;
                    bestCandidateIndex = k;
                }
            }

            if (bestCandidateIndex != -1)
            {
                SerializedProperty fixProp = list.GetArrayElementAtIndex(bestCandidateIndex).FindPropertyRelative("DropChance");

                if (fixProp.intValue + error >= 0)
                    fixProp.intValue += error;
                else
                    list.GetArrayElementAtIndex(changedIndex).FindPropertyRelative("DropChance").intValue += error;
            }
        }
    }
}
