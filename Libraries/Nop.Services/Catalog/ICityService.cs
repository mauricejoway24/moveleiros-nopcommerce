using Nop.Core.Domain.Catalog;
using System.Collections.Generic;

namespace Nop.Services.Catalog
{
    public partial interface ICityService
    {
        IList<int> SearchCities(string city);
        City GetCityById(int cityId);
        City GetCityByUrl(string url);
        IList<City> GetAllCities();
    }
}
