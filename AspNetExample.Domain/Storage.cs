using AspNetExample.Domain.Models;

namespace AspNetExample.Domain;

internal class Storage : IStorage
{
    public Dictionary<Guid, MyModel> Models { get; set; }
}