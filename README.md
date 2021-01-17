# Teeri

<p align="center">
<img src="https://raw.githubusercontent.com/ssiltanen/Teeri/main/Logo.png" width="200px"/>
</p>

Teeri is a simple F# wrapper on top of Azure Blob Storage .NET SDK. Teeri aims at making uploading and downloading blobs simpler and to abstract away the unnecessary details of the underlaying SDK.

## Overview of supported settings

```fsharp
// To upload a blob with all the optional settings
uploadBlob ("folder/file.txt", FromStream stream) {
    cacheControl "max-age=3600"
    cancellationToken token
    contentDisposition "save_as_name.txt"
    contentEncoding UTF8
    contentHash md5Bytes
    contentLanguage "Finnish"
    contentType "application/text"
    accessTier AccessTier.Cool
    conditions blobRequestConditions
    metadata [ "meta1", "value"; "meta2", "value" ]
    progressHandler iprogress
    tags [ "tag1", "value"; "tag2", "value" ]
    transferOptions transferOptions
} |> uploadAsync blobClient

// To upload with the default options
uploadBlobWithDefaults ("folder/file2.txt", FromStream stream) 
|> uploadAsync blobClient

// To read a blob with all the optional settings
downloadBlob "folder/file.txt" {
    position 0L
    bufferSize 100
    requestConditions conditions
    cancellationToken token
} |> openReadAsync blobClient

// To read a blob with the default options
downloadBlobWithDefaults "folder/file2.txt"
|> openReadAsync blobClient
```

All settings inside the computation expressions are optional, but the parameter values before {} are mandatory.

## How to use

### Upload blob builder

**Mandatory** values:

- `path`: File path to upload file to. Does not including the container name.
- `Content`: File contents, either a stream or string. Type is specified with a union types `FromStream` and `FromString`.

**Optional** values:

- `cacheControl`

String value specifying directives for caching mechanisms. Value is passed to underlying SDK as is. Eg. `max-age=3600`

- `cancellationToken`

[System.Threading.CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken?view=net-5.0) to propagate notifications that the operation should be cancelled. Value is passed to underlying SDK as is.

- `contentDisposition`

Conveys additional information about how to process the response payload, and also can be used to attach additional metadata. For example, if set to attachment, it indicates that the user-agent should not display the response, but instead show a Save As dialog with a filename other than the blob name specified. Value is passed to underlying SDK as is.

- `contentEncoding`

Teeri specific ContentEncoding union type value. Specifies which content encodings have been applied to the blob. This value is returned to the client when the Get Blob operation is performed on the blob resource. The client can use this value when returned to decode the blob content. Defaults to UTF8 when uploading a string value. WHen uploading a stream, no default value is set.

- `contentHash`

Byte array of MD5 hash of the blob content. This hash is used to verify the integrity of the blob during transport. When this header is specified, the storage service checks the hash that has arrived with the one that was sent. If the two hashes do not match, the operation will fail with error code 400 (Bad Request). Value is passed to underlying SDK as is.

- `contentLanguage`

String value. Specifies the natural languages used by this resource. Value is passed to underlying SDK as is.

- `contentType`

String value MIME content type of the blob. Value is passed to underlying SDK as is.

- `accessTier`

[Azure.Storage.Blobs.Models.BlobUploadOptions.AccessTier](https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobuploadoptions?view=azure-dotnet) to set on the Block Blob. Value is passed to underlying SDK as is.

- `conditions`

[Azure.Storage.Blobs.Models.BlobRequestConditions](https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobrequestconditions?view=azure-dotnet) to add conditions on the upload of this Block Blob. Value is passed to underlying SDK as is.

- `metadata`

String tuple sequence of custom metadata to set for this blob. Call this only once in a builder since it overwrites previous values on consecutive calls. Values are passed to underlying SDK as is.

- `progressHandler`

[System.IProgress<int64>](https://docs.microsoft.com/en-us/dotnet/api/system.iprogress-1?view=net-5.0) to provide progress updates about data transfers. Value is passed to underlying SDK as is.

- `tags`

String tuple sequence of tags to set for this block blob. Call this only once in a builder since it overwrites previous values on consecutive calls. Values are passed to underlying SDK as is.

- `transferOptions`

[Azure.Storage.StorageTransferOptions](https://docs.microsoft.com/en-us/dotnet/api/azure.storage.storagetransferoptions?view=azure-dotnet) to configure parallel transfer behavior. Value is passed to underlying SDK as is.

### Download blob builder

**Mandatory** values:

- `path`: File path to download file from. Does not including the container name.

**Optional** values:

- `position`

Int64 position within the blob to begin the stream. Defaults to the beginning of the blob. Value is passed to underlying SDK as is.

- `bufferSize`

Int buffer size to use when the stream downloads parts of the blob. Defaults to 1 MB. Value is passed to underlying SDK as is.

- `requestConditions`

[Azure.Storage.Blobs.Models.BlobRequestConditions](https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobrequestconditions?view=azure-dotnet) to add conditions on the download of the blob.

- `cancellationToken`

[System.Threading.CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken?view=net-5.0) to propagate notifications that the operation should be cancelled. Value is passed to underlying SDK as is.

### Supported operations

#### BlobClient operations

- `uploadAsync`

Used to upload a blob to container. When uploading string content, uses content encoding value from Blob builder to determine how string is encoded. If no value is provided, defaults to UTF8.

- `upload`

Synchronous version of `uploadAsync`.

- `openReadAsync`

Opens a stream to read a blob from container.

- `openRead`

Synchronous version of `openReadAsync`.

- `downloadAsync`

Downloads blob contents to string. Uses blob's content encoding value in decoding to string. If blob has no encoding value, UTF8 is used.

- `download`

Synchronous version of `downloadAsync`.

- `createSharedKeyCredential`

Generates a [StorageSharedKeyCredential](https://docs.microsoft.com/en-us/dotnet/api/azure.storage.storagesharedkeycredential?view=azure-dotnet) object from storage connection string.

#### BlobContainerClient operations

- `uploadBlobAsync`

Used to upload a blob to container. When uploading string content, uses content encoding value from Blob builder to determine how string is encoded. If no value is provided, defaults to UTF8.

- `uploadBlob`

Synchronous version of `uploadBlobAsync`.

- `openReadblobAsync`

Opens a stream to read a blob from container.

- `openReadblob`

Synchronous version of `openReadAsync`.

- `downloadBlobAsync`

Downloads blob contents to string. Uses blob's content encoding value in decoding to string. If blob has no encoding value, UTF8 is used.

- `downloadBlob`

Synchronous version of `downloadBlobAsync`.

## Help! Teeri does not support my use case

Using Teeri does not exclude the possibility of using the underlying SDK without Teeri. In fact, Teeri positions itself ss a helper DSL. If a feature is missing that would make developer life easier, check out how to contribute your ideas to Teeri.

## Contributions

When contributing to this repository, please first discuss the change you wish to make via an open issue before submitting a pull request. For new feature requests please describe your idea in more detail and how it could benefit other users as well. Please be aware that Fasaani aims to be a light, thin abstraction over the Azure SDK.

After your change has been approved, please submit your PR against `dev` branch.

Use existing code as a guideline for code conventions and keep documentation as well as tests up-to-date.