﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Services.Data
{
    public class SketchPageDataModel: BaseDataModel, IDataModel, ISqliteBase
    {
        public string Index1 { get; set; }

        public string Title { get; set; }
        public string Abstract { get; set; }
    }
}
