using Downcast.Common.Errors;
using Downcast.SecurityCodes.Model;
using Downcast.SecurityCodes.Repository.Domain;
using Downcast.SecurityCodes.Repository.Options;

using Firestore.Typed.Client;
using Firestore.Typed.Client.Extensions;

using Google.Cloud.Firestore;

using MapsterMapper;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SecurityCode = Downcast.SecurityCodes.Repository.Domain.SecurityCode;

namespace Downcast.SecurityCodes.Repository;

public class SecurityCodesFirestoreRepository : ISecurityCodesRepository
{
    private readonly TypedCollectionReference<SecurityCode> _collection;
    private readonly FirestoreDb _firestoreDb;
    private readonly ILogger<SecurityCodesFirestoreRepository> _logger;
    private readonly IMapper _mapper;

    public SecurityCodesFirestoreRepository(
        FirestoreDb firestoreDb,
        IOptions<RepositoryOptions> options,
        ILogger<SecurityCodesFirestoreRepository> logger,
        IMapper mapper)
    {
        _firestoreDb = firestoreDb;
        _logger      = logger;
        _mapper      = mapper;
        _collection  = firestoreDb.TypedCollection<SecurityCode>(options.Value.Collection);
    }


    public async Task<Model.SecurityCode> Create(string code, string target)
    {
        SecurityCode securityCodeToCreate = CreateSecurityCode(code, target);
        await _collection.Document(target).SetAsync(securityCodeToCreate, SetOptions.Overwrite).ConfigureAwait(false);
        _logger.LogInformation("Created security for target {Target}", target);
        return await Get(target).ConfigureAwait(false);
    }

    private static SecurityCode CreateSecurityCode(string code, string target)
    {
        return new SecurityCode
        {
            Code             = code,
            Target           = target,
            ConfirmationDate = null,
            Created          = DateTime.UtcNow
        };
    }

    public async Task<Model.SecurityCode> Get(string target)
    {
        TypedDocumentSnapshot<SecurityCode> document = await GetSnapshotByTarget(target).ConfigureAwait(false);
        return _mapper.Map<Model.SecurityCode>(document.RequiredObject);
    }

    public async Task UpdateConfirmationDate(string target, DateTime date)
    {
        TypedDocumentSnapshot<SecurityCode> document = await GetSnapshotByTarget(target).ConfigureAwait(false);

        UpdateDefinition<SecurityCode> setOptions = new UpdateDefinition<SecurityCode>()
            .Set(code => code.ConfirmationDate, date);
        WriteResult _ = await document.Reference.UpdateAsync(setOptions).ConfigureAwait(false);
    }

    public Task Delete(string target)
    {
        return _collection.Document(target).DeleteAsync();
    }

    private async Task<TypedDocumentSnapshot<SecurityCode>> GetSnapshotByTarget(string target)
    {
        TypedDocumentSnapshot<SecurityCode> snapshot = await _collection
            .Document(target)
            .GetSnapshotAsync()
            .ConfigureAwait(false);

        if (!snapshot.Exists)
        {
            throw new DcException(ErrorCodes.EntityNotFound, $"Could not find security code for target {target}");
        }

        return snapshot;
    }
}