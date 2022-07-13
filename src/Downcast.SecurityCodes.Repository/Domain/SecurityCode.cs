using Google.Cloud.Firestore;

namespace Downcast.SecurityCodes.Repository.Domain;

[FirestoreData]
internal class SecurityCode : CreateSecurityCode
{
    [FirestoreDocumentUpdateTimestamp]
    public DateTime UpdatedDate { get; init; }

    [FirestoreProperty]
    public DateTime ConfirmationDate { get; init; }
}