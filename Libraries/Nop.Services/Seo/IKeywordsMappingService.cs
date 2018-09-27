using Nop.Core.Domain.Seo;

namespace Nop.Services.Seo
{
    public interface IKeywordsMappingService
    {
        KeywordsMapping GetKeywordByNormalized(string keywordNormalized);
    }
}
