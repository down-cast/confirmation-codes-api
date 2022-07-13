using Google.Cloud.Firestore;

namespace Downcast.SecurityCodes.Repository.Domain;

[FirestoreData]
internal class CreateSecurityCode
{
    [FirestoreProperty]
    public string Target { get; init; } = null!;

    [FirestoreProperty]
    public string Code { get; init; } = null!;
}