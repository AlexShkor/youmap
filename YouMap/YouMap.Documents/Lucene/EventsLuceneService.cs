using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using YouMap.Framework;

namespace YouMap.Documents.Lucene
{
    public class EventsLuceneService: LuceneIndexService<EventLucene>
    {
        private DateTime _startDate = new DateTime(2010,1,1);

        public EventsLuceneService(Settings settings) : base(settings)
        {
        }

        public virtual IEnumerable<EventLucene> Search(EventLuceneFilter filter)
        {
            var queries = new List<Query>();
            if (!String.IsNullOrEmpty(filter.SearchText))
            {
                var words = filter.SearchText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                queries.Add(JoinQueriesOr(
                    BuildFuzzyQueryOr("Memo", words),
                    BuildFuzzyQueryOr("Title", words),
                    BuildPrefixQueryAnd("Title", words), 
                    BuildFuzzyQueryOr("PlaceTitle", words),
                    BuildPrefixQueryAnd("PlaceTitle", words),
                    BuildPhraseQueryOr("MembersNames", words)));
            }
            if (filter.MembersIds != null && filter.MembersIds.Any())
            {
                queries.Add(BuildPhraseQueryOr("MembersIds", filter.MembersIds.ToArray()));
            }
            var query = queries.Count == 0 ? new MatchAllDocsQuery() : JoinQueriesAnd(queries.ToArray());
            return Search(query);
        }

        protected override Document MapToLucene(EventLucene item)
        {
            var doc = new Document();
            doc.Add(new Field("_id", item.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("PlaceId", item.PlaceId, Field.Store.YES, Field.Index.NOT_ANALYZED));
            if (item.Title != null)
                doc.Add(new Field("Title", item.Title, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS)); 
            if (item.PlaceTitle != null)
                doc.Add(new Field("PlaceTitle", item.PlaceTitle, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            if (item.StartDate.HasValue)
                doc.Add(new Field("StartDate", DateTools.DateToString(item.StartDate.Value, DateTools.Resolution.MINUTE),
                                  Field.Store.YES, Field.Index.NOT_ANALYZED));
            if (item.Memo != null)
                doc.Add(new Field("Memo", item.Memo, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            if (item.MembersIds != null && item.MembersIds.Any())
            {
                doc.Add(new Field("MembersIds", string.Join(" ", item.MembersIds), Field.Store.YES,
                                  Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            
            }
            if (item.MembersNames != null && item.MembersNames.Any())
            {
                doc.Add(new Field("MembersNames", string.Join(" ", item.MembersNames),
                                  Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            }
            return doc;
        }

        protected override EventLucene MapFromLucene(Document doc)
        {
            return new EventLucene
            {
                Id = doc.Get("_id"),
                Title = doc.Get("Title"),
                Memo = doc.Get("Memo"),
                StartDate = DateTools.StringToDate(doc.Get("StartDate")),
                PlaceTitle = doc.Get("PlaceTitle"),
                PlaceId = doc.Get("PlaceId"),
            };
        }
    }

    public class EventLuceneFilter
    {
        public string SearchText;

        public List<string> MembersIds { get; set; }
    }

    public class EventLucene
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Memo { get; set; }

        public List<string> MembersIds { get; set; }

        public List<string> MembersNames { get; set; }

        public string PlaceTitle { get; set; }

        public DateTime? StartDate { get; set; }

        public string PlaceId { get; set; }
    }
}