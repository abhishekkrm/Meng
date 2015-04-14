using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DataReader
{
    class BusDetails
    {
        public int busNo, areaNumber, busType;
        public double busBaseKiloVoltage;
        public String busName;
        public List<int> bus_mapping = null;
    }

    class Program
    {
        public const string SECTION_END_MARKER = "-999";
        public const string FILENAME_BUSDETAILS = "busdetails.txt";
        public const string FILENAME_BUSMAP = "bus_map.txt";
        
        static void Main(string[] args)
        {
            string line;
            bool sectionStartFound = false;
            Dictionary<int , BusDetails> busInfo = new Dictionary<int, BusDetails>();

            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Amarinder\Desktop\ieee14cdf.txt");
            while ((line = file.ReadLine()) != null)
            {
                string[] values = line.Split(' ');

                if (sectionStartFound == false)
                {
                    if (values.Length >= 3 && values[0].Equals("BUS") && values[1].Equals("DATA") && values[2].Equals("FOLLOWS"))
                    {
                        sectionStartFound = true;
                        continue;
                    }
                    else
                        continue;
                }

                if (values[0].Equals(SECTION_END_MARKER))
                    break;

                string bus = line.Substring(0, 4);
                int bus_id = Convert.ToInt32(bus.Trim());
                BusDetails busData = new BusDetails();

                busData.busNo = bus_id;

                string name = line.Substring(5, 10);
                busData.busName = name.Trim();

                string remLine = line.Substring(18);
                values = remLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                //Console.WriteLine("id " + busData.busNo + "name " + busData.busName + "remLine " + remLine);
                busData.areaNumber = Convert.ToInt32(values[0]);
                busData.busType = Convert.ToInt32(values[2]);
                busData.busBaseKiloVoltage = Convert.ToDouble(values[9]);

                busInfo.Add(busData.busNo, busData);
                //numBuses++;
            }

            sectionStartFound = false;
            while ((line = file.ReadLine()) != null)
            {
                string[] values = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (sectionStartFound == false)
                {
                    if (values.Length >= 3 && values[0].Equals("BRANCH") && values[1].Equals("DATA") && values[2].Equals("FOLLOWS"))
                    {
                        sectionStartFound = true;
                        continue;
                    }
                    else
                        continue;
                }

                if (values[0].Equals(SECTION_END_MARKER))
                    break;

                //Console.WriteLine(values[0] + " " + values[1]);
                int tapBusno = Convert.ToInt32(values[0]);
                int zBusno = Convert.ToInt32(values[1]);

                if (busInfo[tapBusno].bus_mapping == null)
                    busInfo[tapBusno].bus_mapping = new List<int>();

                busInfo[tapBusno].bus_mapping.Add(zBusno);

                if (busInfo[zBusno].bus_mapping == null)
                    busInfo[zBusno].bus_mapping = new List<int>();

                busInfo[zBusno].bus_mapping.Add(tapBusno);
            }

            using (StreamWriter writer = new StreamWriter(FILENAME_BUSDETAILS))
            {
                foreach (var busId in busInfo.Keys)
                {
                    writer.WriteLine(busInfo[busId].busNo + "," + busInfo[busId].busName + "," + busInfo[busId].busBaseKiloVoltage + "," + busInfo[busId].busType + "," + busInfo[busId].areaNumber);
                }
            }

            Console.WriteLine("Created file " + FILENAME_BUSDETAILS); 
            
            using (StreamWriter writer = new StreamWriter(FILENAME_BUSMAP))
            {
                foreach (var busId in busInfo.Keys)
                {
                    if (busInfo[busId].bus_mapping != null)
                    {
                        writer.Write(busInfo[busId].busNo + ":");
                    
                        int j = 0;
                        foreach (int num in busInfo[busId].bus_mapping)
                        {
                            writer.Write(num);
                            j++;
                            if (j != busInfo[busId].bus_mapping.Count)
                                writer.Write(",");
                            else
                                writer.WriteLine("");
                        }
                    }
                }
            }

            Console.WriteLine("Created file " + FILENAME_BUSMAP); 
            
            file.Close();
        }
    }
}
