using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlGenerator
{
    class Bus
    {
        private int mBusNumber, mAreaNumber, mBusType;
        private double mBaseKiloVoltage;
        private String mBusName;
        private bool mIsGenerator, mIsConsumer;
        private List<int> mConnectedBuses = null;

        /* Properties */
        public bool IsConsumer
        {
            get { return mIsConsumer; }
            set { mIsConsumer = value; }
        }

        public bool IsGenerator
        {
            get { return mIsGenerator; }
            set { mIsGenerator = value; }
        }
        
        public int BusType
        {
            get { return mBusType; }
            set { mBusType = value; }
        }

        public int AreaNumber
        {
            get { return mAreaNumber; }
            set { mAreaNumber = value; }
        }

        public int BusNumber
        {
            get { return mBusNumber; }
            set { mBusNumber = value; }
        }
        

        public double BaseKiloVoltage
        {
            get { return mBaseKiloVoltage; }
            set { mBaseKiloVoltage = value; }
        }
        
        public String Name
        {
            get { return mBusName; }
            set { mBusName = value; }
        }
        
        /* Ctor */
        public Bus()
        {
            mConnectedBuses = new List<int>();
        }

        public void AddConnection(int inConnectedBus) 
        {
            mConnectedBuses.Add(inConnectedBus);
        }

        public List<int> GetConnections()
        {
            return mConnectedBuses;
        }
    }
}
