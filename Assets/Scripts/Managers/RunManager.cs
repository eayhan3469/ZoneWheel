using UnityEngine;

public class RunManager : MonoBehaviour
{
    [SerializeField] private Stash currentStash = new Stash();

    public Stash Stash => currentStash;

    public void StartNewRun()
    {
        currentStash.Clear();
    }

    public void HandleReward(WheelEntry entry)
    {
        currentStash.AddItem(entry.ItemData, entry.Amount);
        Debug.Log($"Added {entry.Amount} of {entry.ItemData.DisplayName} to stash.");
    }

    public void GiveUp()
    {
        Debug.Log("Given up! Clearing stash.");
        currentStash.Clear();
    }
}
