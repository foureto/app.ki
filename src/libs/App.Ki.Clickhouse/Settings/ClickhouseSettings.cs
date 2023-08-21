namespace App.Ki.Clickhouse.Settings;

public class ClickhouseSettings
{
    public bool UseMigrations { get; set; }
    public bool LogQueries { get; set; } = true;
    public ushort Port { get; set; }
    public string Host { get; set; }
    public string Database { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public int CommandTimeout { get; set; }
    public string Cluster { get; set; }
    public BufferSettings Buffer { get; set; } = new();
}

public class BufferSettings
{
    public int FlushInSeconds { get; set; } = 2;
    public int MaxGetFromBuffer { get; set; } = 2000;
    public int MinGetFromBuffer { get; set; } = 1;
}