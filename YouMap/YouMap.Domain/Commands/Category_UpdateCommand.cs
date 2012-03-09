using Paralect.Domain;
using mPower.Framework;

namespace YouMap.Domain.Commands
{
    public class Category_UpdateCommand : Command
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public bool IsTop { get; set; }
    }

    public class Category_UpdateCommandHandler : CommandHandler<Category_UpdateCommand>
    {
        public Category_UpdateCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override void Handle(Category_UpdateCommand message)
        {
            var ar = Repository.GetById<CategoryAR>(message.Id);
            ar.SetCommandMetadata(message.Metadata);
            ar.Update(message);
            Repository.Save(ar);
        }
    }
}