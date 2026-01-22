using System.Collections.Generic;
using UnityEngine;

public class UIStashController : MonoBehaviour
{
    [SerializeField] private RunManager runManager;

    [SerializeField] private Transform contentParent;
    [SerializeField] private UIStashItem stashItemPrefab;

    private List<UIStashItem> _spawnedItems = new List<UIStashItem>();

    private void Start()
    {
        runManager.Stash.OnStashUpdated += RefreshUI;
        runManager.Stash.OnStashCleared += ClearUI;
    }

    private void RefreshUI(List<CollectedItem> allItems, CollectedItem lastUpdated)
    {
        //TODO: Optimize this to not clear and respawn everything
        ClearUI();

        foreach (var item in allItems)
        {
            UIStashItem uiItem = Instantiate(stashItemPrefab, contentParent);
            uiItem.Setup(item.Data.Icon, item.Amount);
            _spawnedItems.Add(uiItem);
        }
    }

    private void ClearUI()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        _spawnedItems.Clear();
    }

    private void OnDestroy()
    {
        runManager.Stash.OnStashUpdated -= RefreshUI;
        runManager.Stash.OnStashCleared -= ClearUI;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (runManager == null)
        {
            runManager = GetComponentInParent<RunManager>(true);
            
            if (!runManager)
                Debug.LogError($"[UIStashController] Error: RunManager not found in parent of '{gameObject.name}'.");
        }

        if (contentParent == null)
        {
            var transforms = GetComponentsInChildren<Transform>(true);
            bool found = false;
            foreach (var transform in transforms)
            {
                if (transform.name == "UI_Stash_Content")
                {
                    contentParent = transform;
                    found = true;
                    break;
                }
            }
            if (!found)
                Debug.LogError($"[UIStashController] Error: Transform named 'UI_Content' not found in children of '{gameObject.name}'.");
        }
    }
#endif
}
