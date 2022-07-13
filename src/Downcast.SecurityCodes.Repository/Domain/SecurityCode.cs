using Google.Cloud.Firestore;

namespace Downcast.SecurityCodes.Repository.Domain;

[FirestoreData]
internal class SecurityCode : CreateSecurityCode
{
    [FirestoreDocumentId]
    public string Id { get; set; } = null!;

    [FirestoreDocumentCreateTimestamp]
    public DateTime CreatedDate { get; set; }

    [FirestoreProperty]
    public DateTime ConfirmationDate { get; init; }
}