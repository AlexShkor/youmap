// Type: Paralect.Domain.CommandMetadata
// Assembly: Paralect.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: D:\install\YouMap\YouMapCqrsMongo\YouMap\libs\Paralect\Paralect.Domain.dll

using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Paralect.Domain
{
  [BsonDiscriminator("CommandMetadata")]
  public class CommandMetadata : ICommandMetadata
  {
    public string CommandId { get; set; }

    public string UserId { get; set; }

    public string TypeName { get; set; }

    public DateTime CreatedDate { get; set; }
  }
}
