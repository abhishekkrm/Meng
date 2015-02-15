/*

Copyright 2009, Jared Cantwell. All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted 
provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions 
   and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice, this list of 
   conditions and the following disclaimer in the documentation and/or other materials provided
  with the distribution.

THIS SOFTWARE IS PROVIDED "AS IS" BY THE ABOVE COPYRIGHT HOLDER(S) AND ALL OTHER CONTRIBUTORS 
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE ABOVE 
COPYRIGHT HOLDER(S) OR ANY OTHER CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
OF THE POSSIBILITY OF SUCH DAMAGE. 
 
*/

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Value = QS._qss_x_.Properties_.Value_;

namespace liveobjects_8.Components_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.DataFlow, 
        "Data Flow Component", "")]
    public sealed class DataFlow
        : QS._qss_x_.Properties_.Component_.Base_,
          QS.Fx.Object.Classes.IDataFlowExposed<Value.Round_, QS.Fx.Serialization.ISerializable>,
          QS.Fx.Interface.Classes.IDataFlow,
          QS.Fx.Interface.Classes.IDataFlowClient,
          QS.Fx.Interface.Classes.IAggUpdaterClient<Value.Round_, QS.Fx.Serialization.ISerializable>
    {

        #region Constructor

        public DataFlow(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("rules", QS.Fx.Reflection.ParameterClass.Value)]
            String rules,
            [QS.Fx.Reflection.Parameter("name", QS.Fx.Reflection.ParameterClass.Value)]
            string name
        ) : base(_mycontext, true)
        {
            if (name == null)
                name = "";
            this.debug_identifier = name;

            upper_endpoint = _mycontext.DualInterface<QS.Fx.Interface.Classes.IDataFlowClient,
                                QS.Fx.Interface.Classes.IDataFlow>(this);
            upper_endpoint.OnConnect += 
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.processExternalEvents)));
                    });
            lower_endpoint = _mycontext.DualInterface<QS.Fx.Interface.Classes.IDataFlow,
                                QS.Fx.Interface.Classes.IDataFlowClient>(this);

            agg_endpoint = _mycontext.DualInterface<QS.Fx.Interface.Classes.IAggUpdater<Value.Round_, QS.Fx.Serialization.ISerializable>,
                                QS.Fx.Interface.Classes.IAggUpdaterClient<Value.Round_, QS.Fx.Serialization.ISerializable>>(this);

            aggRegisters = new Dictionary<Int32, AggRegister>();
            registers = new List<IRegister>();
            registerNameMap = new Dictionary<string, IRegister>();
            eventQueue = new Queue<ExternalEvent>();

            dissRegisters = new Dictionary<Int32, DissRegister>();
            _mostRecentRound = new Value.Round_(new QS.Fx.Base.Incarnation(0), new QS.Fx.Base.Index(0));

            setup(rules);    
        }

        #endregion

        #region Fields

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDataFlow,
            QS.Fx.Interface.Classes.IDataFlowClient>
                lower_endpoint;

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDataFlowClient,
            QS.Fx.Interface.Classes.IDataFlow>
                upper_endpoint;

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IAggUpdater<Value.Round_, QS.Fx.Serialization.ISerializable>,
            QS.Fx.Interface.Classes.IAggUpdaterClient<Value.Round_, QS.Fx.Serialization.ISerializable>>
                agg_endpoint;

        private MetaRule _rootMR;
        private List<IRegister> registers;
        private Dictionary<Int32, AggRegister> aggRegisters;
        private Dictionary<Int32, DissRegister> dissRegisters;
        private Dictionary<String, IRegister> registerNameMap;
        private Queue<ExternalEvent> eventQueue;
        private QS.Fx.Clock.IAlarm _alarm;
        private Value.Round_ _mostRecentRound;
        private long version = 0;
        private QS.Fx.Base.Incarnation _rootIncarnation = null;
        private string debug_identifier;

        #endregion

        #region Operations

        private interface IOperation
        {
            QS.Fx.Serialization.ISerializable Perform(List<IRegister> _params);
        }

        private class OpAdd : IOperation
        {
            #region IOperation Members

            QS.Fx.Serialization.ISerializable IOperation.Perform(List<IRegister> _params)
            {
                Int32 sum = 0;

                foreach (IRegister b in _params)
                {
                    sum += Int32.Parse(((QS.Fx.Base.Index)b.Value).String);
                }

                return new QS.Fx.Base.Index(sum);
            }

            #endregion
        }

        private class OpNoOp : IOperation
        {
            #region IOperation Members

            QS.Fx.Serialization.ISerializable IOperation.Perform(List<IRegister> _params)
            {
                return _params[0].Value;
            }

            #endregion
        }

        #endregion

        #region Delegates

        private delegate void RuleDelegate(IRule r);
        private delegate void RegisterDelegate(IRegister reg);

        #endregion

        #region IRule

        private interface IRule
        {
            bool IsActive { get; }

            void Run();

            event RuleDelegate Activated; 
        }

        #endregion

        #region Rule

        private class Rule : IRule
        {
            private Rule(IRegister _dest, IOperation _op)
            {
                this._dest = _dest;
                this._op = _op;
                this._listeners = new List<RuleDelegate>();
                this._active = false;
            }

            public Rule(IRegister _dest, IOperation _op, List<IRegister> _params)
                : this(_dest, _op)
            {
                this._params = _params;

                foreach (IRegister reg in _params)
                {
                    reg.OnChange += new RegisterDelegate(reg_OnChange);
                }
            }

            public Rule(IRegister _dest, IOperation _op, IRegister _param)
                : this(_dest, _op)
            {
                this._params = new List<IRegister>();
                this._params.Add(_param);
                _param.OnChange += new RegisterDelegate(reg_OnChange);
            }

            #region Fields

            private IRegister _dest;
            private IOperation _op;
            private List<IRegister> _params;
            private List<RuleDelegate> _listeners;
            private bool _active;

            #endregion

            #region IRule Members

            public void Run()
            {
                _dest.Value = _op.Perform(_params);
                _active = false;
            }

            public bool IsActive
            {
                get { return _active; }
            }

            public event RuleDelegate Activated
            {
                add { _listeners.Add(value); }
                remove { _listeners.Remove(value); }
            }

            #endregion

            #region Helper Methods

            private void FireActivated()
            {
                foreach (RuleDelegate func in _listeners)
                    func(this);
            }

            private void reg_OnChange(IRegister reg)
            {
                if (!_active)
                {
                    _active = true;
                    FireActivated();
                }
            }

            #endregion 
        }

        #endregion

        #region MetaRule

        private class MetaRule : IRule
        {
            public MetaRule()
            {
                this._rules = new List<IRule>();
                this._active = new List<int>();
                this._listeners = new List<RuleDelegate>();
            }

            #region Fields
            private List<IRule> _rules;
            private List<Int32> _active;
            private List<RuleDelegate> _listeners;
            #endregion

            public void Add(IRule rule)
            {
                rule.Activated += new RuleDelegate(rule_Activated);
                _rules.Add(rule);
            }

            #region IRule Members

            public void Run()
            {
                while (_active.Count > 0)
                {
                    Int32 min = _active.Min();
                    
                    IRule rule = _rules[min];
                    rule.Run(); // This can add more elements to _active through callbacks

                    // This must be last so calls to rule_Activated will not add the
                    // currently processing rule again.
                    _active.Remove(min);
                }
            }

            public bool IsActive
            {
                get 
                {
                    return _active.Count > 0;
                }
            }

            public event RuleDelegate Activated
            {
                add { _listeners.Add(value); }
                remove { _listeners.Remove(value); }
            }

            #endregion 

            #region Helper Methods

            void rule_Activated(IRule r)
            {
                if (_active.Count == 0)
                    FireActivated();

                Int32 index = _rules.IndexOf(r);
                if (!_active.Contains(index))
                {
                    _active.Add(index);
                }
            }

            private void FireActivated()
            {
                foreach (RuleDelegate func in _listeners)
                    func(this);
            }

            #endregion

        }

        #endregion


        #region IRegister

        private interface IRegister
        {
            QS.Fx.Serialization.ISerializable Value { get; set; }
            String Name { get; }
            Int32 Id { get; }
            event RegisterDelegate OnChange;
        }

        #endregion


        #region Register

        private class Register : IRegister
        {
            public Register(DataFlow home, String _name, Int32 _id, QS.Fx.Serialization.ISerializable _value)
            {
                this._home = home;
                this._name = _name;
                this._id = _id;
                this._value = _value;
                this._listeners = new List<RegisterDelegate>();
            }

            #region Fields
            private DataFlow _home;
            private String _name;
            private Int32 _id;
            private QS.Fx.Serialization.ISerializable _value;
            private List<RegisterDelegate> _listeners;
            #endregion

            #region Accessors

            public QS.Fx.Serialization.ISerializable Value
            {
                set
                {
                    if (!value.Equals(this._value))
                    {
                        this._value = value;
                        FireChanged();
                    }
                }
                get
                {
                    return _value;
                }
            }

            public String Name
            {
                get { return this._name; }
            }

            public int Id
            {
                get { return this._id; }
            }

            public event RegisterDelegate OnChange
            {
                add { _listeners.Add(value); }
                remove { _listeners.Remove(value); }
            }

            #endregion

            protected void FireChanged()
            {
                foreach (RegisterDelegate func in _listeners)
                    func(this);
            }

            #region IRegister Members

            #endregion
        }

        #endregion

        #region ExternalRegister

        private class ExternalRegister : IRegister
        {
            private ExternalRegister(DataFlow _home, String _name, Int32 _id, Int32 _externalId)
            {
                this._home = _home;
                this._name = _name;
                this._id = _id;
                this._xid = _externalId;
                this._listeners = new List<RegisterDelegate>();
            }

            public ExternalRegister(DataFlow _home, String _name, Int32 _id, Int32 _externalId,
                QS.Fx.Endpoint.Internal.IDualInterface<
                    QS.Fx.Interface.Classes.IDataFlowClient,QS.Fx.Interface.Classes.IDataFlow> _endpoint)
                : this(_home, _name, _id, _externalId)
            {
                this._upper = _endpoint;
            }

            public ExternalRegister(DataFlow _home, String _name, Int32 _id, Int32 _externalId,
                QS.Fx.Endpoint.Internal.IDualInterface<
                    QS.Fx.Interface.Classes.IDataFlow, QS.Fx.Interface.Classes.IDataFlowClient> _endpoint)
                : this(_home, _name, _id, _externalId)
            {
                this._lower = _endpoint;
            }

            #region Fields
            private DataFlow _home;
            private String _name;
            private Int32 _id;
            private Int32 _xid;
            private QS.Fx.Serialization.ISerializable _value;
            private List<RegisterDelegate> _listeners;
            private QS.Fx.Endpoint.Internal.IDualInterface<
                        QS.Fx.Interface.Classes.IDataFlowClient,
                        QS.Fx.Interface.Classes.IDataFlow> _upper;
            private QS.Fx.Endpoint.Internal.IDualInterface<
                        QS.Fx.Interface.Classes.IDataFlow,
                        QS.Fx.Interface.Classes.IDataFlowClient> _lower;

            #endregion

            #region Accessors

            public QS.Fx.Serialization.ISerializable Value
            {
                set
                {
                    this._value = value;
                    if(_upper != null && _upper.IsConnected)
                        _upper.Interface.Send(_xid, _home.version, value);
                    if (_lower != null && _lower.IsConnected)
                        _lower.Interface.Send(_xid, _home.version, value);
                }
                get
                {
                    return _value;
                }
            }

            public String Name
            {
                get { return this._name; }
            }

            public int Id
            {
                get { return this._id; }
            }

            public event RegisterDelegate OnChange
            {
                add { _listeners.Add(value); }
                remove { _listeners.Remove(value); }
            }

            #endregion

            protected void FireChanged()
            {
                foreach (RegisterDelegate func in _listeners)
                    func(this);
            }
        }

        #endregion

        #region AggRegister

        private abstract class AggRegister : IRegister
        {

            public AggRegister(DataFlow _home, String _name, Int32 _id, IRegister _target)
            {
                this._home = _home;
                this._name = _name;
                this._id = _id;
                this._target = _target;
                this._listeners = new List<RegisterDelegate>();
            }

            protected DataFlow _home;
            protected String _name;
            protected Int32 _id;
            protected IRegister _target;
            protected List<RegisterDelegate> _listeners;

            private QS.Fx.Serialization.ISerializable _value;

            #region IRegister Members

            public QS.Fx.Serialization.ISerializable Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    if (!value.Equals(this._value))
                    {
#if VERBOSE
                        if (_home._logger != null)
                            _home._logger.Log("##DataFlow.AggRegister(): Setting Value: was \n" + 
                                QS.Fx.Printing.Printable.ToString(this._value) + "\n\nis:" +
                                QS.Fx.Printing.Printable.ToString(value));
#endif
                        this._value = value;
                        FireChanged();
                    }
                }
            }

            public string Name
            {
                get { return _name; }
            }

            public int Id
            {
                get { return _id; }
            }

            public event RegisterDelegate OnChange
            {
                add { _listeners.Add(value); }
                remove { _listeners.Remove(value); }
            }

            #endregion

            public IRegister Target
            {
                get { return _target; }
            }

            abstract public QS.Fx.Serialization.ISerializable performOp(QS.Fx.Serialization.ISerializable param);

            protected void FireChanged()
            {
                foreach (RegisterDelegate func in _listeners)
                    func(this);
            }
        }

        private class AggMinRegister : AggRegister
        {

            public AggMinRegister(DataFlow _home, String _name, Int32 _id, IRegister _target)
                : base(_home, _name, _id, _target)
            { }

            public override QS.Fx.Serialization.ISerializable performOp(QS.Fx.Serialization.ISerializable param)
            {
                QS.Fx.Base.Index min = (QS.Fx.Base.Index)param;
                QS.Fx.Base.Index myVal = (QS.Fx.Base.Index)_target.Value;
#if VERBOSE
                if (_home._logger != null)
                    _home._logger.Log("##DataFlow.appendToAggregationMessage(): Old min: " + min.String);
#endif
                // if my value is less than the value in the token
                if (myVal.CompareTo(min) < 0)
                {
                    min = myVal;
                }

#if VERBOSE
                if (_home._logger != null)
                    _home._logger.Log("##DataFlow.appendToAggregationMessage(): New min: " + min.String);
#endif

                return min;
            }
        }

        private class AggUnionRegister : AggRegister
        {
            
            public AggUnionRegister(DataFlow _home, String _name, Int32 _id, IRegister _target)
                : base(_home, _name, _id, _target)
            { }

            public override QS.Fx.Serialization.ISerializable performOp(QS.Fx.Serialization.ISerializable param)
            {
                QS._qss_x_.Properties_.Value_.Array_ set = (QS._qss_x_.Properties_.Value_.Array_)param;
                QS._qss_x_.Properties_.Value_.Array_ myItems = (QS._qss_x_.Properties_.Value_.Array_) _target.Value;

                bool found = false;

#if VERBOSE
                if (_home._logger != null)
                    _home._logger.Log("##DataFlow.appendToAggregationMessage(): Old set: " + QS.Fx.Printing.Printable.ToString(set));
#endif

                foreach (QS.Fx.Serialization.ISerializable myItem in myItems.Tokens)
                {
                    found = false;
                    // See if the set contains my item
                    foreach (QS.Fx.Serialization.ISerializable item in set.Tokens)
                    {
                        // if the set contains my item, ignore it
                        if (((QS.Fx.Base.Index)item).Equals((QS.Fx.Base.Index)myItem))
                        {
                            found = true;
                            break;
                        }
                    }
                    // Add item if it wasn't found in the set
                    if(!found)
                        set.Add(myItem);
                }

#if VERBOSE
                if (_home._logger != null)
                    _home._logger.Log("##DataFlow.appendToAggregationMessage(): New set: " + QS.Fx.Printing.Printable.ToString(set));
#endif

                return set;
            }
        }

        #endregion

        #region DissRegister

        private class DissRegister : IRegister
        {

            public DissRegister(DataFlow _home, String _name, Int32 _id, IRegister _target, Value.Round_ _version)
            {
                this._home = _home;
                this._name = _name;
                this._id = _id;
                this._target = _target;
                this._version = _version;
                this._listeners = new List<RegisterDelegate>();

                Target.OnChange += new RegisterDelegate(Target_OnChange);
            }

            private DataFlow _home;
            private String _name;
            private Int32 _id;
            private IRegister _target;
            private Value.Round_ _version;
            private List<RegisterDelegate> _listeners;

            private QS.Fx.Serialization.ISerializable _value;

            #region IRegister Members

            public QS.Fx.Serialization.ISerializable Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                    FireChanged();
                }
            }

            public Value.Round_ Version
            {
                get { return _version; }
                set { _version = value; }
            }

            public string Name
            {
                get { return _name; }
            }

            public int Id
            {
                get { return _id; }
            }

            public event RegisterDelegate OnChange
            {
                add { _listeners.Add(value); }
                remove { _listeners.Remove(value); }
            }

            #endregion

            public IRegister Target
            {
                get { return _target; }
            }

            void Target_OnChange(IRegister reg)
            {
                _value = reg.Value;
                FireChanged();
                _version = _home._mostRecentRound;
            }

            protected void FireChanged()
            {
                foreach (RegisterDelegate func in _listeners)
                    func(this);
            }
        }

        #endregion

        #region ExternalEvent
        private class ExternalEvent
        {
            public ExternalEvent(int id, long version, QS.Fx.Serialization.ISerializable obj)
            {
                this._id = id;
                this._version = version;
                this._obj = obj;
            }

            private int _id;
            private long _version;
            private QS.Fx.Serialization.ISerializable _obj;

            public Int32 Id
            {
                get { return _id; }
            }

            public long Version
            {
                get { return _version; }
            }

            public QS.Fx.Serialization.ISerializable Value
            {
                get { return _obj; }
            }
        }
        
        #endregion

        #region File Parsing

        /*
         * Bucket = name
            Val => Bucket | Number
            LHS => Bucket | U[Bucket] | L[Bucket]

            Rule => Assignment | Calculation
            Assignment => LHS := AssRHS
            AssRHS => AggOp Bucket | Bucket
            AggOp => min | max | union | ...

            Calculation => LHS = CalcRHS
            CalcRHS => UnaryOp | BinaryOp
            UnaryOp => Not | ...
            Not => not Val
            BinaryOp => Val BinOp Val
            BinOp => + | - | * | /
         * 
         */

        private void setup(String contents)
        {
            Queue<String> tokens = tokenize(contents.Split(new char[] {'\n'}));
            // TODO: make sure its a metarule!
            _rootMR = (MetaRule)parseMetaRule(tokens);
        }

        private Queue<String> tokenize(String[] lines)
        {
            List<String> tokens = new List<string>();

            foreach(String line in lines)
            {
                String[] lineTokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                tokens.AddRange(lineTokens);
                tokens.Add("\n");
            }

            return new Queue<String>(tokens);
        }

        private IRule parseIRule(Queue<String> tokens)
        {
            // if { then I will parse a metarule, otherwise, just this line is a rule
            if (tokens.Peek().Equals("{"))
                return parseMetaRule(tokens);
            else
                return parseRule(tokens);
        }

        private enum RegisterType {UNKNOWN, SET, INT, EXTERNAL, CONSTANT};

        private IRegister parseRegister(String name)
        {
            return parseRegister(name, RegisterType.UNKNOWN);
        }


        private IRegister parseRegister(String name, RegisterType rType)
        {
            if (!registerNameMap.ContainsKey(name))
            {
                IRegister register;
                int rv;

                if (name.StartsWith("["))
                {
                    String rName = name.Substring(1, name.Length - 3);
                    Int32 xid = registerNameMap[rName].Id;

                    if (name.EndsWith("U"))
                    {
                        register = new ExternalRegister(this, rName, registers.Count, xid, upper_endpoint);
                    }
                    else if (name.EndsWith("L"))
                    {
                        register = new ExternalRegister(this, rName, registers.Count, xid, lower_endpoint);
                    }
                    else
                        throw new Exception("Unrecognized register name: " + name);
                }
                else if (Int32.TryParse(name, out rv))
                {
                    register = new Register(this, name, registers.Count, new QS.Fx.Base.Index(rv));
                }
                else if (rType.Equals(RegisterType.SET))
                {
                    register = new Register(this, name, registers.Count, new Value.Array_());
                }
                else
                {
                    register = new Register(this, name, registers.Count, new QS.Fx.Base.Index(0));
                }

                registers.Add(register);
                registerNameMap[name] = register;
            }

            return registerNameMap[name];
        }

        private MetaRule parseMetaRule(Queue<String> tokens)
        {
            if (tokens.Count == 0) 
                throw new Exception("Attempted to parse a MetaRule, but no tokens.");
            String openBrace = tokens.Dequeue();
            parseStatementEnd(tokens);
            if (!openBrace.Equals("{"))
                throw new Exception("parseMetaRule: expected '{', found '" + openBrace + "'");

            MetaRule meta = new MetaRule();

            while (!tokens.Peek().Equals("}"))
            {
                if(tokens.Peek().Equals("set") || tokens.Peek().Equals("int"))
                    parseRegisterDeclaration(tokens);
                else
                    meta.Add(parseIRule(tokens));
            }
            tokens.Dequeue();   // eat the }

            parseStatementEnd(tokens);

            return meta;
        }

        private void parseRegisterDeclaration(Queue<string> tokens)
        {
            String type = tokens.Dequeue();

            if (type.Equals("set"))
                parseRegister(tokens.Dequeue(), RegisterType.SET);
            else if (type.Equals("int"))
                parseRegister(tokens.Dequeue(), RegisterType.INT);

            parseStatementEnd(tokens);
        }

        private Rule parseRule(Queue<string> tokens)
        {
            if (tokens.Count == 0)
                throw new Exception("Attempted to parse a MetaRule, but no tokens.");

            String lhsStr = tokens.Dequeue();
            IRegister lhs = parseRegister(lhsStr);

            String op = tokens.Dequeue();
            if (op.Equals("="))
                return parseBinaryOp(lhs, tokens);
            else if (op.Equals(":="))
                return parseAssignment(lhs, tokens);
            else if (op.Equals("<="))
                return parseDissemination(lhs, tokens);

            throw new Exception("Unable to parse rule.");
        }

        private Rule parseAssignment(IRegister dest, Queue<String> tokens)
        {
            List<String> UnaryOpTokens = new List<String>(new String[] {"min","union"});

            // if the first token is a UnaryOp, then parseUnaryOp
            if (UnaryOpTokens.Contains(tokens.Peek()))
                return parseUnaryOp(dest, tokens);
            else
                return parseAssignOp(dest, tokens);
        }

        private Rule parseUnaryOp(IRegister dest, Queue<String> tokens)
        {
            String op = tokens.Dequeue();
            if(op.Equals("min"))
                return parseUnaryOpMin(dest, tokens);
            else if (op.Equals("union"))
                return parseUnaryOpUnion(dest, tokens);
            throw new Exception("Unrecognized unary op: " + op);
        }

        private Rule parseAssignOp(IRegister dest, Queue<String> tokens)
        {
            IRegister rhs = parseRegister(tokens.Dequeue());
            parseStatementEnd(tokens);
            return new Rule(dest, new OpNoOp(), rhs);
        }

        private Rule parseUnaryOpMin(IRegister dest, Queue<String> tokens)
        {
            IRegister target = parseRegister(tokens.Dequeue());

            int index = aggRegisters.Count;
            AggRegister aggReg = new AggMinRegister(this, "agg_min_" + index, index, target);
            aggRegisters.Add(index, aggReg);

            Rule rule = new Rule(dest, new OpNoOp(), aggReg);

            parseStatementEnd(tokens);
            return rule;
        }

        private Rule parseUnaryOpUnion(IRegister dest, Queue<String> tokens)
        {
            IRegister target = parseRegister(tokens.Dequeue());

            int index = aggRegisters.Count;
            AggRegister aggReg = new AggUnionRegister(this, "agg_union_" + index, index, target);
            aggRegisters.Add(index, aggReg);

            Rule rule = new Rule(dest, new OpNoOp(), aggReg);

            parseStatementEnd(tokens);
            return rule;
        }

        private Rule parseBinaryOp(IRegister dest, Queue<String> tokens)
        {
            IRegister left = parseRegister(tokens.Dequeue());
            String op = tokens.Dequeue();
            IRegister right = parseRegister(tokens.Dequeue());
            parseStatementEnd(tokens);

            List<IRegister> args = new List<IRegister>();
            args.Add(left);
            args.Add(right);

            if (op.Equals("+"))
                return new Rule(dest, new OpAdd(), args);

            throw new Exception("Unrecognized binary op: " + op);
        }

        private void parseStatementEnd(Queue<String> tokens)
        {
            String endl = tokens.Dequeue();

            if (!endl.Equals("\n") && !endl.Equals(";"))
                throw new Exception("Expected end of statement, found '" + endl + "'");

            if (tokens.Count == 0) return;  // in case we're at the end

            endl = tokens.Peek();
            while (endl.Equals(";") || endl.Equals("\n"))
            {
                tokens.Dequeue();
                if (tokens.Count == 0) break;
                endl = tokens.Peek();
            }
        }

        private Rule parseDissemination(IRegister lhs, Queue<String> tokens)
        {
            IRegister target = parseRegister(tokens.Dequeue());

            int index = dissRegisters.Count;
            DissRegister dissReg = new DissRegister(this, "diss_" + index, index, target, _mostRecentRound);
            dissRegisters.Add(index, dissReg);

            Rule rule = new Rule(lhs, new OpNoOp(), dissReg);

            parseStatementEnd(tokens);
            return rule;
        }

        #endregion

        #region Event Processing

        void upper_endpoint_OnConnect()
        {
            processExternalEvents(null);
        }

        private void processEvent(int id, QS.Fx.Serialization.ISerializable o)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DataFlow " + this.debug_identifier + ": DataFlow.processEvent(): processing an event: (id, value) => (" +
                    id + "," + QS.Fx.Printing.Printable.ToString(o) + ")");
