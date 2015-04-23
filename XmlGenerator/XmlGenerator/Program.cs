using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlGenerator
{
    class BusDetail
    {
        public int id;
        public int start_x, start_y;
        public int end_x, end_y;
        public bool isArrow;
        public List<int> busMapping = null;
        public int assigned_connection;
    }

    class Program
    {
        static Dictionary<int, BusDetail> busInfo = new Dictionary<int,BusDetail>();
        static Dictionary<int, int> busPosition = new Dictionary<int,int>();
        static int numLines = 0;
        public static int LINE = 10;

        static void writeEntry(XmlDocument doc, int x1, int y1, int x2, int y2, String connectionId)
        {
            XmlNode root = doc.DocumentElement;

            XmlElement line = doc.CreateElement("component");
            numLines++;
            line.SetAttribute("id", Convert.ToString(numLines));
            line.SetAttribute("type", "Line");
            XmlElement start_x = doc.CreateElement("x1");
            start_x.SetAttribute("value", Convert.ToString(x1));
            XmlElement start_y = doc.CreateElement("y1");
            start_y.SetAttribute("value", Convert.ToString(y1));
            XmlElement end_x = doc.CreateElement("x2");
            end_x.SetAttribute("value", Convert.ToString(x2));
            XmlElement end_y = doc.CreateElement("y2");
            end_y.SetAttribute("value", Convert.ToString(y2));
            XmlElement conn = doc.CreateElement("connection");
            conn.SetAttribute("value", Convert.ToString(connectionId));
            line.AppendChild(start_x);
            line.AppendChild(start_y);
            line.AppendChild(end_x);
            line.AppendChild(end_y);
            line.AppendChild(conn);

            root.AppendChild(line);
        }

        static void writeToXML(XmlDocument doc, BusDetail source, BusDetail destination)
        {
            int lengthBus, numConnection;

            lengthBus = source.end_x - source.start_x;
            numConnection =source.busMapping.Count;
            source.assigned_connection++;
            int x1 = source.start_x + source.assigned_connection * (lengthBus / (numConnection + 1));
            int y1 = source.start_y;

            lengthBus = destination.end_x - destination.start_x;
            numConnection = destination.busMapping.Count;
            destination.assigned_connection++;
            int x2 = destination.start_x + destination.assigned_connection * (lengthBus / (numConnection + 1));
            int y2 = destination.start_y;

            String connectionId = source.id + "," + destination.id;
            if (y1 < y2)
            {
                writeEntry(doc, x1, y1, x1, y1 + (source.busMapping.Count - source.assigned_connection + 1) * LINE, connectionId);
                if (x1 < x2)
                    writeEntry(doc, x1, y1 + (source.busMapping.Count - source.assigned_connection + 1) * LINE, x2, y1 + (source.busMapping.Count - source.assigned_connection + 1) * LINE, connectionId);
                else
                    writeEntry(doc, x2, y1 + (source.busMapping.Count - source.assigned_connection + 1) * LINE, x1, y1 + (source.busMapping.Count - source.assigned_connection + 1) * LINE, connectionId);
                writeEntry(doc, x2, y1 + (source.busMapping.Count - source.assigned_connection + 1) * LINE, x2, y2, connectionId);
            }
            else
            {
                writeEntry(doc, x1, y1 - (source.busMapping.Count - source.assigned_connection + 1) * LINE, x1, y1, connectionId);
                if (x1 < x2)
                    writeEntry(doc, x1, y1 - (source.busMapping.Count - source.assigned_connection + 1) * LINE, x2, y1 - (source.busMapping.Count - source.assigned_connection + 1) * LINE, connectionId);
                else
                    writeEntry(doc, x2, y1 - (source.busMapping.Count - source.assigned_connection + 1) * LINE, x1, y1 - (source.busMapping.Count - source.assigned_connection + 1) * LINE, connectionId); 
                writeEntry(doc, x2, y2, x2, y1 - (source.busMapping.Count - source.assigned_connection + 1) * LINE, connectionId);
            }
        }

        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Component_Input.xml");
            
            XmlNodeList components = doc.GetElementsByTagName("component");

            // Populate bus Info
            for (int i = 0; i < components.Count; i++)
            {
                if (Convert.ToString(components[i].Attributes[1].Value).Equals("Bus"))
                {
                    //adding a bus
                    BusDetail bus = new BusDetail();
                    bus.id = Convert.ToInt32(components[i].Attributes[0].Value);
                    bus.start_x = Convert.ToInt32(components[i].ChildNodes[0].Attributes[0].InnerText);
                    bus.start_y = Convert.ToInt32(components[i].ChildNodes[1].Attributes[0].InnerText);
                    bus.end_x = Convert.ToInt32(components[i].ChildNodes[2].Attributes[0].InnerText);
                    bus.end_y = Convert.ToInt32(components[i].ChildNodes[3].Attributes[0].InnerText);
                    bus.isArrow = Convert.ToString(components[i].ChildNodes[4].Attributes[0].InnerText).Equals("line-arrow");
                    bus.assigned_connection = 0;
                    if (components[i].ChildNodes.Count > 5)
                    {
                        String connections = Convert.ToString(components[i].ChildNodes[5].Attributes[0].InnerText);
                        String[] connectionList = connections.Split(',');
                        foreach (String busId in connectionList)
                        {
                            if (bus.busMapping == null)
                                bus.busMapping = new List<int>();

                            int id = Convert.ToInt32(busId);
                            bus.busMapping.Add(id);
                        }
                    }

                    busInfo.Add(bus.id, bus);
                    busPosition.Add(bus.id, bus.start_x);
                }
            }
            
            // Find the positions of the line 
            var items = from pair in busPosition
                        orderby pair.Value ascending
                        select pair;

            foreach (KeyValuePair<int, int> bus in items)
            {
                Dictionary<int, int> xCoordinates = new Dictionary<int, int>();
                if (busInfo[bus.Key].busMapping != null)
                {
                    foreach (int connection in busInfo[bus.Key].busMapping)
                    {
                        if (busInfo[bus.Key].start_x < busInfo[connection].start_x)
                            xCoordinates.Add(connection, busInfo[connection].start_x);
                    }

                    var adjacent = from pair in xCoordinates
                                   orderby pair.Value ascending
                                   select pair;

                    foreach (KeyValuePair<int, int> connection in adjacent)
                    {
                        writeToXML(doc, busInfo[bus.Key], busInfo[connection.Key]);
                    }
                }
            }



            // Write information of lines connecting buses
            /*
            */
            doc.Save("Component.xml");
        }
    }
}
