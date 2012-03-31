using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using YouMap.Domain.Enums;
using YouMap.Framework;

namespace YouMap.Documents.Lucene
{
    public class PlaceLuceneService: LuceneIndexService<PlaceLucene>
    {
        public PlaceLuceneService(Settings settings) : base(settings)
        {
            SetIndexName("Places");
        }

        public virtual IEnumerable<PlaceLucene> Search(string searchText,PlaceStatusEnum? statusEqual = null)
        {
            var queries = new List<Query>();
            if (!String.IsNullOrEmpty(searchText))
            {
                var words = searchText.Split(new [] { " " }, StringSplitOptions.RemoveEmptyEntries);

                queries.Add(JoinQueriesOr(
                    BuildFuzzyQueryOr("Memo", words),
                    BuildFuzzyQueryOr("Title", words),
                    BuildFuzzyQueryOr("Tags", words),
                    BuildPrefixQueryAnd("Title", words),
                    BuildFuzzyQueryOr("Address", words)));
            }
            if (statusEqual.HasValue)
            {
                queries.Add(BuildMatchQuery("Status", ((int)statusEqual.Value).ToString()));
            }
            var query = queries.Count == 0 ? new MatchAllDocsQuery() : JoinQueriesAnd(queries.ToArray());
            return Search(query);
        }

        protected override Document MapToLucene(PlaceLucene item)
        {
            var doc = new Document();
            doc.Add(new Field("_id", item.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Status", ((int)item.Status).ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            if (item.Title != null)
                doc.Add(new Field("Title", item.Title, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            if (item.Memo != null)
                doc.Add(new Field("Memo", item.Memo, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));            
            if (item.Tags != null && item.Tags.Any())
                doc.Add(new Field("Tags", string.Join(",",item.Tags), Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            if (item.Address != null)
                doc.Add(new Field("Address", item.Address, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            
            return doc;
        }

        protected override PlaceLucene MapFromLucene(Document doc)
        {
            return new PlaceLucene
                       {
                           Id = doc.Get("_id"),
                           Title = doc.Get("Title"),
                           Address = doc.Get("Address"),
                           Memo = doc.Get("Memo"),
                           Status = (PlaceStatusEnum)int.Parse(doc.Get("Status")),
                           Tags = doc.Get("Tags").Split(new []{','},StringSplitOptions.RemoveEmptyEntries)
                       };
        }
    }

    public class PlaceLucene
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Memo { get; set; }

        public string Address { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public PlaceStatusEnum Status { get; set; }
    }
}