using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Npgsql;
using PrototypeWebBlockchain.Repository;
using PrototypeWebBlockchain.Models;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using Newtonsoft.Json;
using PrototypeWebBlockchain.Functions.Filters;
using System.Diagnostics;

namespace PrototypeWebBlockchain.Controllers
{
    [AuthorizeUser]
    public class TransactionController : Controller
    {
        private readonly ImageUpload imageupload;
       
        
        public TransactionController()
        {
         
            imageupload = new ImageUpload();
        }

        // GET: Home

        public ActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadImage(Image image)
        {
            if (HttpContext.Request.Files[0].ContentLength == 0)
            {
                ModelState.AddModelError("image", "Image is null , Please set Image");
                return View();
            }


            image.image = HttpContext.Request.Files[0];

            if (image.image != null)
            {
                // Validate the uploaded image(optional)
                var nfilepath = ConfigurationManager.AppSettings["FileImagePath"];
                var count = Directory.GetFiles(nfilepath, "*", SearchOption.TopDirectoryOnly).Length + 1;
                var imageName = count + "_" + image.image.FileName.ToString();

                // Get the complete file path
                var fileSavePath =  Path.Combine(nfilepath + imageName);

                // Save the uploaded file to "UploadedFiles" folder
                image.image.SaveAs(fileSavePath);
                image.hash = count + "_" + imageupload.ConvertSavedFileToSha(fileSavePath);

                try
                {
                    imageupload.ConvertUploadedFileToSha(image.image, fileSavePath, nfilepath);
                }
                catch
                {

                }


                HttpContext.Response.Clear();
                
                //Declare needed resources 
                string script = "<script src='/node_modules/web3/dist/web3.min.js'></script><script src='/Scripts/WebMainConfig.js'></script>" +

                //Declare the Account
                "<script>web3.eth.defaultAccount = web3.eth.accounts[0];" +

                //Declare the Contract Abi
                "var ContractAbi = web3.eth.contract([{'constant':false,'inputs':[{'name':'_id','type':'uint256'},{'name':'_fileid','type':'uint256'},{'name':'_fileHash','type':'string'},{'name':'_date','type':'string'}],'name':'AddFiles','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'anonymous':false,'inputs':[{'indexed':false,'name':'id','type':'uint256'},{'indexed':false,'name':'fileid','type':'uint256'},{'indexed':false,'name':'fileHash','type':'string'},{'indexed':false,'name':'date','type':'string'}],'name':'FileUploadEvent','type':'event'}]);" +

                //Declare the contract Address
                "var ImageContract = ContractAbi.at('0x538882ec49974f8815fee55ad7b40d6dd4b6b75d'); var fileid = web3.eth.blockNumber + 1;" +

                //Declare the transaction
                "ImageContract.AddFiles(" +Session["ID"] + ",fileid,'" + image.hash + "','" + DateTime.Now + "', { gas: 999999 });" +

                "window.location.href = '/Transaction/Transactionlist'; </script>";

                return Content(script);

            }

            return View();

         }

        public ActionResult Transactionlist(Identifier identifier)
        {

            identifier.hash = Session["Hash"].ToString();
            identifier.id = Session["ID"].ToString();
            return View(identifier);
        }

        [HttpPost]
        public JsonResult ValidateImages(string data)
        {
            var _imageFiles = JsonConvert.DeserializeObject<List<ImageJson>>(data);

            var nfilepath = ConfigurationManager.AppSettings["FileImagePath"];
         
            string file;

            foreach (var item in _imageFiles)
            {
                file = Path.Combine(nfilepath, item.filehash);
                try
                {
                    string shavalue = imageupload.ConvertSavedFileToSha(file);
                    string[] filehash = item.filehash.Split('_');
                    if(shavalue != filehash[1])
                    {
                        item.filehash = "Error/tampered.jpg";
                    }
                    
                }

                catch
                {
                        item.filehash = "Error/deleted.jpg";
                }
            }

            return new JsonResult() { Data = new { ImageFiles = _imageFiles } };
        }

    }
}