namespace GridCityServer.Models;

public class Entity<T>
    where T : struct
{
    public T Id { get; set; }
}
