using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.DB
{
  public enum CompareOption
  {
    In = 0,
    Lt = 1,
    Lte = 2,
    Mod = 3,
    Ne = 4,
    Near = 5,
    NearSphere = 6,
    Nin = 7,
    Not = 8,
    OfType = 9,
    Or = 10,
    Regex = 11,
    Size = 12,
    SizeGt = 13,
    SizeGte = 14,
    SizeLt = 15,
    SizeLte = 16,
    Text = 17,
    Type = 18,
    Where = 19,
    Eq = 20,
  }
}
