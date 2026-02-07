public class PlayerName : Singleton<PlayerName>
{
    private string _playerName = null;
    public static string Name
    {
        get => Instance._playerName;
        set => Instance._playerName = value;
    }

    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}
