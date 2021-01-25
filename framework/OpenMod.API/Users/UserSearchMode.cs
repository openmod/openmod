namespace OpenMod.API.Users
{
    /// <summary>
    /// Defines user search modes.
    /// </summary>
    public enum UserSearchMode
    {
        /// <summary>
        /// Find user by ID.
        /// </summary>
        FindById,

        /// <summary>
        /// Find user by name.
        /// </summary>
        FindByName,

        /// <summary>
        /// Find user by name or ID. ID matches will be prioritized.
        /// </summary>
        FindByNameOrId
    }
}