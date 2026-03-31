namespace projectBackend.Models.Redis;

public class RedisSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string InstanceName { get; set; } = "MyAppCache";
    public int ConnectTimeout { get; set; } = 5000;
    public int SyncTimeout { get; set; } = 5000;
    public int ConnectRetry { get; set; } = 3;
    public bool AbortOnConnectFail { get; set; } = false;
    public int Database { get; set; } = 0;

    public string? ServiceName { get; set; }
    public List<string>? SentinelEndpoints { get; set; }

    public bool Ssl { get; set; } = false;
    public string? SslHost { get; set; }

    public int PoolSize { get; set; } = 50;
}
