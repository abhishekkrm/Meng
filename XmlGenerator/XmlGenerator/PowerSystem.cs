using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XmlGenerator
{
    class PowerSystem
    {
        Dictionary<int, Bus> mBusList = null;

        public PowerSystem()
        {
            mBusList = new Dictionary<int, Bus>();
        }

        public void AddBus(Bus inBus)
        {
            if(inBus != null)
            {
                mBusList[inBus.BusNumber] = inBus;
            }
        }

        public Bus GetBus(int inBusNumber)
        {
            if(mBusList.ContainsKey(inBusNumber))
            {
                return mBusList[inBusNumber];
            }
            return null;
        }

        public void WriteBusDetailsFile(String inFile)
        {
            using (StreamWriter writer = new StreamWriter(inFile))
            {
                foreach (var busId in mBusList.Keys)
                {
                    writer.WriteLine(mBusList[busId].BusNumber + "," + mBusList[busId].Name + "," + mBusList[busId].BaseKiloVoltage + "," + mBusList[busId].BusType + "," + mBusList[busId].AreaNumber);
                }
            }
        }

        public void WriteBusConnectionsFile(String inFile)
        {
            using (StreamWriter writer = new StreamWriter(inFile))
            {
                foreach (var busId in mBusList.Keys)
                {
                    List<int> connectedBuses = mBusList[busId].GetConnections();
                    if (connectedBuses != null && connectedBuses.Count > 0)
                    {
                        writer.WriteLine(mBusList[busId].BusNumber + ":" + String.Join(",", connectedBuses));
                    }
                }
            }
        }
    }
}
