using Google.Cloud.Firestore;

namespace Downcast.SecurityCodes.Repository.Domain;

[FirestoreData]
internal class SecurityCode
{
    [FirestoreDocumentId]
    public string Target { get; init; } = null!;

    [FirestoreProperty]
    public string Code { get; init; } = null!;

    [FirestoreDocumentUpdateTimestamp]
    public DateTime Updated { get; init; }

    [FirestoreProperty]
    public DateTime Created { get; init; }

    [FirestoreProperty]
    public DateTime? ConfirmationDate { get; init; }
}