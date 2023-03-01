namespace ScrumPoker.DataContracts
{
    /// <summary>
    /// General scrum poker card type.
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// Card represents a points value.
        /// </summary>
        Points,
        
        /// <summary>
        /// Card represents a vote that indicates there is not enough information to estimate a points value.
        /// </summary>
        Ambiguous,
        
        /// <summary>
        /// Card represents a vote indicating that a user story cannot be completed.
        /// </summary>
        Unattainable,
        
        /// <summary>
        /// Card represents a vote indicating the participant has recused themselves from the vote.
        /// </summary>
        Abstain
    }
}