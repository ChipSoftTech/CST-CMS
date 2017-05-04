using System;
using Lucene.Net.Search;
using Newtonsoft.Json.Linq;

namespace Orchard.Lucene.QueryProviders
{
    public class BooleanQueryProvider : ILuceneQueryProvider
    {
        public Query CreateQuery(IQueryDslBuilder builder, LuceneQueryContext context, string type, JObject query)
        {
            if (type != "bool")
            {
                return null;
            }

            var boolQuery = new BooleanQuery();

            foreach (var property in query.Properties())
            {
                var occur = BooleanClause.Occur.MUST;

                switch (property.Name.ToLowerInvariant())
                {
                    case "must":
                        occur = BooleanClause.Occur.MUST;
                        break;
                    case "mustnot":
                    case "must_not":
                        occur = BooleanClause.Occur.MUST_NOT;
                        break;
                    case "should":
                        occur = BooleanClause.Occur.SHOULD;
                        break;
                    case "boost":
                        boolQuery.Boost = query.Value<float>();
                        break;
                    case "minimum_should_match":
                        boolQuery.MinimumNumberShouldMatch = query.Value<int>();
                        break;
                    default: throw new ArgumentException($"Invalid property '{property.Name}' in boolean query");
                }

                switch (property.Value.Type)
                {
                    case JTokenType.Object:

                        break;
                    case JTokenType.Array:
                        foreach (var item in ((JArray)property.Value))
                        {
                            if (item.Type != JTokenType.Object)
                            {
                                throw new ArgumentException($"Invalid value in boolean query");
                            }
                            boolQuery.Add(builder.CreateQueryFragment(context, (JObject)item), occur);
                        }
                        break;
                    default: throw new ArgumentException($"Invalid value in boolean query");

                }
            }

            return boolQuery;
        }
    }
}