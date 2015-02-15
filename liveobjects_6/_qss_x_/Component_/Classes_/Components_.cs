/*

Copyright (c) 2004-2009 Krzysztof Ostrowski. All rights reserved.

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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(
        QS.Fx.Reflection.ComponentClasses.Components_, 
        "Components", 
        "A user-interface object that displays a list of components and types that can be used in a visual object designer.")]
    public sealed partial class Components_ : UserControl, QS.Fx.Object.Classes.IUI
    {
        #region Constructor

        public Components_(QS.Fx.Object.IContext _mycontext)
        {
            InitializeComponent();
            QS._qss_x_.Reflection_.Library.LocalLibrary_._Load();
            this._ui = _mycontext.ExportedUI(this);
            my_treenode_ _rootnode = new my_treenode_(null, my_treenode_.kindof_.library_, QS._qss_x_.Reflection_.Library.LocalLibrary);
            _rootnode._reinitialize(true);
            this._my_treeview.BeginUpdate();
            this._my_treeview.Nodes.Add(_rootnode);
            this._my_treeview.Sort();
            this._my_treeview.EndUpdate();
        }

        #endregion

        #region Fields

        private QS.Fx.Endpoint.Internal.IExportedUI _ui;

        #endregion

        #region IUI Members

        QS.Fx.Endpoint.Classes.IExportedUI QS.Fx.Object.Classes.IUI.UI
        {
            get { return this._ui; }
        }

        #endregion

        #region __on_Load

        private void __on_Load(object sender, EventArgs e)
        {
        }

        #endregion

        #region _my_treenode

        private sealed class my_treenode_ : System.Windows.Forms.TreeNode
        {
            #region Constructor

            public my_treenode_(string _caption, kindof_ _kindof, object _o) : base()
            {
                this._caption = _caption;
                this._kindof = _kindof;
                this._o = _o;
            }

            #endregion

            #region Fields

            private kindof_ _kindof;
            private object _o;
            private string _caption, _name, _comment, _id;

            #endregion

            #region myimage_

            private enum myimage_ : int
            {
                document_ = 0,
                props_,
                closedfolder_,
                openfolder_,
                book_,
                globe_,
                plug_,
                wrench_,
                mail_,
                table_,
                wheel_
            }

            #endregion

            #region kindof_

            public enum kindof_ : int
            {
                library_ = 0, 
                objects_, 
                objectclasses_, 
                endpointclasses_, 
                interfaceclasses_, 
                valueclasses_, 
                object_, 
                objectclass_, 
                endpoints_, 
                endpoint_, 
                endpointclass_, 
                interfaceclass_, 
                operations_, 
                operation_, 
                operationclass_, 
                message_, 
                messageclass_, 
                values_, 
                value_, 
                valueclass_, 
                parameters_, 
                parameter_, 
                parametervalue_, 
                string_, 
                null_
            }

            #endregion

            #region imagemapping_

            private static IDictionary<kindof_, myimage_> imagemapping_0_, imagemapping_1_;

            static my_treenode_()
            {
                imagemapping_0_ = new Dictionary<kindof_, myimage_>();

                imagemapping_0_.Add(kindof_.library_, myimage_.book_);
                imagemapping_0_.Add(kindof_.objects_, myimage_.closedfolder_);
                imagemapping_0_.Add(kindof_.objectclasses_, myimage_.closedfolder_);
                imagemapping_0_.Add(kindof_.endpointclasses_, myimage_.closedfolder_);
                imagemapping_0_.Add(kindof_.interfaceclasses_, myimage_.closedfolder_);
                imagemapping_0_.Add(kindof_.valueclasses_, myimage_.closedfolder_);
                imagemapping_0_.Add(kindof_.object_, myimage_.globe_);
                imagemapping_0_.Add(kindof_.objectclass_, myimage_.table_);
                imagemapping_0_.Add(kindof_.endpoints_, myimage_.closedfolder_);
                imagemapping_0_.Add(kindof_.endpoint_, myimage_.wrench_);
                imagemapping_0_.Add(kindof_.endpointclass_, myimage_.table_);
                imagemapping_0_.Add(kindof_.interfaceclass_, myimage_.table_);
                imagemapping_0_.Add(kindof_.operations_, myimage_.closedfolder_);
                imagemapping_0_.Add(kindof_.operation_, myimage_.wheel_);
                imagemapping_0_.Add(kindof_.operationclass_, myimage_.table_);
                imagemapping_0_.Add(kindof_.message_, myimage_.mail_);
                imagemapping_0_.Add(kindof_.messageclass_, myimage_.table_);
                imagemapping_0_.Add(kindof_.values_, myimage_.closedfolder_);
                imagemapping_0_.Add(kindof_.value_, myimage_.mail_);
                imagemapping_0_.Add(kindof_.valueclass_, myimage_.table_);
                imagemapping_0_.Add(kindof_.parameters_, myimage_.closedfolder_);
                imagemapping_0_.Add(kindof_.parameter_, myimage_.props_);
                imagemapping_0_.Add(kindof_.parametervalue_, myimage_.mail_);
                imagemapping_0_.Add(kindof_.string_, myimage_.document_);
                imagemapping_0_.Add(kindof_.null_, myimage_.document_);

                imagemapping_1_ = new Dictionary<kindof_, myimage_>();

                imagemapping_1_.Add(kindof_.library_, myimage_.book_);
                imagemapping_1_.Add(kindof_.objects_, myimage_.openfolder_);
                imagemapping_1_.Add(kindof_.objectclasses_, myimage_.openfolder_);
                imagemapping_1_.Add(kindof_.endpointclasses_, myimage_.openfolder_);
                imagemapping_1_.Add(kindof_.interfaceclasses_, myimage_.openfolder_);
                imagemapping_1_.Add(kindof_.valueclasses_, myimage_.openfolder_);
                imagemapping_1_.Add(kindof_.object_, myimage_.globe_);
                imagemapping_1_.Add(kindof_.objectclass_, myimage_.table_);
                imagemapping_1_.Add(kindof_.endpoints_, myimage_.openfolder_);
                imagemapping_1_.Add(kindof_.endpoint_, myimage_.wrench_);
                imagemapping_1_.Add(kindof_.endpointclass_, myimage_.table_);
                imagemapping_1_.Add(kindof_.interfaceclass_, myimage_.table_);
                imagemapping_1_.Add(kindof_.operations_, myimage_.openfolder_);
                imagemapping_1_.Add(kindof_.operation_, myimage_.wheel_);
                imagemapping_1_.Add(kindof_.operationclass_, myimage_.table_);
                imagemapping_1_.Add(kindof_.message_, myimage_.mail_);
                imagemapping_1_.Add(kindof_.messageclass_, myimage_.table_);
                imagemapping_1_.Add(kindof_.values_, myimage_.openfolder_);
                imagemapping_1_.Add(kindof_.value_, myimage_.mail_);
                imagemapping_1_.Add(kindof_.valueclass_, myimage_.table_);
                imagemapping_1_.Add(kindof_.parameters_, myimage_.openfolder_);
                imagemapping_1_.Add(kindof_.parameter_, myimage_.props_);
                imagemapping_1_.Add(kindof_.parametervalue_, myimage_.mail_);
                imagemapping_1_.Add(kindof_.string_, myimage_.document_);
                imagemapping_1_.Add(kindof_.null_, myimage_.document_);
            }

            #endregion

            #region _reinitialize

            public void _reinitialize(bool _createchildren)
            {
                switch (this._kindof)
                {
                    case kindof_.library_:
                    case kindof_.objects_:
                    case kindof_.objectclasses_:
                    case kindof_.endpointclasses_:
                    case kindof_.interfaceclasses_:
                    case kindof_.valueclasses_:
                    case kindof_.endpoints_:
                    case kindof_.operations_:
                    case kindof_.values_:
                    case kindof_.parameters_:
                    case kindof_.message_:
                    case kindof_.messageclass_:
                    case kindof_.endpoint_:
                    case kindof_.operation_:
                    case kindof_.value_:
                        _createchildren = true;
                        break;

                    case kindof_.object_:
                    case kindof_.objectclass_:
                    case kindof_.endpointclass_:
                    case kindof_.interfaceclass_:
                    case kindof_.operationclass_:
                    case kindof_.valueclass_:
                    case kindof_.parameter_:
                    case kindof_.parametervalue_:
                    case kindof_.string_:
                    case kindof_.null_:
                        break;

                    default:
                        throw new NotImplementedException();
                }
                this.ImageIndex = (int) my_treenode_.imagemapping_0_[this._kindof];
                this.SelectedImageIndex = (int) my_treenode_.imagemapping_1_[this._kindof];
                QS.Fx.Attributes.IAttributes _attributes = null;
                IList<my_treenode_> _child_nodes = null;
                if (_createchildren)
                    _child_nodes = new List<my_treenode_>();
                this._id = null;
                string _label = null;
                switch (this._kindof)
                {
                    #region library_

                    case kindof_.library_:
                        {
                            if (_createchildren)
                            {
                                QS._qss_x_.Interface_.Classes_.ILibrary2_ _x = (QS._qss_x_.Interface_.Classes_.ILibrary2_)_o;
                                List<QS.Fx.Reflection.IObject> _objects = new List<QS.Fx.Reflection.IObject>();
                                foreach (QS.Fx.Reflection.IComponentClass _c in _x.ComponentClasses())
                                    _objects.Add(_c);
                                _child_nodes.Add(new my_treenode_(null, kindof_.objects_, _objects));
                                _child_nodes.Add(new my_treenode_(null, kindof_.objectclasses_, _x.ObjectClasses()));
                                _child_nodes.Add(new my_treenode_(null, kindof_.endpointclasses_, _x.EndpointClasses()));
                                _child_nodes.Add(new my_treenode_(null, kindof_.interfaceclasses_, _x.InterfaceClasses()));
                                _child_nodes.Add(new my_treenode_(null, kindof_.valueclasses_, _x.ValueClasses()));
                            }
                            _label = "library";
                        }
                        break;

                    #endregion

                    #region objects_

                    case kindof_.objects_:
                        {
                            if (_createchildren)
                            {
                                IEnumerable<QS.Fx.Reflection.IObject> _x = (IEnumerable<QS.Fx.Reflection.IObject>)_o;
                                foreach (QS.Fx.Reflection.IObject _e in _x)
                                {
                                    bool _hide = false;
                                    if (_e is QS._qss_x_.Reflection_.ComponentClass)
                                        _hide = ((QS._qss_x_.Reflection_.ComponentClass)_e)._IsInternal;
                                    if (!_hide)
                                        _child_nodes.Add(new my_treenode_(null, kindof_.object_, _e));
                                }
                            }
                            _label = "objects";
                        }
                        break;

                    #endregion

                    #region objectclasses_

                    case kindof_.objectclasses_:
                        {
                            if (_createchildren)
                            {
                                IEnumerable<QS.Fx.Reflection.IObjectClass> _x = (IEnumerable<QS.Fx.Reflection.IObjectClass>)_o;
                                foreach (QS.Fx.Reflection.IObjectClass _e in _x)
                                {
                                    bool _hide = false;
                                    if (_e is QS._qss_x_.Reflection_.ObjectClass)
                                        _hide = ((QS._qss_x_.Reflection_.ObjectClass)_e)._IsInternal;
                                    if (!_hide)
                                        _child_nodes.Add(new my_treenode_(null, kindof_.objectclass_, _e));
                                }
                            }
                            _label = "objectclasses";
                        }
                        break;

                    #endregion

                    #region endpointclasses_

                    case kindof_.endpointclasses_:
                        {
                            if (_createchildren)
                            {
                                IEnumerable<QS.Fx.Reflection.IEndpointClass> _x = (IEnumerable<QS.Fx.Reflection.IEndpointClass>)_o;
                                foreach (QS.Fx.Reflection.IEndpointClass _e in _x)
                                {
                                    bool _hide = false;
                                    if (_e is QS._qss_x_.Reflection_.EndpointClass)
                                        _hide = ((QS._qss_x_.Reflection_.EndpointClass)_e)._IsInternal;
                                    if (!_hide)
                                        _child_nodes.Add(new my_treenode_(null, kindof_.endpointclass_, _e));
                                }
                            }
                            _label = "endpointclasses";
                        }
                        break;

                    #endregion

                    #region interfaceclasses_

                    case kindof_.interfaceclasses_:
                        {
                            if (_createchildren)
                            {
                                IEnumerable<QS.Fx.Reflection.IInterfaceClass> _x = (IEnumerable<QS.Fx.Reflection.IInterfaceClass>)_o;
                                foreach (QS.Fx.Reflection.IInterfaceClass _e in _x)
                                {
                                    bool _hide = false;
                                    if (_e is QS._qss_x_.Reflection_.InterfaceClass)
                                        _hide = ((QS._qss_x_.Reflection_.InterfaceClass)_e)._IsInternal;
                                    if (!_hide)
                                        _child_nodes.Add(new my_treenode_(null, kindof_.interfaceclass_, _e));
                                }
                            }
                            _label = "interfaceclasses";
                        }
                        break;

                    #endregion

                    #region valueclasses_

                    case kindof_.valueclasses_:
                        {
                            if (_createchildren)
                            {
                                IEnumerable<QS.Fx.Reflection.IValueClass> _x = (IEnumerable<QS.Fx.Reflection.IValueClass>)_o;
                                foreach (QS.Fx.Reflection.IValueClass _e in _x)
                                {
                                    bool _hide = false;
                                    if (_e is QS._qss_x_.Reflection_.ValueClass)
                                        _hide = ((QS._qss_x_.Reflection_.ValueClass)_e)._IsInternal;
                                    if (!_hide)
                                        _child_nodes.Add(new my_treenode_(null, kindof_.valueclass_, _e));
                                }
                            }
                            _label = "valueclasses";
                        }
                        break;

                    #endregion

                    #region object_

                    case kindof_.object_:
                        {
                            QS.Fx.Reflection.IObject _x = (QS.Fx.Reflection.IObject)_o;
                            if (_createchildren)
                            {
                                IDictionary<string, QS.Fx.Reflection.IParameter> _parameters = _x.ClassParameters;
                                if (_parameters != null && _parameters.Count > 0)
                                    _child_nodes.Add(new my_treenode_(null, kindof_.parameters_, _parameters));
                                _child_nodes.Add(new my_treenode_(null, kindof_.objectclass_, _x.ObjectClass));
                            }
                            _id = _x.ID;
                            _label = "object";
                            _attributes = _x.Attributes;
                        }
                        break;

                    #endregion

                    #region objectclass_

                    case kindof_.objectclass_:
                        {
                            QS.Fx.Reflection.IObjectClass _x = (QS.Fx.Reflection.IObjectClass)_o;
                            if (_createchildren)
                            {
                                IDictionary<string, QS.Fx.Reflection.IParameter> _parameters = _x.ClassParameters;
                                if (_parameters != null && _parameters.Count > 0)
                                    _child_nodes.Add(new my_treenode_(null, kindof_.parameters_, _parameters));
                                _child_nodes.Add(new my_treenode_(null, kindof_.endpoints_, _x.Endpoints));
                            }
                            _label = "objectclass";
                            _attributes = _x.Attributes;
                            QS._qss_x_.Reflection_.ObjectClass _internal = (QS._qss_x_.Reflection_.ObjectClass)_x;
                            _id = _internal._Namespace.uuid_ + ":" + _internal.uuid_;
                        }
                        break;

                    #endregion

                    #region endpoints_

                    case kindof_.endpoints_:
                        {
                            if (_createchildren)
                            {
                                IDictionary<string, QS.Fx.Reflection.IEndpoint> _x = (IDictionary<string, QS.Fx.Reflection.IEndpoint>)_o;
                                foreach (QS.Fx.Reflection.IEndpoint _e in _x.Values)
                                    _child_nodes.Add(new my_treenode_(null, kindof_.endpoint_, _e));
                            }
                            _label = "endpoints";
                        }
                        break;

                    #endregion

                    #region endpoint_

                    case kindof_.endpoint_:
                        {
                            QS.Fx.Reflection.IEndpoint _x = (QS.Fx.Reflection.IEndpoint)_o;
                            if (_createchildren)
                            {
                                _child_nodes.Add(new my_treenode_(null, kindof_.endpointclass_, _x.EndpointClass));
                            }
                            _id = _x.ID;
                            _label = "endpoint";
                        }
                        break;

                    #endregion

                    #region endpointclass_

                    case kindof_.endpointclass_:
                        {
                            QS.Fx.Reflection.IEndpointClass _x = (QS.Fx.Reflection.IEndpointClass)_o;
                            if (_createchildren)
                            {
                                IDictionary<string, QS.Fx.Reflection.IParameter> _parameters = _x.ClassParameters;
                                if (_parameters != null && _parameters.Count > 0)
                                    _child_nodes.Add(new my_treenode_(null, kindof_.parameters_, _parameters));
                            }
                            _label = "endpointclass";
                            _attributes = _x.Attributes;
                            QS._qss_x_.Reflection_.EndpointClass _internal = (QS._qss_x_.Reflection_.EndpointClass)_x;
                            _id = _internal._Namespace.uuid_ + ":" + _internal.uuid_;
                        }
                        break;

                    #endregion

                    #region interfaceclass_

                    case kindof_.interfaceclass_:
                        {
                            QS.Fx.Reflection.IInterfaceClass _x = (QS.Fx.Reflection.IInterfaceClass)_o;
                            if (_createchildren)
                            {
                                IDictionary<string, QS.Fx.Reflection.IParameter> _parameters = _x.ClassParameters;
                                if (_parameters != null && _parameters.Count > 0)
                                    _child_nodes.Add(new my_treenode_(null, kindof_.parameters_, _parameters));
                                _child_nodes.Add(new my_treenode_(null, kindof_.operations_, _x.Operations));
                            }
                            _label = "interfaceclass";
                            _attributes = _x.Attributes;
                            QS._qss_x_.Reflection_.InterfaceClass _internal = (QS._qss_x_.Reflection_.InterfaceClass)_x;
                            _id = _internal._Namespace.uuid_ + ":" + _internal.uuid_;
                        }
                        break;

                    #endregion

                    #region operations_

                    case kindof_.operations_:
                        {
                            if (_createchildren)
                            {
                                IDictionary<string, QS.Fx.Reflection.IOperation> _x = (IDictionary<string, QS.Fx.Reflection.IOperation>)_o;
                                foreach (QS.Fx.Reflection.IOperation _e in _x.Values)
                                    _child_nodes.Add(new my_treenode_(null, kindof_.operation_, _e));
                            }
                            _label = "operations";
                        }
                        break;

                    #endregion

                    #region operation_

                    case kindof_.operation_:
                        {
                            QS.Fx.Reflection.IOperation _x = (QS.Fx.Reflection.IOperation)_o;
                            if (_createchildren)
                            {
                                _child_nodes.Add(new my_treenode_(null, kindof_.operationclass_, _x.OperationClass));
                            }
                            _id = _x.ID;
                            _label = "operation";
                        }
                        break;

                    #endregion

                    #region operationclass_

                    case kindof_.operationclass_:
                        {
                            if (_createchildren)
                            {
                                QS.Fx.Reflection.IOperationClass _x = (QS.Fx.Reflection.IOperationClass)_o;
                                _child_nodes.Add(new my_treenode_("incoming", kindof_.messageclass_, _x.Incoming));
                                _child_nodes.Add(new my_treenode_("outgoing", kindof_.messageclass_, _x.Outgoing));
                            }
                            _label = "operationclass";
                        }
                        break;

                    #endregion

                    #region message_

                    case kindof_.message_:
                        {
                            throw new NotImplementedException();
                        }
                        break;

                    #endregion

                    #region messageclass_

                    case kindof_.messageclass_:
                        {
                            if (_createchildren)
                            {
                                QS.Fx.Reflection.IMessageClass _x = (QS.Fx.Reflection.IMessageClass)_o;
                                _child_nodes.Add(new my_treenode_(null, kindof_.values_, _x.Values));
                            }
                            _label = "messageclass";
                        }
                        break;

                    #endregion

                    #region values_

                    case kindof_.values_:
                        {
                            if (_createchildren)
                            {
                                IDictionary<string, QS.Fx.Reflection.IValue> _x = (IDictionary<string, QS.Fx.Reflection.IValue>)_o;
                                foreach (QS.Fx.Reflection.IValue _e in _x.Values)
                                    _child_nodes.Add(new my_treenode_(null, kindof_.value_, _e));
                            }
                            _label = "values";
                        }
                        break;

                    #endregion

                    #region value_

                    case kindof_.value_:
                        {
                            QS.Fx.Reflection.IValue _x = (QS.Fx.Reflection.IValue)_o;
                            if (_createchildren)
                            {
                                _child_nodes.Add(new my_treenode_(null, kindof_.valueclass_, _x.ValueClass));
                            }
                            _id = _x.ID;
                            _label = "value";
                        }
                        break;

                    #endregion

                    #region valueclass_

                    case kindof_.valueclass_:
                        {
                            QS.Fx.Reflection.IValueClass _x = (QS.Fx.Reflection.IValueClass)_o;
                            if (_createchildren)
                            {
                                IDictionary<string, QS.Fx.Reflection.IParameter> _parameters = _x.ClassParameters;
                                if (_parameters != null && _parameters.Count > 0)
                                    _child_nodes.Add(new my_treenode_(null, kindof_.parameters_, _parameters));
                            }
                            _label = "valueclass";
                            _attributes = _x.Attributes;
                            QS._qss_x_.Reflection_.ValueClass _internal = (QS._qss_x_.Reflection_.ValueClass) _x;
                            _id = _internal._Namespace.uuid_ + ":" + _internal.uuid_;
                        }
                        break;

                    #endregion

                    #region parameters_

                    case kindof_.parameters_:
                        {
                            if (_createchildren)
                            {
                                IDictionary<string, QS.Fx.Reflection.IParameter> _x = (IDictionary<string, QS.Fx.Reflection.IParameter>)_o;
                                foreach (QS.Fx.Reflection.IParameter _p in _x.Values)
                                    _child_nodes.Add(new my_treenode_(null, kindof_.parameter_, _p));
                            }
                            _label = "parameters";
                        }
                        break;

                    #endregion

                    #region parameter_

                    case kindof_.parameter_:
                        {
                            QS.Fx.Reflection.IParameter _x = (QS.Fx.Reflection.IParameter)_o;
                            _id = _x.ID;
                            string _a = null;
                            kindof_ _k;
                            object _v = _x.Value;
                            while ((_v != null) && (_v is QS._qss_x_.Reflection_.ParameterValue))
                                _v = ((QS._qss_x_.Reflection_.IParameterValue)_v).Parameter.Value;
                            switch (_x.ParameterClass)
                            {
                                case QS.Fx.Reflection.ParameterClass.ObjectClass:
                                    {
                                        if (_v == null)
                                            _k = kindof_.null_;
                                        else
                                            _k = kindof_.objectclass_;
                                    }
                                    break;

                                case QS.Fx.Reflection.ParameterClass.EndpointClass:
                                    {
                                        if (_v == null)
                                            _k = kindof_.null_;
                                        else
                                            _k = kindof_.endpointclass_;
                                    }
                                    break;
                                
                                case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                                    {
                                        if (_v == null)
                                            _k = kindof_.null_;
                                        else
                                            _k = kindof_.interfaceclass_;
                                    }
                                    break;
                                
                                case QS.Fx.Reflection.ParameterClass.ValueClass:
                                    {
                                        if (_v == null)
                                            _k = kindof_.null_;
                                        else
                                            _k = kindof_.valueclass_;
                                    }
                                    break;
                                
                                case QS.Fx.Reflection.ParameterClass.Value:
                                    {
                                        if (_v == null)
                                            _k = kindof_.null_;
                                        else if (_v is QS.Fx.Reflection.IObject)
                                            _k = kindof_.object_;
                                        else
                                        {
                                            _k = kindof_.string_;
                                            _v = _v.ToString();
                                        }
                                    }
                                    break;

                                default:
                                    throw new NotImplementedException();
                            }
                            if (_createchildren)
                            {
                                _child_nodes.Add(new my_treenode_(_a, _k, _v));
                            }
                            _label = "parameter";
                            _attributes = _x.Attributes;
                        }
                        break;

                    #endregion

                    #region string_

                    case kindof_.string_:
                        {
                            _label = (string)_o;
                        }
                        break;

                    #endregion

                    #region null_

                    case kindof_.null_:
                        {
                            _label = "null";
                        }
                        break;

                    #endregion

                    default:
                        throw new NotImplementedException();
                }
                StringBuilder _ss = new StringBuilder();
                if (this._caption != null)
                {
                    _ss.Append(this._caption);
                    _ss.Append(" ");
                }
                _ss.Append(_label);
                QS.Fx.Attributes.IAttribute _name;
                if ((_attributes != null) && _attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _name))
                {
                    _ss.Append(" \"");
                    _ss.Append(_name.Value);
                    _ss.Append("\"");
                }
                else
                {
                    _ss.Append(" ");
                    _ss.Append(_id);
                }
                this._name = _ss.ToString();
                this.Text = this._name;
                QS.Fx.Attributes.IAttribute _comment;
                if ((_attributes != null) && _attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_comment, out _comment))
                    this._comment = _comment.Value;
                else
                    this._comment = null;
                StringBuilder _tooltip = new StringBuilder();
                if (this._id != null)
                {
                    _tooltip.Append("id = ");
                    _tooltip.AppendLine(this._id);
                }
                if (this._name != null)
                {
                    _tooltip.Append("name = \"");
                    _tooltip.Append(this._name);
                    _tooltip.AppendLine("\"");
                }
                if (this._comment != null)
                {
                    _tooltip.AppendLine("comment:");
                    _tooltip.AppendLine(this._comment);
                }
                this.ToolTipText = _tooltip.ToString();
                this.Nodes.Clear();
                if (_createchildren)
                {
                    foreach (my_treenode_ _n in _child_nodes)
                        _n._reinitialize(false);
                    foreach (my_treenode_ _n in _child_nodes)
                        this.Nodes.Add(_n);
                }
            }

            #endregion

            #region _drag

            public object _drag()
            {
                switch (this._kindof)
                {
                    case kindof_.valueclass_:
                        return new QS._qss_x_.Reflection_.draggable_library_object_(
                            QS._qss_x_.Reflection_.draggable_library_object_.category_.valueclass_, 
                            (QS.Fx.Reflection.IValueClass) this._o);
                    case kindof_.interfaceclass_:
                        return new QS._qss_x_.Reflection_.draggable_library_object_(
                            QS._qss_x_.Reflection_.draggable_library_object_.category_.interfaceclass_, 
                            (QS.Fx.Reflection.IInterfaceClass) this._o);
                    case kindof_.endpointclass_:
                        return new QS._qss_x_.Reflection_.draggable_library_object_(
                            QS._qss_x_.Reflection_.draggable_library_object_.category_.endpointclass_, 
                            (QS.Fx.Reflection.IEndpointClass) this._o);
                    case kindof_.objectclass_:
                        return new QS._qss_x_.Reflection_.draggable_library_object_(
                            QS._qss_x_.Reflection_.draggable_library_object_.category_.objectclass_, 
                            (QS.Fx.Reflection.IObjectClass) this._o);
                    case kindof_.object_:
                        return new QS._qss_x_.Reflection_.draggable_library_object_(
                            QS._qss_x_.Reflection_.draggable_library_object_.category_.object_, 
                            (QS.Fx.Reflection.IObject) this._o);
                    case kindof_.library_:
                    case kindof_.objects_:
                    case kindof_.objectclasses_:
                    case kindof_.endpointclasses_:
                    case kindof_.interfaceclasses_:
                    case kindof_.valueclasses_:
                    case kindof_.endpoints_:
                    case kindof_.operations_:
                    case kindof_.values_:
                    case kindof_.parameters_:
                    case kindof_.message_:
                    case kindof_.messageclass_:
                    case kindof_.endpoint_:
                    case kindof_.operation_:
                    case kindof_.value_:
                    case kindof_.operationclass_:
                    case kindof_.parameter_:
                    case kindof_.parametervalue_:
                    case kindof_.string_:
                    case kindof_.null_:
                        return null;
                    default:
                        throw new NotImplementedException();
                }
            }

            #endregion
        }

        #endregion

        #region _my_treeview_DoubleClick

        private void _my_treeview_DoubleClick(object sender, EventArgs e)
        {
            lock (this)
            {
                my_treenode_ _n = _my_treeview.SelectedNode as my_treenode_;
                if (_n != null)
                {
                    _my_treeview.BeginUpdate();
                    _n._reinitialize(true);
                    _my_treeview.EndUpdate();
                }
            }
        }

        #endregion

        #region _my_treeview_ItemDrag

        private void _my_treeview_ItemDrag(object sender, ItemDragEventArgs e)
        {
            lock (this)
            {
                my_treenode_ _n = e.Item as my_treenode_;
                if (_n != null)
                {
                    object _o = _n._drag();
                    if (_o != null)
                        DoDragDrop(_o, DragDropEffects.Copy);
                }
            }
        }

        #endregion
    }
}
