public interface IServiceContainerConsumer<T>
{
    void OnServiceUpdated(T oldService, T newService);
}
