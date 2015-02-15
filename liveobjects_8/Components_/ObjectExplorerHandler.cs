/*

Copyright (c) 2009 Revant Kapoor (rk368@cornell.edu), Yilin Qin (yq33@cornell.edu), Krzysztof Ostrowski (krzys@cs.cornell.edu). All rights reserved.

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
using System.Text;
using System.Collections;

namespace QS._qss_x_.ObjectExplorer_
{
    class ObjectExplorerHandler : QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>,
        QS.Fx.Interface.Classes.IMetadataClient
        
   {

       #region Fields

       private QS.Fx.Object.IContext _mycontext;

       ObjectExplorer obj;

       //Stores all the objects added to this
       private List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _objects;

       // Stores the endpoints of the shared folder objects
       private IDictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
            QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>>> endPoints;
              
       //Stores connections
       private IDictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
           QS.Fx.Endpoint.IConnection> connections;

        //stores property endpoints
       private IDictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
                            QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IMetadata,
                                               QS.Fx.Interface.Classes.IMetadataClient>> propertyEndPoints;

       private IDictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
             QS.Fx.Endpoint.IConnection> propertyConnections;
       
       // private IDictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
       //    PropertyHandler> propertyHandlers;

       private static readonly QS.Fx.Reflection.IObjectClass sharedFolderObject =
           QS.Fx.Reflection.Library.LocalLibrary.ObjectClass<QS.Fx.Object.Classes.IDictionary<String,QS.Fx.Object.Classes.IObject>>();


       private static readonly QS.Fx.Reflection.IObjectClass sharedTextObject =
         QS.Fx.Reflection.Library.LocalLibrary.ObjectClass<QS.Fx.Object.Classes.ILogging_UI_Properties>();


       #endregion


       #region Constructor
       public ObjectExplorerHandler(QS.Fx.Object.IContext _mycontext, ObjectExplorer obj)
       {
            this._mycontext = _mycontext;
            endPoints = new Dictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
            QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>>>();

            connections = new Dictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
                QS.Fx.Endpoint.IConnection>();
           _objects = new List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>();
          
           //propertyHandlers = new Dictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
           //                 PropertyHandler>();
           propertyEndPoints = new Dictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
                            QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IMetadata,
                                               QS.Fx.Interface.Classes.IMetadataClient>>();
           propertyConnections = new Dictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
                QS.Fx.Endpoint.IConnection>();

          this.obj = obj;
       }

       #endregion


       #region Handler Methods

       //Adds an object to the explorer
       //Takes in a reference to the object
       //Returns the name of the object
       public String Add(QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
       {
           if (_object == null) return null;
           //Casts the Object to a generic object as of now.
           //Can be changed to metadata accepting class.
           QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> objReference =
               _object.CastTo<QS.Fx.Object.Classes.IObject>();

           String name = getObjectName(objReference);

           _objects.Add(objReference);
           return name;
       }

       //Removes an object from the handler
       public void Remove(QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
       {
           _objects.Remove(_object);
           endPoints.Remove(_object);
           connections.Remove(_object);
       }

       //Returns a name List of all the parent obects names
       //(ie. those objects added to the root
       public List<String> getObjectNameList()
       {
           List<String> nameList = new List<String>();
           IEnumerator < QS.Fx.Object.IReference < QS.Fx.Object.Classes.IObject >> iter =  _objects.GetEnumerator();
           while (iter.MoveNext())
           {
               nameList.Add(getObjectName(iter.Current));
           }
           return nameList;
       }

       //Returns a dictionary with the object names and objectReferences
       //This only returns the top level objects in the handler. The children are not
       //returned through this. Basically the objects which were added using the Add method
       //can be retreived through this method
       public List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> getObjects()
       {
           return new List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>(_objects);
       }

       //Returns the children of a given key (Object name)
       //Returns it as a dictionary with the keys as the names and the value as the object references of the children
       //If the object is not a shared folder or doesnt have any children, it returns an empty dictionary
       public IDictionary<String,QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> 
           getChildren(QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _key)
       {
           
           IDictionary<String,QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> returnMap = 
               new Dictionary<String,QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>();

           if (!_key.ObjectClass.IsSubtypeOf(sharedFolderObject)) return returnMap;

            QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>> endPoint;

           if (!endPoints.ContainsKey(_key))
           {
               //cast it to a folder object
               QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>> folderRef =
               _key.CastTo<QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>>();

               QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject> folder =
                       folderRef.Dereference(_mycontext);

               //Get the endPoint
               //Tells the endPoint that this class is the second end of the connection
                endPoint=
               _mycontext.DualInterface<
               QS.Fx.Interface.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>,
               QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>>(this);
               endPoints.Add(_key, endPoint);
               connections.Add(_key, endPoint.Connect(folder.Endpoint));
           }
           else
           {
               endPoints.TryGetValue(_key, out endPoint);
           }

           foreach (String _name in endPoint.Interface.Keys())
           {

               QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> objRef;

               if (endPoint.Interface.TryGetObject(_name, out objRef))
               {
                   QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> objReference =
                       objRef.CastTo<QS.Fx.Object.Classes.IObject>();
                   
                   returnMap.Add(_name,objReference);
               }
           }
           return returnMap;
       }

       //Mandatory to call this method, when the children of an object are hidden (ie. minimized)
       //This disconnects the object, and also removes its children from any other places
       public void hideChildren(QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _key)
       {
           //Disconnect the shared folder.
           QS.Fx.Endpoint.IConnection connection;
           if (connections.TryGetValue(_key, out connection))
           {
               connection.Dispose();
           }

           connections.Remove(_key);
           endPoints.Remove(_key);
       }


       public IEnumerable<QS.Fx.Value.Classes.IExplorableMetadata<object>> 
           getProperties(QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _key)
       {
           if (_key.ObjectClass.IsSubtypeOf(sharedTextObject))
           {
                QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IMetadata,
                                               QS.Fx.Interface.Classes.IMetadataClient> pEndPoint;
               // PropertyHandler pHandler = null;
                if (!propertyEndPoints.TryGetValue(_key, out pEndPoint))
                {
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.ILogging_UI_Properties> pRef =
                             _key.CastTo<QS.Fx.Object.Classes.ILogging_UI_Properties>();

                    QS.Fx.Object.Classes.ILogging_UI_Properties prop = pRef.Dereference(_mycontext);

                   // pHandler = new PropertyHandler();
                    pEndPoint = _mycontext.DualInterface<QS.Fx.Interface.Classes.IMetadata,
                                                    QS.Fx.Interface.Classes.IMetadataClient>(this);
                    QS.Fx.Endpoint.IConnection pConnection = pEndPoint.Connect(prop.Properties);
                    propertyEndPoints.Add(_key, pEndPoint);
                    propertyConnections.Add(_key, pConnection);
                    //propertyHandlers.Add(_key, pHandler);
                }
                IEnumerable<string> names = pEndPoint.Interface.GetMetadata();
                List<QS.Fx.Value.Classes.IExplorableMetadata<object>> tmp = new List<QS.Fx.Value.Classes.IExplorableMetadata<object>>();
                foreach (string name in names)
                {
                    QS.Fx.Value.Classes.IExplorableMetadata<object> value;
                    pEndPoint.Interface.TryGetMetadata(name, out value);
                    value.parentObject = _key;
                    tmp.Add(value);
                }
               return tmp;
           }
           return null;
       }

       public IEnumerable<QS.Fx.Value.Classes.IExplorableMetadata<object>>
           getPropertyChildren(QS.Fx.Value.Classes.IExplorableMetadata<object> _key)
       {
           QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IMetadata,
                                               QS.Fx.Interface.Classes.IMetadataClient> pEndPoint;

           List<QS.Fx.Value.Classes.IExplorableMetadata<object>> temp = new List<QS.Fx.Value.Classes.IExplorableMetadata<object>>();

           if( !propertyEndPoints.TryGetValue(_key.parentObject, out pEndPoint)) return null;
          
           foreach (string name in  _key.ChildrenName)
          {
              QS.Fx.Value.Classes.IExplorableMetadata<object> property;
              if (pEndPoint.Interface.TryGetMetadata(_key.IndexedName + "/" + name, out property))
              {
                  temp.Add(property);
                  property.IndexedName = _key.IndexedName + "/" + name;
                  property.parentObject = _key.parentObject;
              }
          }

           return temp;
       }

        public bool updateMetadata(QS.Fx.Value.Classes.IExplorableMetadata<object> _key, string value)
        {
            QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IMetadata,
                                               QS.Fx.Interface.Classes.IMetadataClient> pEndPoint;
            if (!propertyEndPoints.TryGetValue(_key.parentObject, out pEndPoint)) return false;
            string sProperty = value.Substring(value.IndexOf(":")+1);
            if (pEndPoint.Interface.SetMetadata(_key.Name, sProperty))
                return true;
            else
                return false;
        }

       #endregion


       #region IDictionaryClient<String,IObject> Members

       void QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>.Ready()
       {
           return;
       }

       void QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>.Added(String _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
       {
           //TODO
           return ;
       }

       void QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>.Removed(String _key)
       {
           //TODO
           return;
       }

       #endregion

       #region Utility methods
       private String getObjectName(QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> objReference)
       {
           QS.Fx.Attributes.IAttribute _nameattribute;
           objReference.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _nameattribute);
           return _nameattribute.Value;
       }
       #endregion


       #region IMetadataClient Members

       public void MetadataCallback()
       {
           obj.updateTreeView();
       }

       #endregion
   }
}
