[<AutoOpen>]
module Teeri.Operations

open System
open System.IO
open Azure.Storage
open Azure.Storage.Blobs
open Azure.Storage.Sas
open FSharp.Control.Tasks

module Blob =

    let internal uploadStreamAsync (client: BlobClient) (stream: Stream) (blob: Blob) =
        task {
            blob.UploadOptions.HttpHeaders <- blob.HttpHeaders
            let! response = client.UploadAsync(stream, blob.UploadOptions, blob.CancellationToken)
            return response.Value
        }

    let internal uploadStringAsync (client: BlobClient) (content: string) (blob: Blob) =
        task {
            let bytes =
                match Option.ofObj blob.HttpHeaders.ContentEncoding with
                | Some enc when enc = ASCII.Value -> Text.Encoding.ASCII.GetBytes content
                | Some enc when enc = BigEndianUnicode.Value -> Text.Encoding.BigEndianUnicode.GetBytes content
                | Some enc when enc = Latin1.Value -> Text.Encoding.Latin1.GetBytes content
                | Some enc when enc = Unicode.Value -> Text.Encoding.Unicode.GetBytes content
                | Some enc when enc = UTF32.Value -> Text.Encoding.UTF32.GetBytes content
                | Some enc when enc = UTF8.Value -> Text.Encoding.UTF8.GetBytes content
                | Some enc when enc = Default.Value -> Text.Encoding.Default.GetBytes content
                | Some _
                | None ->
                    blob.HttpHeaders.ContentEncoding <- UTF8.Value
                    Text.Encoding.UTF8.GetBytes content

            use stream = new IO.MemoryStream(bytes)
            // Task is awaited here to make sure stream isnt disposed too early
            let! blobContentInfo = uploadStreamAsync client stream blob
            return blobContentInfo
        }

    /// When uploading string content, uses content encoding value from Blob builder to determine how string is encoded.
    /// If no value is provided, defaults to UTF8.
    let uploadAsync (client: BlobClient) (blob: Blob) =
        match blob.Content with
        | FromStream stream -> uploadStreamAsync client stream blob
        | FromString str -> uploadStringAsync client str blob

    /// When uploading string content, uses content encoding value from Blob builder to determine how string is encoded.
    /// If no value is provided, defaults to UTF8.
    let upload (client: BlobClient) (blob: Blob) =
        match blob.Content with
        | FromStream stream -> uploadStreamAsync client stream blob
        | FromString str -> uploadStringAsync client str blob
        |> Async.AwaitTask
        |> Async.RunSynchronously

    let openReadAsync (client : BlobClient) (blob : ReadBlob) =
        client.OpenReadAsync(blob.Position, (Option.toNullable blob.BufferSize), blob.BlobRequestConditions, blob.CancellationToken)

    let openRead (client : BlobClient) (blob : ReadBlob) =
        client.OpenRead(blob.Position, (Option.toNullable blob.BufferSize), blob.BlobRequestConditions, blob.CancellationToken)

    let downloadAsync (client : BlobClient) (blob : ReadBlob) =
        task {
            use! stream = openReadAsync client blob
            use streamReader = new StreamReader(stream)
            // Task is awaited here to make sure stream isnt disposed too early
            let! content = streamReader.ReadToEndAsync()
            return content
        }

    let download (client : BlobClient) (blob : ReadBlob) =
        use stream = openRead client blob
        use streamReader = new StreamReader(stream)
        streamReader.ReadToEnd()

module Container =

    /// When uploading string content, uses content encoding value from Blob builder to determine how string is encoded.
    /// If no value is provided, defaults to UTF8.
    let uploadBlobAsync (container: BlobContainerClient) (blob: Blob) =
        match blob.Content with
        | FromStream stream -> Blob.uploadStreamAsync (container.GetBlobClient(blob.Path)) stream blob
        | FromString str -> Blob.uploadStringAsync (container.GetBlobClient(blob.Path)) str blob

    /// When uploading string content, uses content encoding value from Blob builder to determine how string is encoded.
    /// If no value is provided, defaults to UTF8.
    let uploadBlob (container: BlobContainerClient) (blob: Blob) =
        match blob.Content with
        | FromStream stream -> Blob.uploadStreamAsync (container.GetBlobClient(blob.Path)) stream blob
        | FromString str -> Blob.uploadStringAsync (container.GetBlobClient(blob.Path)) str blob
        |> Async.AwaitTask
        |> Async.RunSynchronously

    let openReadblobAsync (container: BlobContainerClient) (blob: ReadBlob) =
        Blob.openReadAsync (container.GetBlobClient(blob.Path)) blob

    let openReadBlob (container: BlobContainerClient) (blob: ReadBlob) =
        Blob.openRead (container.GetBlobClient(blob.Path)) blob

    let downloadBlobAsync (container: BlobContainerClient) (blob: ReadBlob) =
        Blob.downloadAsync (container.GetBlobClient(blob.Path)) blob

    let downloadBlob (container: BlobContainerClient) (blob: ReadBlob) =
        Blob.download (container.GetBlobClient(blob.Path)) blob

module Sas =

    let createSharedKeyCredential (connectionString: string) =
        let connStringPairs =
            connectionString.Split ';'
            |> Array.map (fun value ->
                match value.Split '=' |> Array.toList with
                | head :: tail -> head, String.concat "=" tail
                | _ -> failwith "Invalid connection string value pair")

        let getValue key =
            connStringPairs
            |> Array.tryPick (fun (name,value) -> if name = key then Some value else None)
            |> Option.defaultWith (fun _ -> failwithf "Key %s was not found from connection string" key)

        let account = getValue "AccountName"
        let key = getValue "AccountKey"

        StorageSharedKeyCredential(account, key)

    let generateBlobSas container blob startsOn expiresOn (permissions: BlobSasPermissions) sharedKeyCredential =
        let builder = BlobSasBuilder()
        builder.BlobContainerName <- container
        builder.BlobName <- blob
        builder.StartsOn <- startsOn
        builder.ExpiresOn <- expiresOn
        builder.SetPermissions permissions
        builder.ToSasQueryParameters sharedKeyCredential