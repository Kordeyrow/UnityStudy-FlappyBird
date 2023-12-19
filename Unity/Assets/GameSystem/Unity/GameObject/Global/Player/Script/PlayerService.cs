using System;
using System.Collections.Generic;

public class PlayerService : Service<IPlayer> { }

public class Service<T> where T : class
{
    T? service;
    public T? GetService() => service;
    public static Service<T> Instance { get; } = new Service<T>();
    readonly HashSet<ConsumerConnection<T>> consumersConnections = new();

    public void UpdateService(T newService)
    {
        if (service == newService)
            return;

        foreach (var consumersConnection in consumersConnections)
            consumersConnection.Disconnect(service);

        service = newService;

        foreach (var consumersConnection in consumersConnections)
            consumersConnection.Connect(service);
    }

    public void AddConsumer(ConsumerConnection<T> connection)
    {
        if (consumersConnections.Contains(connection))
            return;

        connection.Connect(service);
        consumersConnections.Add(connection);
    }

    public void RemoveConsumer(ConsumerConnection<T> connection)
    {
        if (consumersConnections.Contains(connection) == false)
            return;

        connection.Disconnect(service);
        consumersConnections.Remove(connection);
    }
}

public readonly struct ConsumerConnection<T> where T : class
{
    public readonly Action<T?> Connect { get; }
    public readonly Action<T?> Disconnect { get; }

    public ConsumerConnection(Action<T?> connect, Action<T?> disconnect)
    {
        Connect = connect;
        Disconnect = disconnect;
    }
}

