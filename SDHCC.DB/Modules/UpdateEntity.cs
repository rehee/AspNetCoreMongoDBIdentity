﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.DB.Modules
{
  public class UpdateEntity<T>
  {
    public T Object { get; set; }
    public string Key { get; set; }
  }
}
