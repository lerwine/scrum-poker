namespace ScrumPoker.DataContracts
{
    /// <summary>
    /// Represents the state of a user story.
    /// </summary>
    public enum StoryState
    {
        /// <summary>
        /// User story is in the backlog.
        /// </summary>
        Draft = 0,
        
        /// <summary>
        /// User story has been added to a sprint.
        /// </summary>
        Ready = 1,
        
        /// <summary>
        /// User story has been canceled.
        /// </summary>
        Cancelled = 2
    }
}
