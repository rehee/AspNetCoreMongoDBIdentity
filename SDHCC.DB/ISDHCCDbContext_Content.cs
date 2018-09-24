using MongoDB.Bson;
using SDHCC.DB.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.DB
{
  public partial interface ISDHCCDbContext
  {
    void AddContent(ContentBase content);
    void RemoveContent(ContentBase content);
    void RemoveContent(string contentId);
    void MoveContent(ContentBase content, ContentBase target);
    void MoveContent(ContentBase content, string target);
    void MoveContent(string contentId, ContentBase target);
    void MoveContent(string contentId, string target);

    void UpdatePageContent(ContentBase content);
    ContentBase GetContent(string id);
    IEnumerable<ContentBase> GetContents(IEnumerable<string> ids);
    IEnumerable<BsonDocument> GetChildrenNode(string id);
    IEnumerable<ContentBase> GetChildrenContent(string id);
    void UpdateContent(ContentBase content, IEnumerable<string> ignoreKeys = null, IEnumerable<string> takeKeys = null);
    IEnumerable<ContentBase> GetBreadcrumb(string id);
  }
}
