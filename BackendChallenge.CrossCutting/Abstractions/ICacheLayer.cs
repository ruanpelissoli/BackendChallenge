namespace BackendChallenge.CrossCutting.Abstractions;
public interface ICacheLayer
{
    Task Set<T>(string key, T item, CancellationToken cancellationToken = default);
    Task<T?> Get<T>(string key, CancellationToken cancellationToken = default);
    Task Remove(string key, CancellationToken cancellationToken = default);
}

