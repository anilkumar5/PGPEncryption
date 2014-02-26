using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PGPTestApp1.Utilities;
using System.IO;
using System.Text;
namespace PGPTestApp1.Controllers
{
    public class PGPController : Controller
    {
        //
        // GET: /PGP/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string Message)
        {
            this.EncryptMessage(Message);
            return View();
        }

        private void EncryptMessage(string Message)
        {
            StreamReader reader = new StreamReader(@"d:/keyPublic.txt");
            string pubKey = reader.ReadToEnd().ToString();
            var clearText = Message;
            using (var stream = pubKey.Streamify())
            {
                var key = stream.ImportPublicKey();
                using (var clearStream = clearText.Streamify())
                using (var cryptoStream = new MemoryStream())
                {
                    clearStream.PgpEncrypt(cryptoStream, key);
                    cryptoStream.Position = 0;
                    var cryptoString = cryptoStream.Stringify();
                    ViewBag.EncryptedMessage = cryptoString;
                }
            }
        }

        public string DecryptMessage(string message)
        {
            Stream inputData = null;
            string privateKey = string.Empty;
            byte[] array = Encoding.ASCII.GetBytes(message);
            inputData = new MemoryStream(array);
            using (StreamReader reader = new StreamReader(@"d:/keyPrivate.txt"))
            {
                privateKey = reader.ReadToEnd();
                
                Stream descryptedData = OpenPgpUtility.PgpDecrypt(inputData, privateKey, "anil");
                return descryptedData.Stringify();
            }
        }

        public string PGPDecrypt(string Message)
        {
            return this.DecryptMessage(Message);
            
        }
    }
}
