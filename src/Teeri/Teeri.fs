namespace Teeri

open System.IO
open System.Threading
open Azure.Storage.Blobs.Models

type ContentEncoding = ASCII | BigEndianUnicode | Latin1 | Unicode | UTF32 | UTF8 | Default with
    member this.Value = this.ToString()

type BlobContent =
    | FromStream of Stream
    | FromString of string

type Blob =
    { Path: string
      Content: BlobContent
      HttpHeaders: BlobHttpHeaders
      UploadOptions: BlobUploadOptions
      CancellationToken: CancellationToken }

type ReadBlob =
    { Path: string
      Position: int64
      BufferSize: int option
      BlobRequestConditions: BlobRequestConditions
      CancellationToken: CancellationToken }