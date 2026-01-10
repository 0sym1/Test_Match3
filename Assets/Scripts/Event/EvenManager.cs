using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvenManager
{
    public static Action<Item> OnItemCollected;
    public static Action<Vector2> OnItemMoved;
    public static Action OnCheckGameWin;

    public static void InvokeItemCollected(Item item)
    {
        OnItemCollected?.Invoke(item);
    }

    public static void InvokeItemMoved(Vector2 position)
    {
        OnItemMoved?.Invoke(position);
    }

    public static void InvokeCheckGameWin()
    {
        OnCheckGameWin?.Invoke();
    }
}
