using UnityEngine;
using System.Collections.Generic; // Required for using Lists

public class JewelryRestockManager : MonoBehaviour
{
    // These are the items that will be "restocked" (i.e., made active).
    [Tooltip("Drag all the jewelry GameObjects that should be restocked here.")]
    public List<GameObject> jewelryItemsToRestock;

    // This is the public function that will call from the UI Button's OnClick() event.
    public void RestockAllJewelry()
    {
        Debug.Log("Restock button pressed. Attempting to restock items...");

        if (jewelryItemsToRestock == null || jewelryItemsToRestock.Count == 0)
        {
            Debug.LogWarning("JewelryRestockManager: No jewelry items assigned to the restock list.");
            return;
        }

        int restockedCount = 0;
        foreach (GameObject jewelryItem in jewelryItemsToRestock)
        {
            if (jewelryItem != null)
            {
                // "Restocking" here means setting the GameObject to active.
                // (e.g., increasing a quantity, changing a material, etc.)
                jewelryItem.SetActive(true);
                restockedCount++;
                Debug.Log("Restocked: " + jewelryItem.name);
            }
            else
            {
                Debug.LogWarning("JewelryRestockManager: Found a null item in the restock list. Skipping.");
            }
        }

        if (restockedCount > 0)
        {
            Debug.Log(restockedCount + " jewelry item(s) have been restocked.");
        }
        else
        {
            Debug.Log("No items were actively restocked (list might be empty or items were already active/null).");
        }
    }

    
    public void DeactivateJewelryItem(GameObject itemToDeactivate)
    {
        if (itemToDeactivate != null && jewelryItemsToRestock.Contains(itemToDeactivate))
        {
            itemToDeactivate.SetActive(false);
            Debug.Log("Deactivated: " + itemToDeactivate.name + " for potential restocking later.");
        }
        else
        {
            Debug.LogWarning("JewelryRestockManager: Attempted to deactivate an item not in the restock list or item was null.");
        }
    }
}
