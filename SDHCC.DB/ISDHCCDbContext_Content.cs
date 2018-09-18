using SDHCC.DB.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.DB
{
  public partial interface ISDHCCDbContext
  {
    void AddContent(ContentPassingModel content);
  }
}
