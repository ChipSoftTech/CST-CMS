using Lucene.Net.Search;
using Newtonsoft.Json.Linq;

namespace Orchard.Lucene
{
    public interface ILuceneQueryProvider
    {
        Query CreateQuery(IQueryService builder, LuceneQueryContext context, string type, JObject query);
    }
}
