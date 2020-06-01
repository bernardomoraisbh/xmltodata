using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace XmlManager
{
    class Program
    {
        /// <summary>
        /// Read nodes of <see cref="XmlDocument">xmlDoc</see> and if node is an element, it treats its attributes  <br />
        /// in <see cref="ProcessFullAttributes">ProcessFullAttributes</see> and its children nodes in the <see cref="ProcessFullNode">ProcessFullNode</see><br />
        /// converting the content to <see cref="Data">data</see> object type.<br />
        /// </summary>
        /// <param name="xmlDoc">Document to parse</param>
        /// <param name="data">Data to insert Document content</param>
        public static void LastTry(XmlDocument xmlDoc, Data data)
        {
            var rootData = new ObjectData();

            foreach (var rootNodes in xmlDoc)
            {
                var firstObject = new ObjectData();
                switch (rootNodes)
                {
                    case XmlDeclaration de:
                        firstObject.Key = "?" + de.Name;
                        var version = new ValueData();
                        version.Key = "@version";
                        version.Value = de.Version;
                        var encoding = new ValueData();
                        encoding.Key = "@encoding";
                        encoding.Value = de.Encoding;
                        firstObject.Values.Add(version);
                        firstObject.Values.Add(encoding);
                        break;

                    case XmlElement _:
                        firstObject.Key = xmlDoc.DocumentElement.Name;
                        //PROCESS ATTRIBUTES FROM ROOT NODES
                        ProcessFullAttributes(((XmlNode)rootNodes).Attributes, firstObject);
                        ProcessFullNode((XmlNode)rootNodes, firstObject, false, "");
                        break;
                }
                rootData.Objects.Add(firstObject);
            }            

            rootData.Noun = "TESTE_BOOK";
            data.Objects.Add(rootData);
        }

        /// <summary>
        /// Checks the <see cref="XmlNode">node</see> childrens and converts them all to the respective <see cref="Data">Data</see> object type, recursively.<br />
        /// <br />
        ///   <see cref="LastTry">ProcessFullAttributes</see> 
        /// </summary>
        /// <param name="node">Node to parse</param>
        /// <param name="obj">Object to add content of node</param>
        /// <param name="name">If true appends obj name</param>
        /// <param name="type">If called by array, append number</param>
        public static void ProcessFullNode(XmlNode node, JsonData obj, bool name, string type)
        {
            SortedList<string, object> currentNodes = new SortedList<string, object>();
            // ARMAZENAR NOS NA LISTA ORDENADA POR KEY (element)
            foreach (XmlNode n in node)
            {
                StoreChildNode(currentNodes, n.Name, n);
            }
            foreach (string key in currentNodes.Keys)
            {
                // PROCESSAR LISTA DO node ATUAL
                var objData = new ObjectData();
                var arrData = new ArrayData();
                var valData = new ValueData();
                List<object> alChild = (List<object>)currentNodes[key];
                if (alChild.Count == 1 && alChild[0] == null) // CASO TENHA UM ELEMENTO VAZIO
                {
                    valData.Value = "";
                    valData.Key = key;
                    obj.Values.Add(valData);
                }
                else if (alChild.Count == 1 && alChild[0] is string) // CASO TENHA UM ELEMENTO TEXTO SEM ATRIBUTO
                {
                    valData.Value = alChild[0].ToString();
                    valData.Key = key;
                    obj.Values.Add(valData);
                }
                else if (alChild.Count == 1 && alChild[0] is XmlElement) // CASO TENHA UM FILHO E SEJA TEXT OU XML ELEMENT
                {
                    XmlElement lonely_node = (XmlElement)alChild[0];
                    if (lonely_node.ChildNodes.Count == 1 && lonely_node.FirstChild is XmlText) // CASO TENHA UM FILHO E SEJA TEXT
                    {
                        // NEW OBJECT DATA NAME (KEY)
                        if (name)
                            objData.Key = type + "." + lonely_node.Name; 
                        else
                            objData.Key = lonely_node.ParentNode.Name + "." + lonely_node.Name;

                        // SAVE ATRIBUTES AS VALUEDATA
                        if (lonely_node.Attributes != null) 
                            ProcessFullAttributes(lonely_node.Attributes, objData);

                        //ADD ELEMENT VALUE TO valData
                        valData.Value = lonely_node.FirstChild.Value;
                        valData.Key = lonely_node.FirstChild.Name;
                        //ADD valData to objData and objData to obj
                        objData.Values.Add(valData);
                        obj.Objects.Add(objData);
                    }
                    else if (lonely_node.ChildNodes.Count >= 1 && lonely_node.FirstChild is XmlNode) // CASO TENHA UM FILHO E SEJA XML NODE/ELEMENT
                    {
                        // NEW OBJECT DATA NAME (KEY)
                        if (obj.Key != null)
                            objData.Key = obj.Key + "." + lonely_node.Name;
                        else
                            objData.Key = lonely_node.ParentNode.Name + "." + lonely_node.Name;

                        // SAVE ATRIBUTES AS VALUEDATA
                        if (lonely_node.Attributes != null) 
                            ProcessFullAttributes(lonely_node.Attributes, objData);

                        // SEND THE LONELY NODE AND DO STUF
                        ProcessFullNode(lonely_node, objData, false,"");
                        obj.Objects.Add(objData);
                    }
                }
                else if (alChild.Count >= 1) // Se array
                {
                    if (obj.Key != null)
                        arrData.Key = obj.Key + "." + key; // first array name
                    foreach (object child in alChild)
                    {
                        if (child is string)
                        {
                            var attData = new ValueData();
                            attData.Value = child.ToString();
                            arrData.Values.Add(attData);
                        }
                        else
                        {
                            var objDataForArray = new ObjectData();

                            // SAVE ATRIBUTES AS VALUEDATA
                            if (((XmlElement)child).Attributes != null)
                                ProcessFullAttributes(((XmlElement)child).Attributes,objDataForArray);

                            //SET OBJECT NAME (KEY) FOR NEW ARRAY
                            if (obj.Key != null)
                                objDataForArray.Key = obj.Key + "." + ((XmlElement)child).Name + "[" + alChild.IndexOf(child) + "]";
                            else
                                objDataForArray.Key = ((XmlElement)child).ParentNode.Name + "." + ((XmlElement)child).Name + "[" + alChild.IndexOf(child) + "]";

                            ProcessFullNode((XmlElement)child, objDataForArray, true, objDataForArray.Key);
                            arrData.Objects.Add(objDataForArray);
                        }
                    }
                    //APPEND ARRAY ELEMENT
                    obj.Arrays.Add(arrData);
                }
            }
        }

        /// <summary>
        /// Adds every attribute in <see cref="XmlAttributeCollection">attributes</see> to the respective <see cref="JsonData">object</see>.<br />
        /// <br />
        ///   <see cref="LastTry">ProcessRoot</see> 
        ///   <see cref="ProcessFullNode">ProcessFullNode</see> 
        /// </summary>
        /// <param name="attributes">XmlAttributeCollection to parse</param>
        /// <param name="objData">Object to add the attributes</param>
        public static void ProcessFullAttributes(XmlAttributeCollection attributes, JsonData objData)
        {
            foreach (XmlAttribute attr in attributes)
            {
                var attData = new ValueData();
                attData.Value = attr.InnerText;
                attData.Key = "@" + attr.Name;
                objData.Values.Add(attData);
            }
        }

        /// <summary>
        /// Store data associated with each <see cref="string">nodeName</see> in the <see cref="SortedList">SortedList</see> <br />
        /// so that we know whether the <see cref="string">nodeName</see> is an array or not.<br />
        /// <br />
        ///   <see cref="ProcessFullNode">ProcessFullNode</see> 
        /// </summary>
        /// <param name="childNodeNames">List of objects sorted by string (key)</param>
        /// <param name="nodeName">Name of the node to add to the list</param>
        /// <param name="nodeValue">node content</param>
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

        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.Load("booksData.xml");

            Data data = new Data();

            LastTry(doc, data);

            File.WriteAllText("output.xml", ToXmlString(data));
            Console.WriteLine("XML OK\n");
        }
    }
}
