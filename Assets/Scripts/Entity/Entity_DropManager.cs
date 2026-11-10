using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity_DropManager : MonoBehaviour
{
    [SerializeField] private GameObject itemGropPrefab;
    [SerializeField] private ItemListDataSO dropData;

    [Header("Drop restrctions")]
    [SerializeField] private int maxRarityAmount = 1600;
    [SerializeField] private int maxItemsToDrop = 4;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            DropItems();
        }
    }

    public virtual void DropItems()
    {
        if (dropData == null)
        {
            Debug.Log("You need to assign drop data on entity" + gameObject.name);
            return;
        }

        List<ItemDataSO> itemsToDrop = RollDrops();
        int dropAmount = Mathf.Min(itemsToDrop.Count, maxItemsToDrop);

        for (int i = 0; i < dropAmount; ++i)
        {
            CreateDropItem(itemsToDrop[i]);
        }
    }

    public void CreateDropItem(ItemDataSO itemToDrop)
    {
        GameObject item = Instantiate(itemGropPrefab, transform.position + new Vector3(0, 0.3f), Quaternion.identity);
        item.GetComponent<Object_ItemPickup>().SetupItem(itemToDrop);
    }

    public List<ItemDataSO> RollDrops()
    {
        List<ItemDataSO> possibleDrops = new List<ItemDataSO>();
        List<ItemDataSO> finalDrops = new List<ItemDataSO>();
        int maxRarityAmount = this.maxRarityAmount;

        foreach (var item in dropData.itemList)
        {
            float dropChance = item.GetDropChance();
            if (Random.Range(0, 100) < dropChance)
                possibleDrops.Add(item);
        }

        possibleDrops = possibleDrops.OrderByDescending(item => item.itemRarity).ToList();

        foreach (var item in possibleDrops)
        {
            if (maxRarityAmount > item.itemRarity)
            {
                finalDrops.Add(item);
                maxRarityAmount -= item.itemRarity;
            }
        }

        return finalDrops;
    }
}
