namespace Teeri

open Azure.Storage
open Azure.Storage.Blobs
open Azure.Storage.Blobs.Models

type Blob =
    { HttpHeaders : BlobHttpHeaders
      UploadOptions : BlobUploadOptions }
