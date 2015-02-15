/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/

using System;
using System.Collections.Generic;

#if XNA
using Microsoft.Xna.Framework;

namespace Nuclex {

  internal class DrawOrderComparer : IComparer<IDrawable> {
    // Fields
    public static readonly DrawOrderComparer Default = new DrawOrderComparer();

    // Methods
    public int Compare(IDrawable x, IDrawable y) {
      if((x == null) && (y == null)) {
        return 0;
      }
      if(x != null) {
        if(y == null) {
          return -1;
        }
        if(x.Equals(y)) {
          return 0;
        }
        if(x.DrawOrder < y.DrawOrder) {
          return -1;
        }
      }
      return 1;
    }
  }

} // namespace Nuclex

#endif