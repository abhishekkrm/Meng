using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace XmlGenerator
{
    class Parser
    {
        private const String BUS_SECTION_START_CARD = "BUS DATA FOLLOWS";
        private const String BRANCH_SECTION_START_CARD = "BRANCH DATA FOLLOWS";
        private const String SECTION_END_CARD = "-999";

        private void ReadSectionStartCard(System.IO.StreamReader inFileReader, String inSectionStartText)
        {
            String line;
            while((line = inFileReader.ReadLine()) != null)
            {
                if(line.StartsWith(inSectionStartText) == true)
                {
                    break;
                }
            }
        }

        private void ReadBusInfoSection(System.IO.StreamReader inFileReader, PowerSystem inPowerSystem)
        {
            ReadSectionStartCard(inFileReader, BUS_SECTION_START_CARD);
            String line = null;

            while((line = inFileReader.ReadLine())!= null && line.Trim().Equals(SECTION_END_CARD)==false)
            {
                /* Create a new Bus */
                Bus bus = new Bus();

                bus.BusNumber = Convert.ToInt32(line.Substring(0, 4).Trim());
                bus.Name = line.Substring(5, 10).Trim();

                String [] values = line.Substring(18).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                bus.AreaNumber = Convert.ToInt32(values[0]);
                bus.BusType = Convert.ToInt32(values[2]);
                bus.BaseKiloVoltage = Convert.ToDouble(values[9]);
                bus.IsGenerator = (Convert.ToDouble(values[7]) != 0.0);
                bus.IsConsumer = (Convert.ToDouble(values[5]) != 0.0);

                /* Add the created bus to power system */
                inPowerSystem.AddBus(bus);
            }
        }

        private void ReadBusConnectionSection(System.IO.StreamReader inFileReader, PowerSystem inPowerSystem)
        {
            ReadSectionStartCard(inFileReader, BRANCH_SECTION_START_CARD);
             String line = null;

             while ((line = inFileReader.ReadLine()) != null && line.Equals(SECTION_END_CARD) == false)
             {
                 string[] values = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                 int fromBusNumber = Convert.ToInt32(values[0]);
                 int toBusNumber = Convert.ToInt32(values[1]);

                 Bus fromBus = inPowerSystem.GetBus(fromBusNumber);
                 Bus toBus = inPowerSystem.GetBus(toBusNumber);

                 Debug.Assert( fromBus != null && toBus != null, "Wrong bus number in file!");

                 fromBus.AddConnection(toBusNumber);
                 toBus.AddConnection(fromBusNumber);
             }
        }

        public PowerSystem CreatePowerSystemFromFile(String inIEEEModelFile)
        {
            PowerSystem powerSystem = new PowerSystem();
            
            using(System.IO.StreamReader fileReader = new System.IO.StreamReader(inIEEEModelFile))
            {
                ReadBusInfoSection(fileReader, powerSystem);
                ReadBusConnectionSection(fileReader, powerSystem);
            }
            
            return powerSystem;
        }
    }
}
