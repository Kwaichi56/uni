namespace ServiceLocatorLib;

public interface IServiceStorage
{
    object Get(ServiceScope scope);
}
