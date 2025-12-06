using AspNetExample.Domain.Repositories;
using AspNetExample.Service.Models;
using MyDomainModel = AspNetExample.Domain.Models.MyModel;

namespace AspNetExample.Service.Services;

internal class Service : IService
{
    private readonly IRepository _repository;
    private readonly IModelDescriptionProvider _modelDescriptionProvider;

    public Service(IRepository repository, IModelDescriptionProvider modelDescriptionProvider)
    {
        _repository = repository;
        _modelDescriptionProvider = modelDescriptionProvider;
    }

    public MyModel CreateModel(string data) => Convert(_repository.CreateModel(data));

    public MyModel? GetModel(Guid id)
    {
        var domainModel = _repository.GetModel(id);
        if (domainModel is null)
        {
            return null;
        }
        
        var model = Convert(domainModel);
        string description = _modelDescriptionProvider.GetDescription(id);

        return model with { Description = description };
    }

    public MyModel UpdateModel(MyModel data) => Convert(_repository.UpdateModel(Convert(data)));

    public bool DeleteModel(Guid id) => _repository.DeleteModel(id);

    private MyModel Convert(MyDomainModel model) => new(model.Data, model.Id);

    private MyDomainModel Convert(MyModel model) => new(model.Data, model.Id);
}