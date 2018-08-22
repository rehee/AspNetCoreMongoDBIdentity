using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
  [BsonIgnoreExtraElements(ignoreExtraElements: true, Inherited = true)]
  public class MUser : IdentityUser
  {

  }
  [BsonIgnoreExtraElements(ignoreExtraElements: true, Inherited = true)]
  public class MRole : IdentityRole<string>
  {

  }
  [BsonIgnoreExtraElements(ignoreExtraElements: true, Inherited = true)]
  public class MUserRole : IdentityUserRole<string>
  {

  }
  

}