#endif

            lock (this)
            {
                IRegister currentRegister = registers[id];
                // This should trigger all Activated events
                // and set an initial list of activations.
                currentRegister.Value = o;
                if (_rootMR.IsActive)
                    _rootMR.Run();
            }
        }

        private void receiveExternalEvent(int id, long version, QS.Fx.Serialization.ISerializable value)
        {
            lock (this)
            {
                eventQueue.Enqueue(new ExternalEvent(id, version, value));
            }
        }

        private void processExternalEvents(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            lock (this)
            {
                while (eventQueue.Count > 0)
                {
                    ExternalEvent e = eventQueue.Dequeue();
                    processEvent(e.Id, e.Value);
                }
            }

            this._alarm = this._platform.AlarmClock.Schedule
            (
                1.0/2.0,
                new QS.Fx.Clock.AlarmCallback
                (
                    delegate(QS.Fx.Clock.IAlarm _alarm)
                    {
                        if ((this._alarm != null) && !this._alarm.Cancelled && ReferenceEquals(this._alarm, _alarm))
                            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this.processExternalEvents));
                    }
                ),
                null
            );
        }

        #endregion

        #region IDataFlow Members

        // This is for messages from my upper endpoint
        void QS.Fx.Interface.Classes.IDataFlow.Send(int id, long version, QS.Fx.Serialization.ISerializable value)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DataFlow " + this.debug_identifier + ": DataFlow.Send(): Recevied a message from my 'leaf' endpoint: (id, version, value) => (" +
                    id + "," + version + "," + QS.Fx.Printing.Printable.ToString(value) + ")");
