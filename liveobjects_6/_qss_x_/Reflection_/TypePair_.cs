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

namespace QS._qss_x_.Reflection_
{
    public sealed class TypePair_ : IEquatable<TypePair_>, IComparable<TypePair_>, IComparable
    {
        #region Constructor

        public TypePair_(Type type1_, Type type2_)
        {
            this.type1_ = type1_;
            this.type2_ = type2_;
        }

        #endregion

        #region Fields

        private Type type1_, type2_;

        #endregion

        #region IEquatable<TypePair_> Members

        bool IEquatable<TypePair_>.Equals(TypePair_ other)
        {
            return type1_.Equals(other.type1_) && type2_.Equals(other.type2_);
        }

        #endregion

        #region IComparable<TypePair_> Members

        int IComparable<TypePair_>.CompareTo(TypePair_ other)
        {
            int result_;
            if (type1_.Equals(other.type1_))
            {
                if (type2_.Equals(other.type2_))
                {
                    result_ = 0;
                }
                else
                {
                    result_ = type2_.GetHashCode().CompareTo(other.type2_.GetHashCode());
                    if (result_ == 0)
                    {
                        result_ = type2_.FullName.CompareTo(other.type2_.FullName);
                        if (result_ == 0)
                        {
                            result_ = type2_.Assembly.FullName.CompareTo(other.type2_.Assembly.FullName);
                            if (result_ == 0)
                                throw new Exception("Two types are different even though they both have the same has codes, the same name \"" +
                                    type2_.FullName + "\", and come from the same assembly \"" + type2_.Assembly.FullName + "\".");
                        }
                    }
                }
            }
            else
            {
                result_ = type1_.GetHashCode().CompareTo(other.type1_.GetHashCode());
                if (result_ == 0)
                {
                    result_ = type1_.FullName.CompareTo(other.type1_.FullName);
                    if (result_ == 0)
                    {
                        result_ = type1_.Assembly.FullName.CompareTo(other.type1_.Assembly.FullName);
                        if (result_ == 0)
                            throw new Exception("Two types are different even though they both have the same has codes, the same name \"" +
                                type1_.FullName + "\", and come from the same assembly \"" + type1_.Assembly.FullName + "\".");
                    }
                }
            }
            return result_;
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            else if (obj is TypePair_)
                return ((IComparable<TypePair_>) this).CompareTo((TypePair_)obj);
            else
                throw new Exception("Cannot compare an object of type \"" + typeof(TypePair_).FullName +
                    "\" with an object of type \"" + obj.GetType().FullName + "\".");
        }

        #endregion

        #region Overridden from System.Object

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            else if (obj is TypePair_)
                return ((IEquatable<TypePair_>)this).Equals((TypePair_)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return type1_.GetHashCode() ^ type2_.GetHashCode();
        }

        public override string ToString()
        {
            return "{" + type1_.ToString() + ", " + type2_.ToString() + "}";
        }

        #endregion
    }
}
