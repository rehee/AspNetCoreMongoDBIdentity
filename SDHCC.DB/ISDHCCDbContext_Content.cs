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

    ContentBase GetContent(string id);
    IEnumerable<ContentBase> GetContents(IEnumerable<string> ids);
    IEnumerable<ContentBase> GetChildrenNode(string id);
    void UpdateContent(ContentBase content);
  }
}
