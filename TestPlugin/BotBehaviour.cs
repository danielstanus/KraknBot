using TestPlugin;
using UnityEngine;

public class BotBehaviour : MonoBehaviour
{
    private readonly Collector _collector = new Collector();

    public void StartCollector()
    {
        _collector.Start();
        InvokeRepeating(nameof(LazyUpdate), 0f, 1f);
    }

    public void StopCollector()
    {
        _collector.Stop();
        CancelInvoke(nameof(LazyUpdate));
    }

    private void LazyUpdate()
    {
        _collector.Update();
    }

    private void Update()
    {
        // Implement update logic here
    }
}