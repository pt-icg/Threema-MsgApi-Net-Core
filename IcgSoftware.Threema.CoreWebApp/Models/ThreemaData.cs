using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace IcgSoftware.Threema.CoreWebApp.Models
{
    public class ThreemaData
    {
        public List<ThreemaAccount> ThreemaAccounts { get; set; }

        public ThreemaData()
        {
            ThreemaAccounts = new List<ThreemaAccount>();
        }

        public ThreemaAccount Get(string threemaId)
        {
            return ThreemaAccounts.SingleOrDefault(i => i.ThreemaId == threemaId);
        }

        public void XmlSerialize(String fileName)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            //settings.NewLineChars = "\r";
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            var serializer = new XmlSerializer(this.GetType());
            using (var writer = XmlWriter.Create(fileName, settings))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static ThreemaData XmlDeserialize(String fileName)
        {
            var serializer = new XmlSerializer(typeof(ThreemaData));
            using (var reader = XmlReader.Create(fileName))
            {
                return (ThreemaData)serializer.Deserialize(reader);
            }
        }

    }

    public class ThreemaAccount
    {
        public ThreemaAccount()
        {
        }

        public ThreemaAccount(string threemaId, string publicKey)
        {
            ThreemaId = threemaId;
            PublicKey = publicKey;
        }

        public string ThreemaId { get; set; }

        public string PublicKey { get; set; }
    }
}
