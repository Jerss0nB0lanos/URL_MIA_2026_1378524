using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MIA_AzureBlob.Services;

public class BlobStorageService
{
	private readonly BlobContainerClient _containerClient;

	public BlobStorageService(string connectionString, string containerName)
	{
		var serviceClient = new BlobServiceClient(connectionString);
		_containerClient = serviceClient.GetBlobContainerClient(containerName);

		// Crea el contenedor si no existe
		_containerClient.CreateIfNotExists(PublicAccessType.None);
	}

	public async Task<string> SubirArchivoAsync(string rutaLocal)
	{
		if (!File.Exists(rutaLocal))
			throw new FileNotFoundException("El archivo no existe.", rutaLocal);

		string nombreArchivo = Path.GetFileName(rutaLocal);
		BlobClient blobClient = _containerClient.GetBlobClient(nombreArchivo);

		await blobClient.UploadAsync(rutaLocal, overwrite: true);

		return nombreArchivo;
	}

	
	public async Task<List<(string Nombre, long Tamanio)>> ListarArchivosAsync()
	{
		var lista = new List<(string, long)>();

		await foreach (BlobItem blob in _containerClient.GetBlobsAsync())
		{
			long size = blob.Properties.ContentLength ?? 0;
			lista.Add((blob.Name, size));
		}

		return lista;
	}

	
	public async Task<bool> ExisteBlobAsync(string nombreBlob)
	{
		BlobClient blobClient = _containerClient.GetBlobClient(nombreBlob);
		return await blobClient.ExistsAsync();
	}

	
	public async Task<string> DescargarArchivoAsync(string nombreBlob, string carpetaDestino)
	{
		BlobClient blobClient = _containerClient.GetBlobClient(nombreBlob);

		if (!await blobClient.ExistsAsync())
			throw new FileNotFoundException($"El blob '{nombreBlob}' no existe.");

		if (!Directory.Exists(carpetaDestino))
			Directory.CreateDirectory(carpetaDestino);

		string rutaDestino = Path.Combine(carpetaDestino, nombreBlob);

		await blobClient.DownloadToAsync(rutaDestino);

		return rutaDestino;
	}


	public async Task EliminarArchivoAsync(string nombreBlob)
	{
		BlobClient blobClient = _containerClient.GetBlobClient(nombreBlob);

		if (!await blobClient.ExistsAsync())
			throw new FileNotFoundException($"El blob '{nombreBlob}' no existe.");

		await blobClient.DeleteAsync();
	}
}