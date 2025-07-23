using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// Helper class to run code on Unity's main thread from other threads (like WebSocket events)
public class UnityMainThreadDispatcher : MonoBehaviour
{
  private static readonly Queue<System.Action> _executionQueue = new Queue<System.Action>();
  private static UnityMainThreadDispatcher _instance = null;

  public static UnityMainThreadDispatcher Instance()
  {
    if (_instance == null)
    {
      _instance = FindObjectOfType<UnityMainThreadDispatcher>();
      if (_instance == null)
      {
        var go = new GameObject("UnityMainTreadDispatcher");
        _instance = go.AddComponent<UnityMainThreadDispatcher>();
        DontDestroyOnLoad(go);
      }
    }
    return _instance;
  }

  public void Enqueue(System.Action action)
  {
    lock (_executionQueue)
    {
      _executionQueue.Enqueue(action);
    }
  }

  private void Update()
  {
    lock (_executionQueue)
    {
      while (_executionQueue.Count > 0)
      {
        _executionQueue.Dequeue().Invoke();
      }
    }
  }
}
