using System.Collections.Generic;

namespace AerionDyseti.API.Shared.Models.ResponseDTOs
{
    /// <summary>
    /// A response DTO to communicate when one or more errors has occurred.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// The list of errors to return to the API, with descriptive text on what exactly has happened.
        /// </summary>
        public List<string> Errors { get; set; }
    }
}
