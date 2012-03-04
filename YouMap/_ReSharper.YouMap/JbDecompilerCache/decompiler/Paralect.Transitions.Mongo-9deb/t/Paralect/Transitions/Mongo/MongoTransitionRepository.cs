// Type: Paralect.Transitions.Mongo.MongoTransitionRepository
// Assembly: Paralect.Transitions.Mongo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: D:\install\YouMap\YouMap\libs\Paralect\Paralect.Transitions.Mongo.dll

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Paralect.Transitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Paralect.Transitions.Mongo
{
  public class MongoTransitionRepository : ITransitionRepository
  {
    private const string _concurrencyException = "E1100";
    private readonly IDataTypeRegistry _dataTypeRegistry;
    private readonly MongoTransitionServer _server;
    private readonly MongoTransitionSerializer _serializer;

    public MongoTransitionRepository(IDataTypeRegistry dataTypeRegistry, string connectionString, string collectionName = "transitions")
    {
      this._dataTypeRegistry = dataTypeRegistry;
      this._serializer = new MongoTransitionSerializer(dataTypeRegistry);
      this._server = new MongoTransitionServer(connectionString, collectionName);
      this.EnsureIndexes();
    }

    public void EnsureIndexes()
    {
      ((MongoCollection) this._server.Transitions).EnsureIndex((IMongoIndexKeys) IndexKeys.Ascending(new string[1]
      {
        "_id.StreamId"
      }));
      ((MongoCollection) this._server.Transitions).EnsureIndex((IMongoIndexKeys) IndexKeys.Ascending(new string[1]
      {
        "_id.Version"
      }));
      ((MongoCollection) this._server.Transitions).EnsureIndex((IMongoIndexKeys) IndexKeys.Ascending(new string[1]
      {
        "Timestamp"
      }));
      ((MongoCollection) this._server.Transitions).EnsureIndex((IMongoIndexKeys) IndexKeys.Ascending(new string[2]
      {
        "Timestamp",
        "_id.Version"
      }));
    }

    public void SaveTransition(Transition transition)
    {
      if (transition.Events.Count < 1)
        return;
      BsonDocument bsonDocument = this._serializer.Serialize(transition);
      try
      {
        this._server.Transitions.Insert(bsonDocument, SafeMode.get_True());
      }
      catch (MongoException ex)
      {
        if (((Exception) ex).Message.Contains("E1100"))
          throw new DuplicateTransitionException(transition.Id.StreamId, transition.Id.Version, (Exception) ex);
        throw;
      }
    }

    public List<Transition> GetTransitions(string streamId, int fromVersion, int toVersion)
    {
      List<BsonDocument> list = Enumerable.ToList<BsonDocument>((IEnumerable<BsonDocument>) ((MongoCursor<BsonDocument>) ((MongoCollection) this._server.Transitions).FindAs<BsonDocument>((IMongoQuery) Query.And(new IMongoQuery[2]
      {
        (IMongoQuery) Query.EQ("_id.StreamId", BsonValue.op_Implicit(streamId)),
        (IMongoQuery) Query.GTE("_id.Version", BsonValue.op_Implicit(fromVersion)).LTE(BsonValue.op_Implicit(toVersion))
      }))).SetSortOrder((IMongoSortBy) SortBy.Ascending(new string[1]
      {
        "_id.Version"
      })));
      if (list.Count < 1)
        throw new ArgumentException(string.Format("There is no stream in store with id {0}", (object) streamId));
      else
        return Enumerable.ToList<Transition>(Enumerable.Select<BsonDocument, Transition>((IEnumerable<BsonDocument>) list, new Func<BsonDocument, Transition>(this._serializer.Deserialize)));
    }

    public List<Transition> GetTransitions()
    {
      return Enumerable.ToList<Transition>(Enumerable.Select<BsonDocument, Transition>((IEnumerable<BsonDocument>) Enumerable.ToList<BsonDocument>((IEnumerable<BsonDocument>) ((MongoCursor<BsonDocument>) ((MongoCollection) this._server.Transitions).FindAllAs<BsonDocument>()).SetSortOrder((IMongoSortBy) SortBy.Ascending(new string[2]
      {
        "Timestamp",
        "_id.Version"
      }))), new Func<BsonDocument, Transition>(this._serializer.Deserialize)));
    }

    public void RemoveTransition(string streamId, int version)
    {
      BsonDocument bsonDocument1 = this._serializer.SerializeTransitionId(new TransitionId(streamId, version));
      BsonDocument bsonDocument2 = new BsonDocument();
      bsonDocument2.Add("_id", (BsonValue) bsonDocument1);
      ((MongoCollection) this._server.Transitions).Remove((IMongoQuery) new QueryDocument((IEnumerable<BsonElement>) bsonDocument2));
    }

    public void RemoveStream(string streamId)
    {
      BsonDocument bsonDocument = new BsonDocument();
      bsonDocument.Add("_id.StreamId", BsonValue.op_Implicit(streamId));
      ((MongoCollection) this._server.Transitions).Remove((IMongoQuery) new QueryDocument((IEnumerable<BsonElement>) bsonDocument));
    }
  }
}
