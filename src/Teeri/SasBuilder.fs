[<AutoOpen>]
module Teeri.SasBuilder

open System
open Azure.Storage.Sas

type SasBuilder (level: SasTarget, expiresOn: DateTimeOffset) =

    member _.Yield _ =
        let builder = BlobSasBuilder()
        builder.ExpiresOn <- expiresOn
        match level with
        | Account permissions ->
            builder.SetPermissions(permissions)
        | Container (name, permissions) ->
            builder.Resource <- "c"
            builder.BlobContainerName <- name
            builder.SetPermissions(permissions)
        | Blob (name, permissions) ->
            builder.Resource <- "b"
            builder.BlobName <- name
            builder.SetPermissions(permissions)
        | Snapshot (name, permissions) ->
            builder.Resource <- "bs"
            builder.Snapshot <- name
            builder.SetPermissions(permissions)
        | BlobVersion (versionId, permissions) ->
            builder.Resource <- "bv"
            builder.BlobVersionId <- versionId
            builder.SetPermissions(permissions)
        { Builder = builder }

    member this.Zero() =
        this.Yield()

    /// Override the value returned for Cache-Control response header.
    [<CustomOperation"cacheControl">]
    member _.CacheControl(state: Sas, cacheControl) =
        state.Builder.CacheControl <- cacheControl
        state

    /// Override the value returned for Content-Disposition response header.
    [<CustomOperation"contentDisposition">]
    member _.ContentDisposition(state: Sas, disposition) =
        state.Builder.ContentDisposition <- disposition
        state

    /// Override the value returned for Cache-Encoding response header.
    [<CustomOperation"contentEncoding">]
    member _.ContentEncoding(state: Sas, encoding) =
        state.Builder.ContentEncoding <- encoding
        state

    /// Override the value returned for Cache-Language response header.
    [<CustomOperation"contentLanguage">]
    member _.ContentLanguage(state: Sas, language) =
        state.Builder.ContentLanguage <- language
        state

    /// Override the value returned for Cache-Type response header.
    [<CustomOperation"contentType">]
    member _.ContentType(state: Sas, contentType) =
        state.Builder.ContentType <- contentType
        state

    /// Optional. Beginning in version 2020-02-10, this value will be used for to correlate the storage audit logs with the audit logs used by the principal generating and distributing SAS. This is only used for User Delegation SAS.
    [<CustomOperation"correlationId">]
    member _.CorrelationId(state: Sas, correlationId) =
        state.Builder.CorrelationId <- correlationId
        state

    /// An optional unique value up to 64 characters in length that correlates to an access policy specified for the container.
    [<CustomOperation"identifier">]
    member _.Identifier(state: Sas, identifier) =
        state.Builder.Identifier <- identifier
        state

    /// Specifies an IP address or a range of IP addresses from which to accept requests. If the IP address from which the request originates does not match the IP address or address range specified on the SAS token, the request is not authenticated. When specifying a range of IP addresses, note that the range is inclusive.
    [<CustomOperation"ipRange">]
    member _.IPRange(state: Sas, ipRange) =
        state.Builder.IPRange <- ipRange
        state

    /// Optional. Beginning in version 2020-02-10, this value will be used for the AAD Object ID of a user authorized by the owner of the User Delegation Key to perform the action granted by the SAS. The Azure Storage service will ensure that the owner of the user delegation key has the required permissions before granting access. No additional permission check for the user specified in this value will be performed. This is only used with generating User Delegation SAS.
    [<CustomOperation"preauthorizedAgentObjectId">]
    member _.PreauthorizedAgentObjectId(state: Sas, objectId) =
        state.Builder.PreauthorizedAgentObjectId <- objectId
        state

    /// The optional signed protocol field specifies the protocol permitted for a request made with the SAS. Possible values are Azure.Storage.Sas.SasProtocol.HttpsAndHttp, Azure.Storage.Sas.SasProtocol.Https, and Azure.Storage.Sas.SasProtocol.None.
    [<CustomOperation"protocol">]
    member _.Protocol(state: Sas, protocol) =
        state.Builder.Protocol <- protocol
        state

    /// Optionally specify the time at which the shared access signature becomes valid. If omitted when DateTimeOffset.MinValue is used, start time for this call is assumed to be the time when the storage service receives the request.
    [<CustomOperation"startsOn">]
    member _.StartsOn(state: Sas, startsOn) =
        state.Builder.StartsOn <- startsOn
        state

let sas target expiresOn = SasBuilder(target, expiresOn)
let sasWithDefaults = sas (Account BlobAccountSasPermissions.Add) DateTimeOffset.UtcNow {()}
