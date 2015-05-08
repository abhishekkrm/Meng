using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlGenerator
{
    class XMLWriter
    {
        private const int LINE_SEGMENT_LENGTH = 10;
        private const int GENERATOR_IMG_HEIGHT = 72;

        private Dictionary<int, BusLocationInfo>    mBusLocations = null;
        private PowerSystem                         mPowerSystem = null;
        private XmlDocument                         mXMLDocument = null;
        private int                                 mNumLines = 0;
        private int                                 mNumGenerators = 0;

        public XMLWriter(String inIEEEModelFile, String inBusLocationFile)
        {
            mBusLocations = new Dictionary<int, BusLocationInfo>();
            mXMLDocument = new XmlDocument();
            
            /* Read Power System Information from IEEE Model File */
            mPowerSystem = new Parser().CreatePowerSystemFromFile(inIEEEModelFile);

            /* Read initial bus locations */
            ParseBusLocationFile(inBusLocationFile);
        }

        private void ParseBusLocationFile(String inBusLocationFile)
        {
            mXMLDocument.Load(inBusLocationFile);
            
            XmlNodeList components = mXMLDocument.GetElementsByTagName("component");

            foreach(XmlNode component in components)
            {
                 if (Convert.ToString(component.Attributes[1].Value).Equals("Bus"))
                 {
                     BusLocationInfo busLocationInfo = new BusLocationInfo(Convert.ToInt32(component.Attributes[0].Value));

                     busLocationInfo.StartX = Convert.ToInt32(component.ChildNodes[0].Attributes[0].InnerText);
                     busLocationInfo.StartY = Convert.ToInt32(component.ChildNodes[1].Attributes[0].InnerText);
                     busLocationInfo.EndX = Convert.ToInt32(component.ChildNodes[2].Attributes[0].InnerText);
                     busLocationInfo.EndY = Convert.ToInt32(component.ChildNodes[3].Attributes[0].InnerText);

                     mBusLocations.Add(busLocationInfo.BusNumber, busLocationInfo);
                 }
            }
        }

        private void UpdateBusEntries()
        {
            XmlNodeList components = mXMLDocument.GetElementsByTagName("component");

            foreach(XmlNode component in components)
            {
                 if (Convert.ToString(component.Attributes[1].Value).Equals("Bus"))
                 {
                     int busNumber = Convert.ToInt32(component.Attributes[0].Value);

                     XmlElement busType = mXMLDocument.CreateElement("type");

                     if(mPowerSystem.GetBus(busNumber).IsConsumer) 
                     {
                         busType.SetAttribute("value", "line-arrow");
                     }
                     else
                     {
                         busType.SetAttribute("value", "line");
                     }
                     
                     component.AppendChild(busType);
                 }
            }
        }

        private void WriteLineEntry(int x1, int y1, int x2, int y2, String connectionId)
        {
            XmlNode root = mXMLDocument.DocumentElement;

            XmlElement line = mXMLDocument.CreateElement("component");
            mNumLines++;

            line.SetAttribute("id", Convert.ToString(mNumLines));
            line.SetAttribute("type", "Line");

            XmlElement start_x = mXMLDocument.CreateElement("x1");
            start_x.SetAttribute("value", Convert.ToString(x1));

            XmlElement start_y = mXMLDocument.CreateElement("y1");
            start_y.SetAttribute("value", Convert.ToString(y1));

            XmlElement end_x = mXMLDocument.CreateElement("x2");
            end_x.SetAttribute("value", Convert.ToString(x2));

            XmlElement end_y = mXMLDocument.CreateElement("y2");
            end_y.SetAttribute("value", Convert.ToString(y2));

            XmlElement conn = mXMLDocument.CreateElement("connection");
            conn.SetAttribute("value", Convert.ToString(connectionId));
            
            line.AppendChild(start_x);
            line.AppendChild(start_y);
            line.AppendChild(end_x);
            line.AppendChild(end_y);
            line.AppendChild(conn);

            root.AppendChild(line);
        }

        private void WriteConnection(BusLocationInfo source, BusLocationInfo destination)
        {
            int lengthBus = source.EndX - source.StartX;
            int srcConnectionCount = mPowerSystem.GetBus(source.BusNumber).GetConnections().Count;

            source.AssignedConnections++;
            int x1 = source.StartX + source.AssignedConnections * (lengthBus / (srcConnectionCount + 1));
            int y1 = source.StartY;

            destination.AssignedConnections++;
            lengthBus = destination.EndX - destination.StartX;
            int destConnectionCount = mPowerSystem.GetBus(destination.BusNumber).GetConnections().Count;

            int x2 = destination.StartX + destination.AssignedConnections * (lengthBus / (destConnectionCount + 1));
            int y2 = destination.StartY;

            String connectionId = source.BusNumber + "," + destination.BusNumber;
            if (y1 < y2)
            {
                WriteLineEntry(x1, y1, x1, y1 + (srcConnectionCount - source.AssignedConnections + 1) * LINE_SEGMENT_LENGTH, connectionId);
                WriteLineEntry(x1, y1 + (srcConnectionCount - source.AssignedConnections + 1) * LINE_SEGMENT_LENGTH, x2, y1 + (srcConnectionCount - source.AssignedConnections + 1) * LINE_SEGMENT_LENGTH, connectionId);
                if (y2 < (y1 + (srcConnectionCount - source.AssignedConnections + 1) * LINE_SEGMENT_LENGTH))
                    WriteLineEntry(x2, y2, x2, y1 + (srcConnectionCount - source.AssignedConnections + 1) * LINE_SEGMENT_LENGTH, connectionId);
                else
                    WriteLineEntry(x2, y1 + (srcConnectionCount - source.AssignedConnections + 1) * LINE_SEGMENT_LENGTH, x2, y2, connectionId);
              
                source.DownSideConnections++;
                destination.UpSideConnections++;
            }
            else
            {
                WriteLineEntry(x1, y1 - (srcConnectionCount - source.AssignedConnections + 1) * LINE_SEGMENT_LENGTH, x1, y1, connectionId);
                WriteLineEntry(x1, y1 - (srcConnectionCount - source.AssignedConnections + 1) * LINE_SEGMENT_LENGTH, x2, y1 - (srcConnectionCount - source.AssignedConnections + 1) * LINE_SEGMENT_LENGTH, connectionId);
                if (y2 < y1 - (srcConnectionCount - source.AssignedConnections + 1) * LINE_SEGMENT_LENGTH)
                    WriteLineEntry(x2, y2, x2, y1 - (srcConnectionCount - source.AssignedConnections + 1) * LINE_SEGMENT_LENGTH, connectionId);
                else
                    WriteLineEntry(x2, y1 - (srcConnectionCount - source.AssignedConnections + 1) * LINE_SEGMENT_LENGTH, x2, y2, connectionId);
                
                source.UpSideConnections++;
                destination.DownSideConnections++;
            }
        }

        private void WriteConnections()
        {
            // Find the positions of the line 
            var items = from pair in mBusLocations
                        orderby pair.Value.StartX ascending
                        select pair;

            foreach (KeyValuePair<int, BusLocationInfo> busLocPair in items)
            {
                Dictionary<int, int> xCoordinatesOfConnectedBuses = new Dictionary<int, int>();
                List<int> connectedBuses = mPowerSystem.GetBus(busLocPair.Key).GetConnections();

                if (connectedBuses != null && connectedBuses.Count > 0)
                {
                    foreach (int connectedBus in connectedBuses)
                    {
                        if (mBusLocations[busLocPair.Key].StartX < mBusLocations[connectedBus].StartX)
                        {
                            xCoordinatesOfConnectedBuses.Add(connectedBus, mBusLocations[connectedBus].StartX);
                        }
                    }

                    var connectedBusesSorted = from pair in xCoordinatesOfConnectedBuses
                                               orderby pair.Value ascending
                                               select pair;

                    foreach (KeyValuePair<int, int> connectedBusSorted in connectedBusesSorted)
                    {
                        WriteConnection(mBusLocations[busLocPair.Key], mBusLocations[connectedBusSorted.Key]);
                    }
                }
            }
        }

        private void WriteGeneratorEntry(int busNumber)
        {
            XmlNode root = mXMLDocument.DocumentElement;

            XmlElement generator = mXMLDocument.CreateElement("component");
            mNumGenerators++;
            generator.SetAttribute("id", Convert.ToString(mNumGenerators));
            generator.SetAttribute("type", "Generator");

            int orientationValue = 0;
            int xCoordinate = mBusLocations[busNumber].StartX;
            int yCoordinate = mBusLocations[busNumber].StartY - GENERATOR_IMG_HEIGHT;
            if(mBusLocations[busNumber].UpSideConnections > mBusLocations[busNumber].DownSideConnections)
            {
                orientationValue = 180;
                yCoordinate = mBusLocations[busNumber].StartY;
            }

            XmlElement start_x = mXMLDocument.CreateElement("x");
            start_x.SetAttribute("value", Convert.ToString(xCoordinate));

            XmlElement start_y = mXMLDocument.CreateElement("y");
            start_y.SetAttribute("value", Convert.ToString(yCoordinate));

            XmlElement orientation = mXMLDocument.CreateElement("orientation");
            orientation.SetAttribute("value", Convert.ToString(orientationValue));

            generator.AppendChild(start_x);
            generator.AppendChild(start_y);
            generator.AppendChild(orientation);

            root.AppendChild(generator);
        }

        private void WriteGenerators()
        {
            List<int> generators = new List<int>();
            XmlNodeList components = mXMLDocument.GetElementsByTagName("component");

            foreach (XmlNode component in components)
            {
                if (Convert.ToString(component.Attributes[1].Value).Equals("Bus"))
                {
                    int busNumber = Convert.ToInt32(component.Attributes[0].Value);

                    if(mPowerSystem.GetBus(busNumber).IsGenerator)
                    {
                        generators.Add(busNumber);
                    }
                }
            }

            foreach(int generator in generators)
            {
                WriteGeneratorEntry(generator);
            }
        }

        private void WriteComponentLocationsFile(String inComponentLocationsFile)
        {
            WriteConnections();
            UpdateBusEntries();
            WriteGenerators();
            mXMLDocument.Save(inComponentLocationsFile);
        }

        public void GenerateFiles(String inBusDetailsFile, String inBusConnectionsFile, String inComponentLocationsFile)
        {
            mPowerSystem.WriteBusDetailsFile(inBusDetailsFile);
            mPowerSystem.WriteBusConnectionsFile(inBusConnectionsFile);

            WriteComponentLocationsFile(inComponentLocationsFile);
        }
    }
}
