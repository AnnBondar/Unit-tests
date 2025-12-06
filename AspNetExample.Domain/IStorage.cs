using AspNetExample.Domain.Models;

namespace AspNetExample.Domain;

public interface IStorage
{
    Dictionary<Guid, MyModel> Models { get; set; }
}