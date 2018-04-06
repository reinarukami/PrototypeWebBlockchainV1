using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using Npgsql;
using PrototypeWebBlockchain.Models;
using System.IO;
using System.Security.Cryptography;
using System;
using System.Web;

namespace PrototypeWebBlockchain.Repository
{
    public class ImageUpload 
    {
        public void ConvertUploadedFileToSha(HttpPostedFileBase nfilepath, string fileSavePath, string filepath)
        {
            string extension = Path.GetExtension(nfilepath.FileName);

            using (SHA256 sha256 = SHA256.Create())
            {
                using (Stream input = nfilepath.InputStream)
                {
                    var count = Directory.GetFiles(filepath, "*", SearchOption.TopDirectoryOnly).Length;

                    var shaValue = BitConverter.ToString(sha256.ComputeHash(input)).Replace("-", "");

                    shaValue = count + "_" + shaValue + extension;

                    shaValue = Path.Combine(filepath, shaValue);

                    File.Move(fileSavePath, shaValue);
                }
            }
        }

        public string ConvertSavedFileToSha(string fileSavePath)
        {
            string extension = Path.GetExtension(fileSavePath);

            using (SHA256 sha256 = SHA256.Create())
            {
                using (FileStream input = File.Open(fileSavePath, FileMode.Open))
                {
                    var shaValue = BitConverter.ToString(sha256.ComputeHash(input)).Replace("-", "");

                    shaValue = shaValue + extension;

                    return shaValue;
                }
            }
        }

    }
}