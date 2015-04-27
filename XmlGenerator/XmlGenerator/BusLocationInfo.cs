using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlGenerator
{
    class BusLocationInfo
    {
        private int mBusNumber;
        private int mStartX, mStartY, mEndX, mEndY;
        private int mAssignedConnections;
        private int mUpSideConnections, mDownSideConnections;

        /* Properties */
        public int DownSideConnections
        {
            get { return mDownSideConnections; }
            set { mDownSideConnections = value; }
        }

        public int UpSideConnections
        {
            get { return mUpSideConnections; }
            set { mUpSideConnections = value; }
        }

        public int BusNumber
        {
            get { return mBusNumber; }
            set { mBusNumber = value; }
        }

        public int EndY
        {
            get { return mEndY; }
            set { mEndY = value; }
        }

        public int EndX
        {
            get { return mEndX; }
            set { mEndX = value; }
        }

        public int StartY
        {
            get { return mStartY; }
            set { mStartY = value; }
        }

        public int StartX
        {
            get { return mStartX; }
            set { mStartX = value; }
        }

        public int AssignedConnections
        {
            get { return mAssignedConnections; }
            set { mAssignedConnections = value; }
        }

        /* Ctor */
        public BusLocationInfo( int inBusNumber )
        {
            mBusNumber = inBusNumber;
            mAssignedConnections = 0;
            mDownSideConnections = mUpSideConnections = 0;
        }
    }
}
