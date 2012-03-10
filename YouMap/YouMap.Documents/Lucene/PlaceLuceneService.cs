using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using YouMap.Framework;

namespace YouMap.Documents.Lucene
{
    public class PlaceLuceneService: LuceneIndexService<PlaceLucene>
    {
        public PlaceLuceneService(Settings settings) : base(settings)
        {
            SetIndexName("Places");
        }

        public virtual IEnumerable<PlaceLucene> Search(string searchText)
        {
            var queries = new List<Query>();
            if (!String.IsNullOrEmpty(searchText))
            {
                var words = searchText.Split(new [] { " " }, StringSplitOptions.RemoveEmptyEntries);

                queries.Add(JoinQueriesOr(
                    BuildFuzzyQueryOr("Memo", words),
                    BuildFuzzyQueryOr("Title", words),
                    BuildFuzzyQueryOr("Address", words)));
            }
            var query = queries.Count == 0 ? new MatchAllDocsQuery() : JoinQueriesAnd(queries.ToArray());
            return Search(query);
        }

        protected override Document MapToLucene(PlaceLucene item)
        {
            var doc = new Document();
            doc.Add(new Field("_id", item.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            if (item.Title != null)
                doc.Add(new Field("Title", item.Title, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            if (item.Memo != null)
                doc.Add(new Field("Memo", item.Memo, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
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
                           Memo = doc.Get("Memo")
                       };
        }
    }

    public class PlaceLucene
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Memo { get; set; }

        public string Address { get; set; }
    }
}