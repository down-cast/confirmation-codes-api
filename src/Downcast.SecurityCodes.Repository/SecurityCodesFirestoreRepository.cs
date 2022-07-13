using Downcast.Common.Errors;
using Downcast.SecurityCodes.Model;
using Downcast.SecurityCodes.Repository.Domain;
using Downcast.SecurityCodes.Repository.Options;

using Google.Cloud.Firestore;

using MapsterMapper;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SecurityCode = Downcast.SecurityCodes.Repository.Domain.SecurityCode;

namespace Downcast.SecurityCodes.Repository;

public class SecurityCodesFirestoreRepository : ISecurityCodesRepository
{
    private readonly CollectionReference _collection;
    private readonly ILogger<SecurityCodesFirestoreRepository> _logger;
    private readonly IMapper _mapper;

    public SecurityCodesFirestoreRepository(
        FirestoreDb firestoreDb,
        IOptions<RepositoryOptions> options,
        ILogger<SecurityCodesFirestoreRepository> logger,
        IMapper mapper)
    {
        _logger     = logger;
        _mapper     = mapper;
        _collection = firestoreDb.Collection(options.Value.Collection);
    }

    public async Task<string> Create(string code, string target)
    {
        var codeData = new CreateSecurityCode
        {
            Code   = code,
            Target = target
        };
        QuerySnapshot snapshot = await GetSnapshotByTarget(target).ConfigureAwait(false);
        if (snapshot is { Count: > 0 } && snapshot.Documents[0] is { Exists: true } document)
        {
            await document.Reference.SetAsync(codeData, SetOptions.Overwrite).ConfigureAwait(false);
            _logger.LogDebug("Updated security code with {SecurityCodeId}", document.Reference.Id);
            return document.Reference.Id;
        }

        DocumentReference documentRef = await _collection.AddAsync(codeData).ConfigureAwait(false);

        _logger.LogDebug("Created security code with {SecurityCodeId}", documentRef.Id);
        return documentRef.Id;
    }

    public async Task<Model.SecurityCode> GetByTarget(string target)
    {
        DocumentSnapshot document = await GetDocumentByTarget(target).ConfigureAwait(false);
        return _mapper.Map<Model.SecurityCode>(document.ConvertTo<SecurityCode>());
    }

    public async Task UpdateConfirmationDate(string target, DateTime date)
    {
        DocumentSnapshot document = await GetDocumentByTarget(target).ConfigureAwait(false);
        WriteResult _ = await document.Reference.SetAsync(new Dictionary<string, object>
        {
            { nameof(SecurityCode.ConfirmationDate), date }
        }, SetOptions.MergeAll).ConfigureAwait(false);
    }

    private async Task<DocumentSnapshot> GetDocumentByTarget(string target)
    {
        QuerySnapshot snapshot = await GetSnapshotByTarget(target).ConfigureAwait(false);
        if (snapshot is not { Count: > 0 } || snapshot.Documents[0] is not { Exists: true } document)
        {
            throw new DcException(ErrorCodes.EntityNotFound, $"Could not find security code for target {target}");
        }

        return document;
    }

    private Task<QuerySnapshot> GetSnapshotByTarget(string target)
    {
        return _collection
            .WhereEqualTo(nameof(SecurityCode.Target), target)
            .Limit(1)
            .GetSnapshotAsync();
    }
}