namespace Teeri

open System.IO
open System.Threading
open Azure.Storage
open Azure.Storage.Blobs.Models
open Azure.Storage.Sas

type ContentEncoding = ASCII | BigEndianUnicode | Latin1 | Unicode | UTF32 | UTF8 | Default with
    member this.Value = this.ToString()

type BlobContent =
    | FromStream of Stream
    | FromString of string

type UploadBlob =
    { Path: string
      Content: BlobContent
      HttpHeaders: BlobHttpHeaders
      UploadOptions: BlobUploadOptions
      CancellationToken: CancellationToken }

type DownloadBlob =
    { Path: string
      Position: int64
      BufferSize: int option
      BlobRequestConditions: BlobRequestConditions
      CancellationToken: CancellationToken }

type SasTarget =
    | Account of BlobAccountSasPermissions
    | Container of name: string * BlobContainerSasPermissions
    | Blob of name: string * BlobSasPermissions
    | Snapshot of name: string * BlobSasPermissions
    | BlobVersion of versionId: string * BlobSasPermissions

type Sas =
    { Builder: BlobSasBuilder }
    with
    static member ToQueryParameters (credentials: StorageSharedKeyCredential) (sas: Sas) =
        sas.Builder.ToSasQueryParameters(credentials).ToString()