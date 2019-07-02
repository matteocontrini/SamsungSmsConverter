using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace SamsungSmsConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] inputFiles = Directory.GetFiles(@"C:\Users\mcont\Downloads\sms texts");

            string output = "<?xml version='1.0' encoding='UTF-8' standalone='yes' ?>";
            output += "<?xml-stylesheet type=\"text/xsl\" href=\"sms.xsl\"?>";

            XElement root = new XElement("smses");

            int total = 0;

            foreach (string file in inputFiles)
            {
                total += ParseFile(file, root);
            }

            root.SetAttributeValue("count", total);

            output += root;

            File.WriteAllText("out.xml", output);
        }

        private static int ParseFile(string inputFile, XElement root)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(inputFile);

            XmlNodeList nodes = doc.DocumentElement.SelectNodes("//message");

            foreach (XmlNode node in nodes)
            {
                string body = node.SelectSingleNode("body").InnerText;

                body = WebUtility.UrlDecode(body);

                root.Add(
                    new XElement("sms",
                        new XAttribute("address", node.SelectSingleNode("address").InnerText),
                        new XAttribute("date", node.SelectSingleNode("date").InnerText),
                        new XAttribute("type", node.SelectSingleNode("type").InnerText),
                        new XAttribute("body", body),
                        new XAttribute("read", node.SelectSingleNode("read").InnerText),
                        new XAttribute("status", -1),
                        new XAttribute("locked", node.SelectSingleNode("locked").InnerText)
                    )
                );
            }

            return nodes.Count;
        }
    }
}
