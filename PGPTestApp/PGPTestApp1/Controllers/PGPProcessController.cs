using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PGPTestAppConsole.KeyModel;
using System.IO;
using PGPTestApp1.Utilities;
using System.Text;

namespace PGPTestApp1.Controllers
{
    public class PGPProcessController : Controller
    {
        //
        // GET: /PGPProcess/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EmailRegister()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EmailRegister(string Email, string Password)
        {
            string path = @"d:/keys/MVC/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (this.GenerateKeys(Email, Password, path))
            {
                using (StreamReader readPrivateFile = new StreamReader(path + "secret.txt"))
                {
                    ViewBag.PrivateKey = readPrivateFile.ReadToEnd();
                }
                using (StreamReader readPublicFile = new StreamReader(path + "pub.txt"))
                {
                    ViewBag.PublicKey = readPublicFile.ReadToEnd();
                }
            }
            return View();
        }

        public ActionResult EncryptMessage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EncryptMessage(string PublicKey, string Message)
        {
            ViewBag.EncryptedMessage = this.EncryptedMessage(PublicKey, Message);
            return View();
        }

        public ActionResult DecryptMessage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DecryptMessage(string PrivateKey, string Password, string EncryptedMessage)
        {
            ViewBag.DecryptedMessage = this.DecryptMessages(PrivateKey, Password, EncryptedMessage);
            return View();
        }

        private bool GenerateKeys(string emailAddress, string password, string directoryToSaveFile)
        {
            try
            {
                KeyGenerator keyModel = new KeyGenerator();
                keyModel.GenerateKey(emailAddress, password, directoryToSaveFile);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string EncryptedMessage(string PublicKey, string Message)
        {

            //StreamReader reader = new StreamReader(@"d:/keys/pub.txt");//new StreamReader(//
            string pubKey = PublicKey;// reader.ReadToEnd().ToString();
            // string pubKey = ConfigurationSettings.AppSettings["publicKey"];
            var clearText = Message;
            using (var stream = pubKey.Streamify())
            {
                var key = stream.ImportPublicKey();
                using (var clearStream = clearText.Streamify())
                using (var cryptoStream = new MemoryStream())
                {
                    clearStream.PgpEncrypt(cryptoStream, key);
                    cryptoStream.Position = 0;
                    return cryptoStream.Stringify();
                }
            }
        }

        private string DecryptMessages(string PrivateKey, string password, string decryptedMessage)
        {
            Stream inputData = null;
            string privateKey = string.Empty;
            byte[] array = Encoding.ASCII.GetBytes(decryptedMessage);
            inputData = new MemoryStream(array);
            //using (StreamReader reader = new StreamReader(@"d:/Keys/secret.txt"))
            //{
            privateKey = PrivateKey;// reader.ReadToEnd();
            Stream descryptedData = OpenPgpUtility.PgpDecrypt(inputData, privateKey, password);
            return descryptedData.Stringify();
            //}
        }
    }
}
