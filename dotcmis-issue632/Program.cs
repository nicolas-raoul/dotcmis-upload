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
 * Code demonstrating DotCMIS issue 632.
 * Modify AtomPubUrl, User, Password, remoteFilePath
 * Output:
 * 
 * before
 * after
 * before
 * [program gets stuck here]
 *
 */
namespace dotcmis_issue632
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create session.
            var parameters = new Dictionary<string, string>();
            parameters[SessionParameter.BindingType] = BindingType.AtomPub;
            parameters[SessionParameter.AtomPubUrl] = "http://192.168.0.22:8080/alfresco/cmisatom"; // MODIFY HERE
            parameters[SessionParameter.User] = "admin"; // MODIFY HERE
            parameters[SessionParameter.Password] = "admin"; // MODIFY HERE
            var factory = SessionFactory.NewInstance();
            ISession session = factory.GetRepositories(parameters)[0].CreateSession();

            // Update a document twice.
            string remoteFilePath = "/User Homes/test.txt";
            Document doc = (Document)session.GetObjectByPath(remoteFilePath);
            UpdateFile(doc);
            UpdateFile(doc); // This one never returns. 
        }

        private static void UpdateFile(IDocument doc)
        {
            Stream data = File.OpenRead("../../local.txt");

            ContentStream contentStream = new ContentStream();
            contentStream.FileName = doc.ContentStreamFileName;
            contentStream.Length = data.Length;
            contentStream.MimeType = "application/octet-stream";
            contentStream.Stream = data;
            contentStream.Stream.Flush();

            Console.WriteLine("before");
            doc.SetContentStream(contentStream, true, true);
            Console.WriteLine("after"); // Not reached the second time the method is called.

            data.Close();
            data.Dispose();
            contentStream.Stream.Close();
        }
    }
}
