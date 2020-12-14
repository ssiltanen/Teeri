[<AutoOpen>]
module Teeri.ReadBlobBuilder

open System.Threading
open Azure.Storage.Blobs.Models

type ReadBlobBuilder (path: string) =

    member _.Yield path =
        { Path = path
          Position = 0L
          BufferSize = None
          BlobRequestConditions = BlobRequestConditions()
          CancellationToken = CancellationToken.None }

    /// The buffer size to use when the stream downloads parts of the blob. Defaults to 1 MB.
    [<CustomOperation"bufferSize">]
    member _.BufferSize(state: ReadBlob, size) =
        { state with BufferSize = Some size }

    /// Optional System.Threading.CancellationToken to propagate notifications that the operation should be cancelled.
    [<CustomOperation"cancellationToken">]
    member _.CancellationToken(state: ReadBlob, token) =
        { state with CancellationToken = token }

    /// The position within the blob to begin the stream. Defaults to the beginning of the blob.
    [<CustomOperation"position">]
    member _.BufferSize(state: ReadBlob, position) =
        { state with Position = position }

    /// Optional Azure.Storage.Blobs.Models.BlobRequestConditions to add conditions on the download of the blob.
    [<CustomOperation"requestConditions">]
    member _.RequestConditions(state: ReadBlob, conditions) =
        { state with BlobRequestConditions = conditions }

let readBlob = ReadBlobBuilder