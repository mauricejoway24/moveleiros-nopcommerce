using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Seo;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Seo
{
    public class KeywordsMappingService : IKeywordsMappingService
    {
        private readonly IRepository<KeywordsMapping> keywordRepository;
        private readonly ICacheManager cacheManager;
        private const string KEYWORDS_MAPPING_ALL = "KEYWORDS_MAPPING_ALL";

        public KeywordsMappingService(
            IRepository<KeywordsMapping> keywordRepository,
            ICacheManager cacheManager)
        {
            this.keywordRepository = keywordRepository;
            this.cacheManager = cacheManager;
        }

        private List<KeywordsMapping> GetAllKeywords()
        {
            return cacheManager.Get(KEYWORDS_MAPPING_ALL, () =>
            {
                //we use no tracking here for performance optimization
                //anyway records are loaded only for read-only operations

                var query = from ur in keywordRepository.TableNoTracking
                            select ur;

                return query.ToList();
            });
        }

        public KeywordsMapping GetKeywordByNormalized(string keywordNormalized)
        {
            return GetAllKeywords()
                .Where(t => t.KeywordNormalized == keywordNormalized)
                .FirstOrDefault();
        }
    }
}
