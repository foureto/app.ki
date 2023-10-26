using System.Data;
using System.Threading.Channels;
using App.Ki.Clickhouse.Helpers;
using Octonica.ClickHouseClient;

namespace App.Ki.Clickhouse.Internals;

public class ChSet<T> where T : class
{
    private const int FlushSize = 100;
    private readonly Channel<object> _channel;

    private int _counter = 0;
    private ClickHouseColumnWriter _dbWriter;

    public ClickhouseSession Session { get; }

    public ChSet(ClickhouseSession session)
    {
        _channel = Channel.CreateUnbounded<object>();
        Session = session;
        SelfInit();
    }

    public ValueTask Write(T item)
        => _channel.Writer.WriteAsync(item);

    private void SelfInit()
    {
        _dbWriter = Session.GetDbWriter(typeof(T));
        Session.Connection.StateChange += ConnectionOnStateChange;
        Task.Run(async () =>
        {
            await foreach (var item in _channel.Reader.ReadAllAsync())
            {
                var rows = CachedObjectInvertor.GetColumnsWithValuesSingle(item);
                await _dbWriter.WriteTableAsync(rows, 1, CancellationToken.None);
                _counter++;
                if (_counter != FlushSize)
                    continue;

                await InitWriter();
            }
        });
    }

    private async Task InitWriter()
    {
        if (_dbWriter != null && _counter > 0)
            await _dbWriter.EndWriteAsync(CancellationToken.None);

        _counter = 0;
        _dbWriter = Session.GetDbWriter(typeof(T));
    }

    private void ConnectionOnStateChange(object sender, StateChangeEventArgs e)
    {
        // TODO refactor
        if (e.OriginalState == ConnectionState.Open && e.CurrentState == ConnectionState.Closed)
        {
            Session.Connection.Open();
        }
    }
}