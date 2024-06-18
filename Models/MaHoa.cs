using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
namespace ShopTechNoLoGy.Models
{
    public class MaHoa
    {
        public static string encryptSHA256(string PlainText)
        {
            string result = "";
            using(SHA256 bb = SHA256.Create()) {
                byte[] sourceData = Encoding.UTF8.GetBytes(PlainText);
                byte[] hashResult = bb.ComputeHash(sourceData);
                result = BitConverter.ToString(hashResult);
            }
            return result;
        }
    }
}