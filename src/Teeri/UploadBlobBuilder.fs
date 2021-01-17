[<AutoOpen>]
module Teeri.UploadBlobBuilder

open System
open System.Threading
open System.Collections.Generic
open Azure.Storage.Blobs.Models

type UploadBlobBuilder (path: string, content: BlobContent) =

    member _.Yield _ =
        { Path = path
          Content = content
          HttpHeaders = BlobHttpHeaders()
          UploadOptions = BlobUploadOptions()
          CancellationToken = CancellationToken.None }

    member this.Zero() =
        this.Yield()

    /// Specify directives for caching mechanisms.
    [<CustomOperation"cacheControl">]
    member _.CacheControl(state: UploadBlob, cacheControl) =
        state.HttpHeaders.CacheControl <- cacheControl
        state

    /// Optional System.Threading.CancellationToken to propagate notifications that the operation should be cancelled.
    [<CustomOperation"cancellationToken">]
    member _.CancellationToken(state: UploadBlob, token) =
        { state with CancellationToken = token }

    /// Conveys additional information about how to process the response payload, and also can be used to attach additional metadata.
    /// For example, if set to attachment, it indicates that the user-agent should not display the response,
    /// but instead show a Save As dialog with a filename other than the blob name specified.
    [<CustomOperation"contentDisposition">]
    member _.ContentDisposition(state: UploadBlob, disposition) =
        state.HttpHeaders.ContentDisposition <- disposition
        state

    /// Specifies which content encodings have been applied to the blob.
    /// This value is returned to the client when the Get Blob operation is performed on the blob resource.
    /// The client can use this value when returned to decode the blob content.
    [<CustomOperation"contentEncoding">]
    member _.ContentEncoding(state: UploadBlob, encoding: ContentEncoding) =
        state.HttpHeaders.ContentEncoding <- encoding.Value
        state

    /// An MD5 hash of the blob content.
    /// This hash is used to verify the integrity of the blob during transport.
    /// When this header is specified, the storage service checks the hash that has arrived with the one that was sent.
    /// If the two hashes do not match, the operation will fail with error code 400 (Bad Request).
    [<CustomOperation"contentHash">]
    member _.ContentHash(state: UploadBlob, hash) =
        state.HttpHeaders.ContentHash <- hash
        state

    /// Specifies the natural languages used by this resource.
    [<CustomOperation"contentLanguage">]
    member _.ContentLanguage(state: UploadBlob, language) =
        state.HttpHeaders.ContentLanguage <- language
        state

    /// Sets the MIME content type of the blob
    [<CustomOperation"contentType">]
    member _.ContentType(state: UploadBlob, contentType) =
        state.HttpHeaders.ContentType <- contentType
        state

    /// Optional Azure.Storage.Blobs.Models.BlobUploadOptions.AccessTier to set on the Block Blob.
    [<CustomOperation"accessTier">]
    member _.AccessTier(state: UploadBlob, tier) =
        state.UploadOptions.AccessTier <- Nullable tier
        state

    /// Optional Azure.Storage.Blobs.Models.BlobRequestConditions to add conditions on the upload of this Block Blob.
    [<CustomOperation"conditions">]
    member _.Conditions(state: UploadBlob, conditions) =
        state.UploadOptions.Conditions <- conditions
        state

    /// Optional custom metadata to set for this blob.
    /// Call this only once in a builder since it overwrites previous values on consecutive calls.
    [<CustomOperation"metadata">]
    member _.Metadata(state: UploadBlob, metadata) =
        let dictionary =
            metadata
            |> Seq.map KeyValuePair.Create
            |> Dictionary
        state.UploadOptions.Metadata <- dictionary
        state

    /// Optional System.IProgress<int64> to provide progress updates about data transfers.
    [<CustomOperation"progressHandler">]
    member _.ProgressHandler(state: UploadBlob, progress) =
        state.UploadOptions.ProgressHandler <- progress
        state

    /// Options tags to set for this block blob.
    /// Call this only once in a builder since it overwrites previous values on consecutive calls.
    [<CustomOperation"tags">]
    member _.Tags(state: UploadBlob, tags) =
        let dictionary =
            tags
            |> Seq.map KeyValuePair.Create
            |> Dictionary
        state.UploadOptions.Tags <- dictionary
        state

    /// Optional Azure.Storage.StorageTransferOptions to configure parallel transfer behavior.
    [<CustomOperation"transferOptions">]
    member _.TransferOptions(state: UploadBlob, transferOptions) =
        state.UploadOptions.TransferOptions <- transferOptions
        state

let uploadBlob path content = UploadBlobBuilder(path, content)
let uploadBlobWithDefaults path content = uploadBlob path content {()}