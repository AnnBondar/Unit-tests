using AspNetExample.Domain.Models;
using System;

namespace AspNetExample.Domain.Repositories;

public interface IRepository
{
    MyModel CreateModel(string data);

    MyModel GetModel(Guid id);

    MyModel UpdateModel(MyModel data);

    bool DeleteModel(Guid id);
}