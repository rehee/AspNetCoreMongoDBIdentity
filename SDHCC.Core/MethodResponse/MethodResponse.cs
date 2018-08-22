using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.Core.MethodResponse
{
  public class MethodResponse
  {
    public int ResponseCode { get; set; }
    public bool Success { get; set; } = false;
    public string ResponseMessage { get; set; } = "";
    public string Message { get; set; } = "";
    public object ResponseObject { get; set; } = null;

  }
}
