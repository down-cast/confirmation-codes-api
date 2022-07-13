using System.ComponentModel.DataAnnotations;

namespace Downcast.SecurityCodes.Manager;

public class SecurityCodesOptions
{
    public const string SectionName = "SecurityCodesOptions";

    [Range(4, 100)]
    public int CodeLength { get; set; }
}