﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace X.ModernDesktop.SimTower.Models.Item
{
  class Floor : Item, IPrototype
  {
    public Floor() { }
    public Floor(Board board) { _board = board; }

    public string Id => "floor";

    public string Name => "Floor";

    public int Price => 500;
    
    private Slot _Size = new Slot(1, 1);
    public Slot Size { get => _Size; set => SetDataAndMarkDirty(ref _Size, value); }
    
    public int Icon => (int)IconNumbers.ICON_FLOOR;

    public int EntranceOffset { get => _EntranceOffset; set => _EntranceOffset = value; }
    public int ExitOffset { get => _ExitOffset; set => _ExitOffset = value; }

    public int MaxInstancesPerFloor => 1;

    public bool KeepGrowingSameInstanceX => true;
    public bool KeepGrowingSameInstanceY => false;

    public IPrototype Make(Board board)
    {
      return new Floor(board); 
    }

    public UIElement MakeUI()
    {
      return new Views.Floor();
    }

    public void FirstTimeDraw()
    {

    }

    public void AddToItem(IPrototype itemToAdd)
    {

    }
  }
}
