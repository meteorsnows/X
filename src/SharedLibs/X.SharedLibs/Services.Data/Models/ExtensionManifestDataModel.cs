﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Services.Data
{
    public class ExtensionManifestDataModel : BaseDataModel, IDataModel, ISqliteBase
    {
        public int FoundInToolbarPositions { get; set; }
        public int LaunchInDockPositions { get; set; }
        public bool IsExtEnabled { get; set; }
        public string Index1 { get; set; }
    }
}
