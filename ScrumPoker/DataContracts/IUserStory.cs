using System;
using System.Collections.Generic;

namespace ScrumPoker.DataContracts
{
    public interface IUserStory : ITitleAndIdentifier
    {
        string Description { get; set; }

        string AcceptanceCriteria { get; set; }
        
        Guid? ProjectId { get; set; }
        
        Guid? ThemeId { get; set; }
        
        DateTime Created { get; set; }
        
        int? Points { get; set; }
        
        StoryState State { get; set; }
        
        Guid? AssignedToId { get; set; }
        
        int Order { get; set; }
    }
}
