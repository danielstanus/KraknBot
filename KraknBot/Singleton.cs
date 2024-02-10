namespace KraknBot;

public class Singleton<T> where T : new()
{
    private static T _instance;
    private static readonly object Lock = new object();

    public static T Instance
    {
        get
        {
            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }
}
