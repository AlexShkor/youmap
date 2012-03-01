using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Paralect.Domain;
using mPower.Framework.Environment;

namespace mPower.Framework
{
    public class YoumapAR : AggregateRoot
    {
        private readonly MongoObjectIdGenerator _idGenerator;

        public YoumapAR()
        {
            _idGenerator = new MongoObjectIdGenerator();
        }

        private ICommandMetadata _commandMetadata;

        public void SetCommandMetadata(ICommandMetadata commandMetadata)
        {
            _commandMetadata = commandMetadata;
        }

        public new void Apply(IEvent evt)
        {
            if(_commandMetadata == null)
                throw new ArgumentException("You should send command metadata to Aggregate Root before Apply event");

            evt.Metadata.UserId = _commandMetadata.UserId;
            evt.Metadata.CommandId = _commandMetadata.CommandId;
            evt.Metadata.EventId = _idGenerator.Generate();

            base.Apply(evt);
        }
    }
}
