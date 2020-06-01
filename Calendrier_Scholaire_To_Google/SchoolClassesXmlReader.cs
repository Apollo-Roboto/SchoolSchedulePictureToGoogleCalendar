using CalendrierScholaireToGoogle;
using System.Text.RegularExpressions;
using System.Xml;

namespace SchoolScheduleLibrary
{
    class SchoolClassesXmlReader
    {
        private XmlDocument xmlDoc = new XmlDocument();

        public SchoolClassesXmlReader(string xmlFilePath)
        {
            // load xml file    
            xmlDoc.Load(xmlFilePath);
        }

        private XmlNode findNodeByCodeAttribute(string code)
        {
            XmlNode result;

            // search a node containing the attribute code with the code value
            result = xmlDoc.SelectNodes($"//class[@code='{code}']")[0];

            // if code didn't found anything, try to find a codeMatch
            if(result == null)
            {
                XmlNodeList codeMatches = xmlDoc.SelectNodes($"//class[@codeMatch]");
                foreach (XmlNode node in codeMatches)
                {
                    // try to match and if a match is found then try to get the node
                    Regex regex = new Regex(node.Attributes["codeMatch"].Value);
                    if (regex.Match(code).Success)
                    {
                        result = node;
                    }
                }
            }

            // will return null if nothing is found
            return result;
        }

        public string getCode(string code)
        {
            XmlNode node = findNodeByCodeAttribute(code);
            return node.Attributes["code"].Value;
        }

        public string getName(string code)
        {
            XmlNode node = findNodeByCodeAttribute(code);
            return node.Attributes["name"].Value;
        }

        public string getFriendlyName(string code)
        {
            XmlNode node = findNodeByCodeAttribute(code);
            return node.Attributes["friendlyName"].Value;
        }

        public string getTeacher(string code)
        {
            XmlNode node = findNodeByCodeAttribute(code);
            return node.Attributes["teacher"].Value;
        }

        public string getLocation(string code)
        {
            XmlNode node = findNodeByCodeAttribute(code);
            if (node.Attributes["location"] == null)
                return null;
            return node.Attributes["location"].Value;
        }

        public string getColorId(string code)
        {
            XmlNode node = findNodeByCodeAttribute(code);
            if (node.Attributes["colorId"] == null)
                return getDefaultColorId();
            return node.Attributes["colorId"].Value;
        }

        

        public string getTimeZone()
        {
            XmlNode setting = xmlDoc.SelectSingleNode("//setting");
            return setting.Attributes["timeZone"].Value;
        }

        public int getYear()
        {
            XmlNode setting = xmlDoc.SelectSingleNode("//setting");
            string result = setting.Attributes["year"].Value;
            return int.Parse(result);
        }

        public string getGroup()
        {
            XmlNode setting = xmlDoc.SelectSingleNode("//setting");
            return setting.Attributes["group"].Value;
        }

        public string getDefaultColorId()
        {
            XmlNode setting = xmlDoc.SelectSingleNode("//setting");
            return setting.Attributes["defaultColorId"].Value;
        }
    }
}
