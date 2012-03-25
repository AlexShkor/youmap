using YouMap.Domain.Commands;
using YouMap.Domain.Events;
using YouMap.Framework;

namespace YouMap.Domain
{
    public class CategoryAR: YoumapAR
    {
        public CategoryAR()
        {
            
        }

        public CategoryAR(Category_CreateCommand message)
        {
            _id = message.Id;
            SetCommandMetadata(message.Metadata);
            Apply(new Category_CreatedEvent
                      {
                          Id = message.Id,
                          Icon = message.Icon,
                          Name = message.Name,
                          Order = message.Order,
                          IsTop = message.IsTop
                      });
        }

        public void Update(Category_UpdateCommand message)
        {
           
            Apply(new Category_UpdatedEvent
                      {
                          Id = message.Id,
                          Name = message.Name,
                          Icon = message.Icon,
                          Order = message.Order,
                          IsTop = message.IsTop
                      });
        }

        public void Delete()
        {
            Apply(new Category_DeletedEvent
            {
                Id = _id
            });

        }

        #region Object Reconstruction

        protected void On(Category_CreatedEvent created)
        {
            _id = created.Id;
        }

        #endregion
    }
}