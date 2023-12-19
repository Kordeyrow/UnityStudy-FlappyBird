using System.Collections.Generic;

public class ServiceContainer<T> where T : class
{
    T? currentService;
    public T? GetService() => currentService;
    public static ServiceContainer<T> Instance { get; } = new ServiceContainer<T>();
    readonly HashSet<IServiceContainerConsumer<T>> consumers = new();

    public void SetService(T newService)
    {
        if (currentService != newService)
            UpdateService(newService);
    }

    public void RemoveService(T serviceToRemove)
    {
        if (currentService == serviceToRemove)
            UpdateService(null);
    }

    void UpdateService(T newService)
    {
        var oldService = currentService;
        currentService = newService;

        // Notify Consumers
        foreach (var consumer in consumers)
            consumer.OnServiceUpdated(oldService, newService);
    }

    public void AddConsumer(IServiceContainerConsumer<T> consumer)
    {
        consumers.Add(consumer);
    }

    public void RemoveConsumer(IServiceContainerConsumer<T> consumer)
    {
        consumers.Remove(consumer);
    }
}
