using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotCMIS;
using DotCMIS.Client.Impl;
using DotCMIS.Client;
using System.IO;
using DotCMIS.Data.Impl;

/**
 * Upload documents to a CMIS server using DotCMIS.
 */
using System.Threading;


namespace dotcmis_upload
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create session.
            var parameters = new Dictionary<string, string>();
            parameters[SessionParameter.BindingType] = BindingType.AtomPub;
            parameters[SessionParameter.AtomPubUrl] = "http://192.168.0.67:8080/alfresco/cmisatom"; // MODIFY HERE
            parameters[SessionParameter.User] = "admin"; // MODIFY HERE
            parameters[SessionParameter.Password] = "admin"; // MODIFY HERE
            var factory = SessionFactory.NewInstance();
            ISession session = factory.GetRepositories(parameters)[0].CreateSession();

			IFolder folder =  (IFolder) session.GetObjectByPath("/Sites/site1/documentLibrary/upload");

			while (true)
			{
				UploadRandomDocumentTo(folder);
				Thread.Sleep(1000);
			}
        }

		private static void UploadRandomDocumentTo(IFolder folder)
        {
			string filename = "file_" + Guid.NewGuid() + ".bin";

			//byte[] content = UTF8Encoding.UTF8.GetBytes("Hello World!");
			int sizeInMb = 4;
			byte[] data = new byte[sizeInMb * 1024 * 1024];
			Random rng = new Random();
			rng.NextBytes(data);

			IDictionary<string, object> properties = new Dictionary<string, object>();
			properties[PropertyIds.Name] = filename;
			properties[PropertyIds.ObjectTypeId] = "cmis:document";

			ContentStream contentStream = new ContentStream();
			contentStream.FileName = filename;
			contentStream.MimeType = "application/octet-stream";
			contentStream.Length = data.Length;
			contentStream.Stream = new MemoryStream(data);

			IDocument doc = folder.CreateDocument(properties, contentStream, null);

			contentStream.Stream.Close();
			contentStream.Stream.Dispose();

			Console.WriteLine ("Uploaded " + filename);
        }
    }
}