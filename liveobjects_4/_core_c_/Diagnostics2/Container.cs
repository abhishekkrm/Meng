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
using System.Text;

namespace QS._core_c_.Diagnostics2
{
    public sealed class Container : Component, IContainer, IComponent
    {
        public Container()
        {
        }

        private IComponent[] subcomponents = new IComponent[0];

        #region IContainer Members

        void IContainer.Register(string name, IComponent subcomponent)
        {
            ((IContainer)this).Register(name, subcomponent, RegisteringMode.Override); // FIX: we will use override as a default to reduce the impact of incmplete cleanup
        }

        void IContainer.Register(string name, IComponent subcomponent, RegisteringMode mode)
        {
            Synchronization.NonblockingLock.Lock(nonblockingLock, subcomponent.Lock);
            try
            {
                if (subcomponent.Container != null)
                {
                    if (!ReferenceEquals(subcomponent.Container, this))
                        throw new Exception("Subcomponent already registered with another container : " + subcomponent.Container.Path);
                    else
                    {
                        if ((mode & RegisteringMode.Reregister) != RegisteringMode.Reregister)
                            throw new Exception("Subcomponent already registered with this container.");
                    }
                }
                else
                {
                    bool ignore = false;

                    for (int ind = 0; ind < subcomponents.Length; ind++)
                    {
                        IComponent c = subcomponents[ind];

                        if (ReferenceEquals(c, subcomponent))
                        {
                            if ((mode & RegisteringMode.Reregister) != RegisteringMode.Reregister)
                                throw new Exception("Subcomponent being added already exists.");
                            else
                            {
                                ignore = true;
                                break;
                            }
                        }

                        if (c.Name.Equals(name))
                        {
                            if ((mode & RegisteringMode.Override) != RegisteringMode.Override)
                                throw new Exception("Another subcomponent with the same name \"" + name + "\" already exists in this container.");
                            else
                            {
                                subcomponent.Name = name;
                                subcomponent.Container = this;
                                subcomponents[ind].Container = null; // TODO: this change may require more careful handling
                                subcomponents[ind] = subcomponent;

                                ignore = true;
                                break;
                            }
                        }
                    }

                    if (!ignore)
                    {
                        subcomponent.Name = name;
                        subcomponent.Container = this;

                        IComponent[] new_subcomponents = new IComponent[subcomponents.Length + 1];
                        subcomponents.CopyTo(new_subcomponents, 0);
                        new_subcomponents[subcomponents.Length] = subcomponent;
                        subcomponents = new_subcomponents;
                    }
                }
            }
            finally
            {
                Synchronization.NonblockingLock.Unlock(nonblockingLock, subcomponent.Lock);
            }
        }

        void IContainer.Unregister(IComponent subcomponent)
        {
            Synchronization.NonblockingLock.Lock(nonblockingLock, subcomponent.Lock);
            try
            {
                bool subcomponent_found = false;

                for (int ind = 0; ind < subcomponents.Length; ind++)
                {
                    IComponent c = subcomponents[ind];
                    if (ReferenceEquals(c, subcomponent))
                    {
                        IComponent[] new_subcomponents = new IComponent[subcomponents.Length - 1];
                        for (int i = 0; i < ind; i++)
                            new_subcomponents[i] = subcomponents[i];
                        for (int i = (ind + 1); i < subcomponents.Length; i++)
                            new_subcomponents[i - 1] = subcomponents[i];
                        subcomponents = new_subcomponents;

                        subcomponent.Container = null;
                        subcomponent.Name = null;

                        subcomponent_found = true;
                        break;
                    }
                }

                if (!subcomponent_found)
                    throw new Exception("Subcomponent being removed does not exist.");
            }
            finally
            {
                Synchronization.NonblockingLock.Unlock(nonblockingLock, subcomponent.Lock);
            }
        }

        IComponent[] IContainer.Subcomponents
        {
            get { return subcomponents; }
        }

        #endregion

        #region IComponent Members

        ComponentClass IComponent.Class
        {
            get { return ComponentClass.Container; }
        }

        #endregion
    }
}
