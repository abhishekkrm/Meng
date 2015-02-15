/*

Copyright (c) 2004-2009 Petko Nikolov. All rights reserved.

Redistribution and use in source and binary forms,
with or without modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above
   copyright notice, this list of conditions and the following
   disclaimer in the documentation and/or other materials provided
   with the distribution.

THIS SOFTWARE IS PROVIDED "AS IS" BY THE ABOVE COPYRIGHT HOLDER(S)
AND ALL OTHER CONTRIBUTORS AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE ABOVE COPYRIGHT HOLDER(S) OR ANY OTHER
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
SUCH DAMAGE.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapLibrary
{
    public class ObjectTree
    {
        private TreeNode root;

        // the max number of objects that can be present in a node before it must be split
        private int threshold;

        // a dictionary to allow direct access to the node where an object currently is
        private Dictionary<String, TreeNode> objectToNode;

        public ObjectTree(int threshold)
        {
            this.threshold = threshold;
            this.objectToNode = new Dictionary<string, TreeNode>();
            this.root = new TreeNode(new Location(180f, -180f, 0f),
                                     new Location(180f, 180f, 0f),
                                     new Location(-180f, -180f, 0f),
                                     new Location(-180f, 180f, 0f),
                                     null);
        }

        public void Insert(ObjectInfo objInfo)
        {
            // check if the object is already in the tree
            TreeNode n = null;
            lock (objectToNode)
            {
                if (objectToNode.ContainsKey(objInfo.key))
                {
                    n = objectToNode[objInfo.key];
                }
            }

            // if it was not already in there, find the correct node and insert it
            if (n == null)
            {
                InsertNew(objInfo, root);
            }

            // otherwise, use fact that new location may be near to old one to find appropriate node
            // TO BE IMPLEMENTED, for now use the InsertNew code and clean up old node
            else
            {
                bool wasRedistributed = false;
                lock (n)
                {
                    if (n.objects != null)
                    {
                        n.objects.Remove(objInfo.key);
                        lock (objectToNode)
                        {
                            objectToNode.Remove(objInfo.key);
                        }
                    }
                    else
                        wasRedistributed = true;
                }

                if (wasRedistributed)
                    Insert(objInfo);
                else
                    InsertNew(objInfo, root);
            }
        }

        private void InsertNew(ObjectInfo objInfo, TreeNode tn)
        {
            bool contained = false;
            lock (tn)
            {
                if (tn.Contains(objInfo.loc))
                {
                    contained = true;
                    if (tn.ULChild == null)
                    {
                        contained = false;
                        tn.AddObject(objInfo);
                        lock (objectToNode)
                        {
                            objectToNode.Add(objInfo.key, tn);
                            if (tn.objects.Count > threshold)
                                tn.Redistribute(objectToNode);
                        }
                    }
                }
            }

            if (contained)
            {
                InsertNew(objInfo, tn.ULChild);
                InsertNew(objInfo, tn.URChild);
                InsertNew(objInfo, tn.LLChild);
                InsertNew(objInfo, tn.LRChild);
            }
        }

        public void Delete(string key)
        {
            // check if the object is already in the tree
            TreeNode n = null;
            lock (objectToNode)
            {
                if (objectToNode.ContainsKey(key))
                {
                    n = objectToNode[key];
                    objectToNode.Remove(key);
                }
            }

            if (n != null)
            {
                lock (n)
                    n.objects.Remove(key);
            }
        }

        public List<String> GetNearbyObjects(float top, float bottom, float left, float right, float zoomLevel)
        {
            List<String> objects = new List<String>();
            GetNearbyObjectsRec(top, bottom, left, right, zoomLevel, objects, root);
            return objects;
        }

        private void GetNearbyObjectsRec(float top, float bottom, float left, float right, float zoomLevel, 
            List<String> objects, TreeNode tn)
        {
            lock (tn)
            {
                if (!tn.Overlap(top, bottom, left, right))
                    return;
                if (tn.ULChild == null)
                {
                    foreach (KeyValuePair<String, ObjectInfo> obj in tn.objects)
                    {
                        if (zoomLevel >= obj.Value.minZoom && zoomLevel <= obj.Value.maxZoom &&
                            obj.Value.loc.Latitude <= top && obj.Value.loc.Latitude >= bottom &&
                            obj.Value.loc.Longitude <= right && obj.Value.loc.Longitude >= left)
                            objects.Add(obj.Key);
                    }
                    return;
                }
            }
            GetNearbyObjectsRec(top, bottom, left, right, zoomLevel, objects, tn.ULChild);
            GetNearbyObjectsRec(top, bottom, left, right, zoomLevel, objects, tn.URChild);
            GetNearbyObjectsRec(top, bottom, left, right, zoomLevel, objects, tn.LLChild);
            GetNearbyObjectsRec(top, bottom, left, right, zoomLevel, objects, tn.LRChild);
        }
    }

    public class TreeNode
    {
        // list of objects currently in the area represented by that node
        public Dictionary<String, ObjectInfo> objects;

        // location of the space represented by the node
        public Location upperLeft, upperRight, lowerLeft, lowerRight;

        public TreeNode parent;
        public TreeNode ULChild, URChild, LLChild, LRChild;

        public TreeNode()
        {
            this.objects = new Dictionary<string,ObjectInfo>();
        }

        public TreeNode(Location upperLeft, Location upperRight, Location lowerLeft, Location lowerRight, TreeNode parent)
        {
            this.parent = parent;
            this.upperLeft = upperLeft;
            this.upperRight = upperRight;
            this.lowerLeft = lowerLeft;
            this.lowerRight = lowerRight;
            this.objects = new Dictionary<string,ObjectInfo>();
        }

        public bool Contains(Location loc)
        {
            return loc.Latitude <= upperLeft.Latitude && loc.Latitude > lowerLeft.Latitude &&
                   loc.Longitude >= upperLeft.Longitude && loc.Longitude < upperRight.Longitude;
        }

        public bool Overlap(float top, float bottom, float left, float right)
        {
            return !(left > upperRight.Longitude ||
                     right < upperLeft.Longitude ||
                     bottom > upperLeft.Latitude ||
                     top < lowerLeft.Latitude);
        }

        public void AddObject(ObjectInfo objInfo)
        {
            if (ULChild != null)
                throw new Exception("Cannot add object to non-leaf TreeNode");
            if (objects.ContainsKey(objInfo.key))
                objects[objInfo.key] = objInfo;
            else
                objects.Add(objInfo.key, objInfo);
        }

        private int CompareObjectInfoLat(ObjectInfo x, ObjectInfo y)
        {
            if (x.loc.Latitude < y.loc.Latitude)
                return -1;
            else if (x.loc.Latitude > y.loc.Latitude)
                return 1;
            else
                return 0;
        }

        private int CompareObjectInfoLon(ObjectInfo x, ObjectInfo y)
        {
            if (x.loc.Longitude < y.loc.Longitude)
                return -1;
            else if (x.loc.Longitude > y.loc.Longitude)
                return 1;
            else
                return 0;
        }
        
        private float FindMedianLat()
        {
            List<ObjectInfo> objectValues = new List<ObjectInfo>(objects.Values);
            objectValues.Sort(CompareObjectInfoLat);
            return objectValues[objectValues.Count / 2].loc.Latitude;
        }

        private float FindMedianLon()
        {
            List<ObjectInfo> objectValues = new List<ObjectInfo>(objects.Values);
            objectValues.Sort(CompareObjectInfoLon);
            return objectValues[objectValues.Count / 2].loc.Longitude;
        }
        
        public void Redistribute(Dictionary<String, TreeNode> objectToNode)
        {
            if (ULChild != null)
                throw new Exception("Cannot redistribute: node has already been redistributed");
            float latSplit = FindMedianLat();
            float lonSplit = FindMedianLon();

            Location upperMiddle = new Location(upperLeft.Latitude, lonSplit, 0f);
            Location lowerMiddle = new Location(lowerLeft.Latitude, upperMiddle.Longitude, 0f);
            Location leftMiddle = new Location(latSplit, upperLeft.Longitude, 0f);
            Location rightMiddle = new Location(leftMiddle.Latitude, upperRight.Longitude, 0f);
            Location middle = new Location(leftMiddle.Latitude, upperMiddle.Longitude, 0f);

            ULChild = new TreeNode(upperLeft, upperMiddle, leftMiddle, middle, this);
            URChild = new TreeNode(upperMiddle, upperRight, middle, rightMiddle, this);
            LLChild = new TreeNode(leftMiddle, middle, lowerLeft, lowerMiddle, this);
            LRChild = new TreeNode(middle, rightMiddle, lowerMiddle, lowerRight, this);

            foreach (KeyValuePair<String, ObjectInfo> obj in objects)
            {
                if (ULChild.Contains(obj.Value.loc))
                {
                    ULChild.AddObject(obj.Value);
                    objectToNode[obj.Key] = ULChild;
                }
                else if (URChild.Contains(obj.Value.loc))
                {
                    URChild.AddObject(obj.Value);
                    objectToNode[obj.Key] = URChild;
                }
                else if (LLChild.Contains(obj.Value.loc))
                {
                    LLChild.AddObject(obj.Value);
                    objectToNode[obj.Key] = LLChild;
                }
                else if (LRChild.Contains(obj.Value.loc))
                {
                    LRChild.AddObject(obj.Value);
                    objectToNode[obj.Key] = LRChild;
                }
                else
                    throw new Exception("Object is not contained in any of the children");
            }

            objects = null;
        }
    }

    public struct ObjectInfo
    {
        public String key;

        // Location of the object
        public Location loc;

        // The min and max zoom levels at which the object is observable
        public float minZoom;
        public float maxZoom;

        public ObjectInfo(String key, Location loc, float minZoom, float maxZoom)
        {
            this.key = key;
            this.loc = loc;
            this.minZoom = minZoom;
            this.maxZoom = maxZoom;
        }
    }
}
