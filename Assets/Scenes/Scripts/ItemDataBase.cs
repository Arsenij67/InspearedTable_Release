

using UnityEngine;

public static class ItemDataBase
{
    public static Item[] items { get; set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()

    {
        
        items = Resources.LoadAll<Item>(path: "Items/");

    
    }

}
