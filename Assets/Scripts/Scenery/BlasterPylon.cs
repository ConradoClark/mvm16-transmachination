﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BlasterPylon : RoomObject
{
    public override bool PerformReset()
    {
        return true;
    }

    public override void Initialize()
    {
    }

    public override bool Activate()
    {
        return true;
    }
}