#endif
            receiveExternalEvent(id, version, value);
        }

        #endregion

        #region IDataFlowClient Members

        // This is for messages from my lower endpoint
        void QS.Fx.Interface.Classes.IDataFlowClient.Send(int id, long version, QS.Fx.Serialization.ISerializable value)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DataFlow " + this.debug_identifier + ": DataFlowClient.Send(): Recevied a message from my 'root' endpoint: (id, version, value) => (" +
                    id + "," + version + "," + QS.Fx.Printing.Printable.ToString(value) + ")");
#endif
            receiveExternalEvent(id, version, value);
        }

        #endregion

        #region IDataFlowExposed Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IDataFlowClient, QS.Fx.Interface.Classes.IDataFlow> QS.Fx.Object.Classes.IDataFlowExposed<
            Value.Round_, QS.Fx.Serialization.ISerializable>.DataFlow
        {
            get { return upper_endpoint; }
        }

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IDataFlow, QS.Fx.Interface.Classes.IDataFlowClient> QS.Fx.Object.Classes.IDataFlowExposed<
            Value.Round_, QS.Fx.Serialization.ISerializable>.DataFlowClient
        {
            get { return lower_endpoint; }
        }

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IAggUpdater<Value.Round_, QS.Fx.Serialization.ISerializable>,
            QS.Fx.Interface.Classes.IAggUpdaterClient<Value.Round_, QS.Fx.Serialization.ISerializable>>
            QS.Fx.Object.Classes.IDataFlowExposed<Value.Round_, QS.Fx.Serialization.ISerializable>.AggUpdater
        {
            get { return agg_endpoint; }
        }

        #endregion

        #region IAggUpdaterClient Members

        // This is a dissemination message
        void QS.Fx.Interface.Classes.IAggUpdaterClient<Value.Round_, QS.Fx.Serialization.ISerializable>.update(Value.Round_ round, QS.Fx.Serialization.ISerializable msg)
        {
            lock (this)
            {
                _mostRecentRound = round;
                Value.DataFlowToken_ token = (Value.DataFlowToken_)msg;

                // If first call ever for this incarnation!
                if (token == null)
                {
                    this._rootIncarnation = round.Incarnation;
                    token = new Value.DataFlowToken_(
                        createBlankDisseminationMessage(), null);
                }

                // check the dissemination field
                Value.KVSetToken_ _diss = recordDisseminationMessage(token.Disseminate);


                _rootMR.Run();


                // If root
                if (this._rootIncarnation != null && this._rootIncarnation.Equals(round.Incarnation))
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("##DataFlow.update(): Got token, I am root.");
#endif

                    Value.KVSetToken_ aggs = new Value.KVSetToken_();

                    foreach (AggRegister register in aggRegisters.Values)
                    {
                        Value.KVToken_ entry =
                            new Value.KVToken_(
                                new QS.Fx.Base.Index(register.Id), round, register.Target.Value);
                        aggs.Add(entry);

#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("DataFlow " + this.debug_identifier + ": ##DataFlow.update(): Adding start agg entry: (id, value) => (" +
                                register.Id + "," + QS.Fx.Printing.Printable.ToString(register.Target.Value) + ")");
#endif
                    }

                    agg_endpoint.Interface.updated(round, new Value.DataFlowToken_(_diss, aggs));
                }
                else // not root
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("DataFlow " + this.debug_identifier + ": ##DataFlow.update(): Got token, I am NOT root.");
#endif

                    // append to the aggregation field
                    Value.KVSetToken_ rv = appendToAggregationMessage(token.Aggregate);

                    // Send on the new message
                    agg_endpoint.Interface.updated(round, new Value.DataFlowToken_(_diss, rv));
                } 
            }
        }

        private Value.KVSetToken_ createBlankDisseminationMessage()
        {
            Value.KVSetToken_ rv = new Value.KVSetToken_();

            foreach(DissRegister register in dissRegisters.Values)
            {
                Value.KVToken_ tok = new Value.KVToken_(
                    new QS.Fx.Base.Index(register.Id), register.Version, register.Target.Value);
                rv.Add(tok);
            }

            return rv;
        }

        private Value.KVSetToken_ recordDisseminationMessage(Value.KVSetToken_ msg)
        {
            Value.KVSetToken_ rv = new Value.KVSetToken_();

            foreach (Value.KVToken_ tok in msg.Tokens)
            {
                DissRegister register = dissRegisters[Int32.Parse(tok.Index.String)];

#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("DataFlow " + this.debug_identifier + ":  Disseminating a Value: mine(" +
                        register.Value + ") token(" + tok.Payload + ") \n\n My Incarnation: " +
                        QS.Fx.Printing.Printable.ToString(register.Version) + " \n Other: " +
                        QS.Fx.Printing.Printable.ToString(tok.Round));
#endif

                // If my value is newer, put it in the token
                if (register.Value != null &&
                    (register.Version.Incarnation.CompareTo(tok.Round.Incarnation) > 0 ||
                    (register.Version.Incarnation.CompareTo(tok.Round.Incarnation) == 0 &&
                     register.Version.Index.CompareTo(tok.Round.Index) >= 0)))
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("DataFlow " + this.debug_identifier + ": Disseminating a Value: My value is newer.");
#endif
                    Value.KVToken_ newtok = new Value.KVToken_(tok.Index, register.Version, register.Value);
                    rv.Add(newtok);
                }
                else
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("DataFlow " + this.debug_identifier + ": Disseminating a Value: Token is newer, saving...");
#endif
                    register.Value = tok.Payload;
                    register.Version = tok.Round;
                    rv.Add(tok);
                }
            }

            return rv;
        }

        private Value.KVSetToken_ appendToAggregationMessage(Value.KVSetToken_ msg)
        {
            Value.KVSetToken_ rv = new Value.KVSetToken_();

            foreach (Value.KVToken_ tok in msg.Tokens)
            {
                // TODO: Only contribute to this aggregation if the rule is "activated"
                AggRegister register = aggRegisters[Int32.Parse(tok.Index.String)];
                QS.Fx.Serialization.ISerializable result = register.performOp(tok.Payload);
                rv.Add(new Value.KVToken_(tok.Index, tok.Round, result));
            }
            return rv;
        }

        // This is asking me to perform the final computation
        void QS.Fx.Interface.Classes.IAggUpdaterClient<Value.Round_, QS.Fx.Serialization.ISerializable>.finalize(Value.Round_ round, QS.Fx.Serialization.ISerializable msg)
        {
            
            // Store the result of the aggregate operations into their local
            // temp variables.
            Value.DataFlowToken_ token = (Value.DataFlowToken_)msg;

            lock (this)
            {
                foreach (Value.KVToken_ tok in token.Aggregate.Tokens)
                {
                    AggRegister register = aggRegisters[Int32.Parse(tok.Index.String)];

                    // TODO: This should actually trigger an external event, instead of making the change now
                    // ??: should it be a single event for all of the agg registers combined?
                    register.Value = tok.Payload;
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("DataFlow " + this.debug_identifier + ": ##DataFlow.final(): Storing aggregate result (id, value) => " +
                            tok.Index.String + "," + QS.Fx.Printing.Printable.ToString(tok.Payload) + ")");
#endif
                }

                _rootMR.Run();
            }
        }

        #endregion
    }
}
