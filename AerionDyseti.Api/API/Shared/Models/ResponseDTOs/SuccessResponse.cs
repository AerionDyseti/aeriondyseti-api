namespace AerionDyseti.API.Shared.Models.ResponseDTOs
{
    /// <summary>
    /// A response DTO to communicate from the API whether or not a given action was successful.
    /// </summary>
    public class SuccessResponse
    {
        /// <summary>
        /// Whether or not the request action succeeded. Defaults to true, as we will usually want to return an error if it was not.
        /// </summary>
        public bool Success { get; set; } = true;
    }

}

