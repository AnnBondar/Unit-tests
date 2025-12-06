using AspNetExample.Service.Models;

namespace AspNetExample.Service.Services;

public interface IService
{
    MyModel CreateModel(string data);

    MyModel GetModel(Guid id);

    MyModel UpdateModel(MyModel data);

    bool DeleteModel(Guid id);
}