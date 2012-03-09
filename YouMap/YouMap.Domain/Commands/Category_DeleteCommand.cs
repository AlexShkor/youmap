﻿using Paralect.Domain;
using mPower.Framework;

namespace YouMap.Domain.Commands
{
    public class Category_DeleteCommand : Command
    {
        public string Id { get; set; }
    }

    public class Category_DeleteCommandHandler: CommandHandler<Category_DeleteCommand>
    {
        public Category_DeleteCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override void Handle(Category_DeleteCommand message)
        {
            var ar = Repository.GetById<CategoryAR>(message.Id);
            ar.SetCommandMetadata(message.Metadata);
            ar.Delete();
            Repository.Save(ar);
        }
    }
}