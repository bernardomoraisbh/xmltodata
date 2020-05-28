using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace XmlManager
{
    class Program
    {
        public static string NewCreation(XmlDocument xDoc, Data data)
        {
            var rootData = new ObjectData();
            var firstObject = new ObjectData();
            firstObject.Noun = xDoc.DocumentElement.Name;

            TreatNewCreationElement(xDoc.LastChild, firstObject, "");

            rootData.Objects.Add(firstObject);
            rootData.Noun = "TESTE_BOOK";
            data.Objects.Add(rootData);
            return "";
        }

        public static void TreatNewCreationElement(XmlNode xmlRoot, JsonData obj, string nameFather)
        {
            if (xmlRoot is XmlElement)
            {
                // PEGAR ATRIBUTOS DO xmlRoot, nó atual, e adicionar como value ao obj
                if (xmlRoot.Attributes != null)
                    foreach (XmlAttribute attr in xmlRoot.Attributes)
                    {
                        var valueData1 = new ValueData();
                        valueData1.Key = attr.Name;
                        valueData1.Value = attr.InnerText;
                        obj.Values.Add(valueData1);
                    }

                if (xmlRoot.HasChildNodes)
                {
                    if (xmlRoot.FirstChild is XmlText)
                    {
                        TreatNewCreationElement(xmlRoot.FirstChild, obj, xmlRoot.Name);
                    }
                    else
                    {
                        TreatNewCreationElement(xmlRoot.FirstChild, obj, xmlRoot.Name);
                    }
                }
                if (xmlRoot.NextSibling != null)
                    TreatNewCreationElement(xmlRoot.NextSibling, obj, xmlRoot.Name);
            }
            else if (xmlRoot is XmlText)
            {
                Console.WriteLine(nameFather + " " + xmlRoot.Value);
            }
            else if (xmlRoot is XmlComment)
            { }
        }

        private static void DoWork(XmlNode node)
        {
            if (node.Attributes != null)
                if (node.Name != null)
                    Console.WriteLine(node.Value);
        }

        //public static void TreatNewCreationNode(XmlElement xmlRoot, JsonData obj)
        //{
        //    foreach (var values in xmlRoot)
        //    {
        //        switch (values)
        //        {
        //            case XmlText _:
        //                break;

        //            case XmlElement _:
        //                TreatNewCreationNode(xmlRoot.FirstChild, obj);
        //                break;
        //        }
        //    }
        //}


        public static string CreateDataXML (XmlDocument xDoc, Data data)
        {
            var rootData = new ObjectData();
            var firstObject = new ObjectData();
            firstObject.Noun = xDoc.DocumentElement.Name;

            TreatXmlRootNode(xDoc.DocumentElement, firstObject,0);
            
            rootData.Objects.Add(firstObject);
            rootData.Noun = "TESTE_BOOK";
            data.Objects.Add(rootData);
            return "";
        }

        public static void TreatXmlRootNode(XmlElement xmlRoot, JsonData obj, int count)
        {
            //if element
            foreach (XmlElement element in xmlRoot)
            {
                SortedList<string, object> childNodeValue = new SortedList<string, object>();
                SortedList<string, object> attributeValue = new SortedList<string, object>();

                switch (element)
                {
                    case XmlElement xmlNodeValue when element.ChildNodes.Count == 1: // Se for objeto final ou atributo
                        Console.WriteLine(xmlNodeValue.Name + " " + xmlNodeValue.InnerText);
                        if (xmlNodeValue.ChildNodes.Count != 0 && xmlNodeValue.GetType() != typeof(XmlText))
                            foreach (XmlAttribute attr in xmlNodeValue.Attributes)
                                Console.WriteLine(attr.Name + " " + attr.InnerText);
                        break;

                    case XmlElement xmlNodeValue when element.Attributes.Count > 0:
                        foreach (XmlAttribute attr in element.Attributes)
                            Console.WriteLine(attr.Name + " " + attr.InnerText);
                        if (xmlNodeValue.ChildNodes.Count != 0)
                            TreatXmlRootNode(xmlNodeValue, obj, count);
                        //StoreNodeValue(attributeValue, attr.Name, attr.InnerText);
                        break;

                    case XmlElement xmlNodeValue when element.ChildNodes.Count == 2: // Se for array
                        var objectData = new ObjectData();
                        if (obj.Noun != null)
                            objectData.Key = obj.Noun + "." + xmlNodeValue.Name;
                        if (obj.Key != null)
                            objectData.Key = obj.Key + "." + xmlNodeValue.Name;
                        TreatXmlRootNode(xmlNodeValue, objectData, count);
                        obj.Objects.Add(objectData);
                        break;

                    case XmlElement xmlNodeValue when element.ChildNodes.Count > 2: // Se for array
                        var arrayData = new ArrayData();
                        if (obj.Noun != null)
                            arrayData.Key = obj.Noun + "." + xmlNodeValue.Name;
                        if (obj.Key != null)
                            arrayData.Key = obj.Key + "." + xmlNodeValue.Name + "[" + count + "]";
                        TreatXmlRootNode(xmlNodeValue, arrayData, count++);
                        obj.Arrays.Add(arrayData);
                        break;
                }

                // SE FOR VALOR
                //if (element.ChildNodes.Count == 1)
                //{
                //    StoreNodeChildValue(childNodeValue, element.Name, element.InnerText);
                //}

                // SE FOR PROPRIEDADE
                //if (element.Attributes.Count > 0)
                //{
                //    foreach (XmlAttribute attr in element.Attributes)
                //    {
                //        StoreNodeChildValue(attributeValue, attr.Name, attr.InnerText);
                //    }
                //}

                //SE FOR ARRAY
                //if (element.ChildNodes.Count > 1 && element.FirstChild.HasChildNodes)
                //{
                //    Console.WriteLine(element.ChildNodes.Count + "\n");
                //    arrayData.Key = xmlDoc.SelectSingleNode("/*").Name + "." + element.Name;
                //    if (!isArray)
                //    {
                //        TreatXmlNode(arrayData, element, true);
                //        obj.Noun = arrayData.Key;
                //        obj.Arrays.Add(arrayData);
                //    }
                //    else
                //    {
                //        objDataForArrayObj.Key = xmlDoc.SelectSingleNode("/*").Name;
                //        TreatXmlRootNode(objDataForArrayObj, element, false);
                //    }
                //}
            }
        }

        public static void TreatXmlChildValue()
        {
            //StoreNodeValue(childNodeNamesValues, element.Name, element.InnerText);
            //if (element.Attributes.Count > 0)
            //    foreach (XmlAttribute attr in element.Attributes)
            //        StoreNodeValue(childNodeNamesValues, attr.Name, attr.InnerText);
        }

        private static void StoreNodeChildValue(SortedList<string, object> childNodeNames, string nodeName, object nodeValue)
        {
            // Add nodeValue to ArrayList associated with each nodeName
            // If nodeName doesn't exist then add it
            List<object> ValuesAL;

            if (childNodeNames.ContainsKey(nodeName))
            {
                ValuesAL = (List<object>)childNodeNames[nodeName];
            }
            else
            {
                ValuesAL = new List<object>();
                childNodeNames[nodeName] = ValuesAL;
            }
            ValuesAL.Add(nodeValue);
        }

        public static string NewXToJ(XmlDocument xmlDoc, Data data)
        {
            //Novo objectData atribuido ao documento xml
            var objectData = new ObjectData();
            TreatXmlNode(objectData, xmlDoc.DocumentElement, false);
            objectData.Noun = "TESTE_BOOK";
            data.Objects.Add(objectData);
            return "";
        }

        public static void TreatXmlNode (JsonData obj, XmlElement xmlDoc, bool isArray)
        {
            var objectData = new ObjectData();
            var objectArray = new ArrayData();
            var objDataForArrayObj = new ObjectData();
            objectData.Noun = xmlDoc.SelectSingleNode("/*").Name;
            SortedList<string, object> childNodeArarays = new SortedList<string, object>();
            SortedList<string, object> childNodeNamesValues = new SortedList<string, object>();

            // PARA CADA ELEMENTO
            foreach (XmlElement element in xmlDoc)
            {
                var arrayData = new ArrayData();
                // SE FOR FILHO UNICO
                if (element.ChildNodes.Count == 1)
                {
                    StoreNodeValue(childNodeNamesValues, element.Name, element.InnerText);
                    if (element.Attributes.Count > 0)
                        foreach (XmlAttribute attr in element.Attributes)
                            StoreNodeValue(childNodeNamesValues, attr.Name, attr.InnerText);
                }
                // SE TIVER + 1 FILHO NAO EH UM NO COM VALOR AINDA:
                if (element.ChildNodes.Count > 1 && element.FirstChild.HasChildNodes)
                {
                    Console.WriteLine(element.ChildNodes.Count + "\n");
                    arrayData.Key = xmlDoc.SelectSingleNode("/*").Name + "." +  element.Name;
                    if (!isArray)
                    {
                        TreatXmlNode(arrayData, element, true);
                        obj.Noun = arrayData.Key;
                        obj.Arrays.Add(arrayData);
                    }
                    else
                    {
                        objDataForArrayObj.Key = xmlDoc.SelectSingleNode("/*").Name;
                        TreatXmlNode(objDataForArrayObj, element, false);
                    }
                }
                //if (arrayData.Key != null) {
                //    objectArray.Noun = arrayData.Key;
                //    objectArray.Arrays.Add(arrayData);
                //}
            }
            objectData.Objects.Add(objDataForArrayObj);
            foreach (string childname in childNodeNamesValues.Keys)
            {
                var valueData1 = new ValueData();
                List<object> alChild = (List<object>)childNodeNamesValues[childname];
                OutputNodeAtt(childname, alChild[0], true, valueData1);
                objectData.Values.Add(valueData1);
            }
            obj.Objects.Add(objectData);
        }

        private static void StoreNodeValue(SortedList<string, object> childNodeNames, string nodeName, object nodeValue)
        {
            // Add nodeValue to ArrayList associated with each nodeName
            // If nodeName doesn't exist then add it
            List<object> ValuesAL;

            if (childNodeNames.ContainsKey(nodeName))
            {
                ValuesAL = (List<object>)childNodeNames[nodeName];
            }
            else
            {
                ValuesAL = new List<object>();
                childNodeNames[nodeName] = ValuesAL;
            }
            ValuesAL.Add(nodeValue);
        }

        private static void OutputNodeAtt(string childname, object alChild, bool showNodeName, JsonData obj)
        {
            if (alChild == null)
            {
                //if (showNodeName)
                //    sbJSON.Append("\"" + SafeJSON(childname) + "\": ");
                obj.Key = childname;
                obj.Value = "";
            }
            else if (alChild is string)
            {
                //if (showNodeName)
                //    sbJSON.Append("\"" + SafeJSON(childname) + "\": ");
                obj.Key = childname;
                obj.Value = (string)alChild;
            }
        }

        private static string TreatVaValue(JToken j)
        {
            switch (j)
            {
                case JValue _ when ((JValue)j).Value is DateTime jpTime:
                    return jpTime.ToString("O");
                case JProperty _ when (((JProperty)j).Value as JValue)?.Value is DateTime jvTime:
                    return jvTime.ToString("O");
                case JValue _:
                    return ((JValue)j).Value.ToString();
                default: return ((JProperty)j).Value.ToString();
            }

        }

        public static string XmlToJSON(XmlDocument xmlDoc, Data data)
        {
            StringBuilder sbJSON = new StringBuilder();
            var objectData = new ObjectData();
            var arrayData = new ArrayData();

            sbJSON.Append("{ ");
            XmlToJSONnode(sbJSON, xmlDoc.DocumentElement, true, objectData, data);
            sbJSON.Append("}");

            objectData.Noun = "TESTE_BOOK"; // NOME CONECTOR
            data.Objects.Add(objectData);

            return sbJSON.ToString();
        }

        //  XmlToJSONnode:  Output an XmlElement, possibly as part of a higher array
        private static void XmlToJSONnode(StringBuilder sbJSON, XmlElement node, bool showNodeName, JsonData obj,Data data)
        {
            if (showNodeName)
                sbJSON.Append("\"" + SafeJSON(node.Name) + "\": ");
            sbJSON.Append("{");
            // Build a sorted list of key-value pairs
            //  where   key is case-sensitive nodeName
            //          value is an ArrayList of string or XmlElement
            //  so that we know whether the nodeName is an array or not.
            SortedList<string, object> childNodeNames = new SortedList<string, object>();

            //  Add in all node attributes
            if (node.Attributes != null)
                foreach (XmlAttribute attr in node.Attributes)
                    StoreChildNode(childNodeNames, attr.Name, attr.InnerText);

            //  Add in all nodes
            foreach (XmlNode cnode in node.ChildNodes)
            {
                if (cnode is XmlText)
                    StoreChildNode(childNodeNames, "value", cnode.InnerText);
                else if (cnode is XmlElement)
                    StoreChildNode(childNodeNames, cnode.Name, cnode);
            }

            // Now output all stored info
            foreach (string childname in childNodeNames.Keys)
            {
                List<object> alChild = (List<object>)childNodeNames[childname];
                if (alChild.Count == 1)
                {
                    OutputNode(childname, alChild[0], sbJSON, true, obj, data);
                    //var valueData1 = new ValueData();
                    //valueData1.Key = childname;

                    //string testeS = "";

                    //if (alChild[0] is string)
                    //{
                    //    testeS = alChild[0].ToString();
                    //}

                    //valueData1.Value = testeS;
                    //obj.Values.Add(valueData1);
                }
                else
                {
                    var objectData = new ObjectData();
                    objectData.Key = childname;                   

                    sbJSON.Append(" \"" + SafeJSON(childname) + "\": [ "); //
                    int childNumber = 0;
                    foreach (object Child in alChild)
                    {
                        var arrayData = new ArrayData();
                        OutputNode(childname, Child, sbJSON, false, arrayData, data);
                        arrayData.Key = childname + "{" + childNumber + "}"; // ADICIONA NOME AO ARRAY DATA
                        //obj.Arrays.Add(arrayData);
                        objectData.Arrays.Add(arrayData);
                        childNumber++;
                    }
                    sbJSON.Remove(sbJSON.Length - 2, 2);
                    sbJSON.Append(" ], ");

                    obj.Objects.Add(objectData); // ADICIONAR O OBJECT DATA CONTENDO O ARRAYDATA AO OBJECTDATA RAIZ
                }
            }
            sbJSON.Remove(sbJSON.Length - 2, 2);
            sbJSON.Append(" }");
        }

        //  StoreChildNode: Store data associated with each nodeName
        //                  so that we know whether the nodeName is an array or not.
        private static void StoreChildNode(SortedList<string, object> childNodeNames, string nodeName, object nodeValue)
        {
            // Pre-process contraction of XmlElement-s
            if (nodeValue is XmlElement)
            {
                // Convert  <aa></aa> into "aa":null
                //          <aa>xx</aa> into "aa":"xx"
                XmlNode cnode = (XmlNode)nodeValue;
                if (cnode.Attributes.Count == 0)
                {
                    XmlNodeList children = cnode.ChildNodes;
                    if (children.Count == 0)
                        nodeValue = null;
                    else if (children.Count == 1 && (children[0] is XmlText))
                        nodeValue = ((XmlText)(children[0])).InnerText;
                }
            }
            // Add nodeValue to ArrayList associated with each nodeName
            // If nodeName doesn't exist then add it
            List<object> ValuesAL;

            if (childNodeNames.ContainsKey(nodeName))
            {
                ValuesAL = (List<object>)childNodeNames[nodeName];
            }
            else
            {
                ValuesAL = new List<object>();
                childNodeNames[nodeName] = ValuesAL;
            }
            ValuesAL.Add(nodeValue);
        }

        private static void OutputNode(string childname, object alChild, StringBuilder sbJSON, bool showNodeName, JsonData obj, Data data)
        {
            if (alChild == null)
            {
                if (showNodeName)
                    sbJSON.Append("\"" + SafeJSON(childname) + "\": ");
                sbJSON.Append("null");
            }
            else if (alChild is string)
            {
                if (showNodeName)
                    sbJSON.Append("\"" + SafeJSON(childname) + "\": ");
                string sChild = (string)alChild;
                sChild = sChild.Trim();
                sbJSON.Append("\"" + SafeJSON(sChild) + "\"");
            }
            else
                XmlToJSONnode(sbJSON, (XmlElement)alChild, showNodeName, obj,data);
            sbJSON.Append(", ");
        }

        // Make a string safe for JSON
        private static string SafeJSON(string sIn)
        {
            StringBuilder sbOut = new StringBuilder(sIn.Length);
            foreach (char ch in sIn)
            {
                if (Char.IsControl(ch) || ch == '\'')
                {
                    int ich = (int)ch;
                    sbOut.Append(@"\u" + ich.ToString("x4"));
                    continue;
                }
                else if (ch == '\"' || ch == '\\' || ch == '/')
                {
                    sbOut.Append('\\');
                }
                sbOut.Append(ch);
            }
            return sbOut.ToString();
        }

        private static void GetNodeInfo(XPathNavigator nav1)
        {
            Console.WriteLine("////////////////////////");
           Console.WriteLine("Name: " + nav1.Name.ToString());
            Console.WriteLine("Node Type: " + nav1.NodeType.ToString());
            //Console.WriteLine("Node Att: " + nav1.HasAttributes.ToString());
            Console.WriteLine("Node value: " + nav1.Value.ToString());

            // If node has children, move to first child.
            if (nav1.HasChildren)
            {
                nav1.MoveToFirstChild();
                while (nav1.MoveToNext())
                {
                    GetNodeInfo(nav1);
                    nav1.MoveToParent();
                }
            }

            else /* Else move to next sibling */
            {
                nav1.MoveToNext();
                GetNodeInfo(nav1);
            }
        }

        //-----
        public static string ToXmlString<T>(T data)
        {
            var serializer = new XmlSerializer(typeof(T));
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream, System.Text.Encoding.UTF8);
            serializer.Serialize(streamWriter, data);
            byte[] utf8EncodedXml = memoryStream.ToArray();
            string xml = Encoding.UTF8.GetString(utf8EncodedXml);
            int index = xml.IndexOf('<');
            if (index > 0) xml = xml.Substring(index, xml.Length - index);
            return xml;
        }

        private static void LoadData(JToken jsonData, Data data)
        {
            var j = JToken.Parse(jsonData.Root.ToString());

            var objectData = new ObjectData();
            var arrayData = new ArrayData();

            switch (j)
            {
                case JArray _:
                    TreatJsonConversion(arrayData, j);
                    arrayData.Noun = "TESTE_BOOK_ARAY";
                    data.Arrays.Add(arrayData);
                    break;
                case JObject _:
                    TreatJsonConversion(objectData, j);
                    objectData.Noun = "TESTE_BOOK_OBJ";
                    data.Objects.Add(objectData);
                    break;
            }
        }

        private static void TreatJsonConversion(JsonData obj, JToken json)
        {
            foreach (var j in json.Children())
            {
                switch (j)
                {
                    case JValue _:
                        var valueData1 = new ValueData();
                        valueData1.Value = TreatJsonValue(j);
                        obj.Values.Add(valueData1);
                        break;

                    case JProperty _:

                        if (((JProperty)j).Value is JValue)
                        {
                            var valueData = new ValueData();
                            valueData.Key = ((JProperty)j).Name;
                            valueData.Value = TreatJsonValue(j);
                            obj.Values.Add(valueData);
                        }
                        else
                        {
                            TreatJsonConversion(obj, j);
                        }
                        break;

                    case JArray _:
                        var arrayData = new ArrayData();
                        arrayData.Key = ((JArray)j).Path;
                        TreatJsonConversion(arrayData, j);
                        obj.Arrays.Add(arrayData);
                        break;

                    case JObject _:
                        var objectData = new ObjectData();
                        objectData.Key = ((JObject)j).Path;
                        TreatJsonConversion(objectData, j);
                        obj.Objects.Add(objectData);
                        break;
                }
            }
        }

        private static string TreatJsonValue(JToken j)
        {
            switch (j)
            {
                case JValue _ when ((JValue)j).Value is DateTime jpTime:
                    return jpTime.ToString("O");
                case JProperty _ when (((JProperty)j).Value as JValue)?.Value is DateTime jvTime:
                    return jvTime.ToString("O");
                case JValue _:
                    return ((JValue)j).Value.ToString();
                default: return ((JProperty)j).Value.ToString();
            }

        }

        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.Load("booksData.xml");

            //XmlNode root = doc.FirstChild;

            //Console.WriteLine(doc.DocumentElement.OuterXml);
            XPathNavigator nav = doc.CreateNavigator();

            //Move to the first child node (comment field).
            nav.MoveToFirstChild();
            Data data = new Data();

            //string xmltojson = XmlToJSON(doc, data);

            //Console.WriteLine("XML \n" + xmltojson + "\n");

            Data datajson = new Data();

            //LoadData(xmltojson, datajson);

            //Console.WriteLine("XML \n" + ToXmlString(datajson) + "\n");

            //Console.WriteLine(ToXmlString(data));

            //data = new Data();

            Console.WriteLine(NewCreation(doc,data));

            Console.WriteLine(ToXmlString(data));
            //do
            //{
            //    //Find the first element.
            //    if (nav.NodeType == XPathNodeType.Element)
            //    {
            //        //Determine whether children exist.
            //        if (nav.HasChildren == true)
            //        {

            //            //Move to the first child.
            //            nav.MoveToFirstChild();

            //            //Loop through all of the children.
            //            do
            //            {
            //                //Display the data.
            //                Console.Write("The XML string for this child ");
            //                Console.WriteLine("is '{0}'", nav.Value);

            //                //Check for attributes.
            //                if (nav.HasAttributes == true)
            //                {
            //                    Console.WriteLine("This node has attributes");
            //                }
            //            } while (nav.MoveToNext());
            //        }
            //        else
            //        {
            //            break;
            //        }
            //    }
            //} while (nav.MoveToNext());
            //Pause.

            //// move to root node
            //nav.MoveToRoot();
            //string name = nav.Name;
            //Console.WriteLine("Root node info: ");
            //Console.WriteLine("Base URI" + nav.BaseURI.ToString());
            //Console.WriteLine("Name: " + nav.NodeType.ToString());
            //Console.WriteLine("Node Type: " + nav.NodeType.ToString());
            ////Console.WriteLine("Node Value: " + nav.Value.ToString());

            //if (nav.HasChildren)
            //{
            //    nav.MoveToFirstChild();
            //    GetNodeInfo(nav);
            //}

            //foreach (XmlNode node in doc)
            //{
            //    if (node.HasChildNodes)
            //    {
            //        foreach (var child in node)
            //        {
            //            switch (child)
            //            {
            //                case XmlWhitespace _:
            //                    //Console.WriteLine(att + " SPACE");
            //                    break;

            //                case XmlNode n:
            //                    foreach (var son in n)
            //                    {

            //                    }
            //                    Console.WriteLine("ATT \n" + n.InnerText.Trim());
            //                    //Console.WriteLine("OUTERXML: \n" + n.OuterXml);
            //                    //Console.WriteLine("INNERXML: \n" + n.InnerXml);
            //                    Console.WriteLine("HAS CHILD: \n" + n.ChildNodes.Count);
            //                    break;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine(node.InnerText);
            //    }
            //}

            //Console.WriteLine(root.InnerText);

            //if (root.HasChildNodes)
            //{
            //    Console.WriteLine("ENTROU");
            //    for (int i = 0; i < root.ChildNodes.Count; i++)
            //    {
            //        Console.WriteLine(root.ChildNodes[i].InnerText + "\n");
            //    }
            //}

        }
    }
}
