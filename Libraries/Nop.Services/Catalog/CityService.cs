using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Catalog
{
    public class CityService : ICityService
    {
        private readonly IRepository<City> cityRepository;
        private readonly ICacheManager cacheManager;

        private const string KEY_CITY_ALL = "KEY_CITY_ALL_{0}";
        private const string KEY_CITY_BY_URL = "KEY_CITY_BY_URL_{0}";
        private const string KEY_CITY_COMPLETE = "KEY_CITY_COMPLETE";

        public CityService(
            IRepository<City> cityRepository,
            ICacheManager cacheManager)
        {
            this.cityRepository = cityRepository;
            this.cacheManager = cacheManager;
        }

        public City GetCityById(int cityId)
        {
            return cityRepository.GetById(cityId);
        }

        public IList<int> SearchCities(string city)
        {
            var key = string.Format(KEY_CITY_ALL, city);

            return cacheManager.Get(key, () => {
                return (from p in cityRepository.Table
                 where p.Description.Contains(city)
                 select p.Id).ToList();
            });
        }

        public IList<City> GetAllCities()
        {
            return cacheManager.Get(KEY_CITY_COMPLETE, () => {
                return (from p in cityRepository.Table
                        orderby p.Description
                        select p).ToList();
            });
        }

        public City GetCityByUrl(string url)
        {
            var key = string.Format(KEY_CITY_BY_URL, url);

            return cacheManager.Get(key, () => {
                return (from p in cityRepository.Table
                        where p.ShortUrl == url
                        select p).FirstOrDefault();
            });
        }
    }
}
