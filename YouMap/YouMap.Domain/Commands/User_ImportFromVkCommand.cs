﻿using Paralect.Domain;
using YouMap.Domain.Data;
using YouMap.Framework;

namespace YouMap.Domain.Commands
{
    public class User_ImportFromVkCommand: Command
    {
        public string UserId { get; set; }

        public VkData Vk { get; set; }
    }

    public class User_ImportFromVkCommandHandler: CommandHandler<User_ImportFromVkCommand>
    {
        public override void Handle(User_ImportFromVkCommand message)
        {
            var ar = Repository.GetById<UserAR>(message.UserId);
            ar.SetCommandMetadata(message.Metadata);
            ar.ImportFromVk(message.Vk);
            Repository.Save(ar);
        }
    }
}