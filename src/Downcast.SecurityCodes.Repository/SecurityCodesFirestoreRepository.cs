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
        DocumentReference document = await _collection.AddAsync(new CreateSecurityCode
        {
            Code   = code,
            Target = target
        }).ConfigureAwait(false);
        _logger.LogDebug("Created security code with {SecurityCodeId}", document.Id);
        return document.Id;
    }

    public async Task<Model.SecurityCode> GetByTarget(string target)
    {
        DocumentSnapshot document = await GetSnapshotByTarget(target).ConfigureAwait(false);
        return _mapper.Map<Model.SecurityCode>(document.ConvertTo<SecurityCode>());
    }

    public async Task UpdateConfirmationDate(string target, DateTime date)
    {
        DocumentSnapshot document = await GetSnapshotByTarget(target).ConfigureAwait(false);
        WriteResult _ = await document.Reference.SetAsync(new Dictionary<string, object>
        {
            {nameof(SecurityCode.ConfirmationDate), date}
        }, SetOptions.MergeAll).ConfigureAwait(false);
    }

    private async Task<DocumentSnapshot> GetSnapshotByTarget(string target)
    {
        QuerySnapshot snapshot = await _collection
            .WhereEqualTo(nameof(SecurityCode.Target), target)
            .Limit(1)
            .GetSnapshotAsync()
            .ConfigureAwait(false);

        if (snapshot is not {Count: > 0} || snapshot.Documents[0] is not {Exists: true} document)
        {
            throw new DcException(ErrorCodes.EntityNotFound, $"Could not find security code for target {target}");
        }

        return document;
    }
}