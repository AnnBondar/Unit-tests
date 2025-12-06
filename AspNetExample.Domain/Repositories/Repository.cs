using AspNetExample.Domain.Models;
using AspNetExample.Service;

namespace AspNetExample.Domain.Repositories;

// Do db related things here
internal class Repository : IRepository
{
    private readonly IGuidProvider _guidProvider;
    private readonly IStorage _storage;

    public Repository(IGuidProvider guidProvider, IStorage storage)
    {
        _guidProvider = guidProvider;
        _storage = storage;
    }

    public MyModel CreateModel(string data)
    {
        if (data is null)
        {
            return null;
        }

        MyModel model = new(data, _guidProvider.GetGuid());
        _storage.Models.Add(model.Id, model);

        return model;
    }

    public MyModel GetModel(Guid id) => _storage.Models.GetValueOrDefault(id);

    public MyModel UpdateModel(MyModel data)
    {
        _storage.Models[data.Id] = data;

        return data;
    }

    public bool DeleteModel(Guid id) => _storage.Models.Remove(id);
}