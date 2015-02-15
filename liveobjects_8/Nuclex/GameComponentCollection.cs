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
using System.Collections.ObjectModel;

#if XNA
using Microsoft.Xna.Framework;

// ************************************************************************* //
// Disable 'missing xml comment' warning for this file since it has been
// generated by .NET Reflector and I'm not going to comment this all
// just so I have to start from scratch again when XNA gets updated
#pragma warning disable 1591
// ************************************************************************* //

namespace Nuclex {

  public sealed class GameComponentCollection : Collection<IGameComponent> {
    // Events
    public event EventHandler<GameComponentCollectionEventArgs> ComponentAdded;

    public event EventHandler<GameComponentCollectionEventArgs> ComponentRemoved;

    // Methods
    internal GameComponentCollection() {
    }

    protected override void ClearItems() {
      for(int i = 0; i < base.Count; i++) {
        this.OnComponentRemoved(new GameComponentCollectionEventArgs(base[i]));
      }
      base.ClearItems();
    }

    protected override void InsertItem(int index, IGameComponent item) {
      if(base.IndexOf(item) != -1) {
        throw new ArgumentException("Resources.CannotAddSameComponentMultipleTimes");
      }
      base.InsertItem(index, item);
      if(item != null) {
        this.OnComponentAdded(new GameComponentCollectionEventArgs(item));
      }
    }

    private void OnComponentAdded(GameComponentCollectionEventArgs eventArgs) {
      if(this.ComponentAdded != null) {
        this.ComponentAdded(this, eventArgs);
      }
    }

    private void OnComponentRemoved(GameComponentCollectionEventArgs eventArgs) {
      if(this.ComponentRemoved != null) {
        this.ComponentRemoved(this, eventArgs);
      }
    }

    protected override void RemoveItem(int index) {
      IGameComponent gameComponent = base[index];
      base.RemoveItem(index);
      if(gameComponent != null) {
        this.OnComponentRemoved(new GameComponentCollectionEventArgs(gameComponent));
      }
    }

    protected override void SetItem(int index, IGameComponent item) {
      throw new NotSupportedException("Resources.CannotSetItemsIntoGameComponentCollection");
    }
  }

} // namespace Nuclex

#endif