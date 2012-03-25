using Paralect.Domain;
using YouMap.Framework;

namespace YouMap.Domain.Commands
{
    public class Category_CreateCommand: Command
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public bool IsTop { get; set; }

        public int Order { get; set; }
    }

    public class Category_CreateCommandHandler: CommandHandler<Category_CreateCommand>
    {
        public Category_CreateCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override void Handle(Category_CreateCommand message)
        {
            var ar = new CategoryAR(message);
            Repository.Save(ar);
        }
    }
 }