using System.ComponentModel.DataAnnotations;

namespace Downcast.SecurityCodes.Repository.Options;

public class RepositoryOptions
{
    public const string SectionName = "RepositoryOptions";
    
    [Required(AllowEmptyStrings = false, ErrorMessage = "Collection name cannot be null or empty")]
    public string Collection { get; init; } = null!;
    
    [Required(AllowEmptyStrings = false, ErrorMessage = "ProjectId is required")]
    public string ProjectId { get; init; } = null!;
}