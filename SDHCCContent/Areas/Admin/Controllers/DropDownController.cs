using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SDHCC;
using SDHCC.DB;
using SDHCC.DB.Content;
using SDHCCContent.Models.DropDowns;

namespace SDHCCContent.Areas.Admin.Controllers
{
  [Area("Admin")]
  public class DropDownController : Controller
  {
    ISDHCCDbContext db { get; set; }
    public DropDownController(ISDHCCDbContext db)
    {
      this.db = db;
    }
    public IActionResult Index()
    {
      var model = db.GetDropDownSummary();
      return View(model);
    }
    public IActionResult List(string id)
    {
      if (String.IsNullOrEmpty(id))
        return RedirectToAction("Index");
      ViewBag.name = id;

      var model = db.GetDropDownsByName(id).ToList();
      return View(model);
    }
    public IActionResult PreCreate(string type)
    {
      if (String.IsNullOrEmpty(type))
        return RedirectToAction("Index");
      var baseType = ContentE.RootDropDown.CustomAttributes.Where(b => b.AttributeType == typeof(AllowChildrenAttribute)).FirstOrDefault();
      if (baseType == null)
        return RedirectToAction("Index");
      var allowChild = baseType.NamedArguments.Where(b => b.MemberName == "ChildrenType").FirstOrDefault();
      if (allowChild == null)
        return RedirectToAction("Index");
      var allowChildName = ((System.Collections.ObjectModel.ReadOnlyCollection<System.Reflection.CustomAttributeTypedArgument>)allowChild.TypedValue.Value)
        .Select(b => b.Value as Type).Where(b => b.Name == type).FirstOrDefault();
      if (allowChild == null)
        return RedirectToAction("Index");
      ViewBag.name = type;
      var model = (DropDownBase)Activator.CreateInstance(allowChildName);

      return View("Create", model.ConvertToPassingModel());
    }
    [HttpPost]
    public IActionResult Create(DropDownPostModel model)
    {
      var dropdown = model.ConvertToBaseModel();
      db.AddDropDown(dropdown);
      return RedirectToAction("List", new { @area = "Admin", id = dropdown.GetType().Name });
    }
    public IActionResult Detail(string id, string name)
    {
      if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
        return RedirectToAction("Index");
      var model = db.Where<DropDownBase>(b => b.Id == id, name).FirstOrDefault();
      ViewBag.name = name;
      return View(model.ConvertToPassingModel());
    }
    [HttpPost]
    public IActionResult Detail(DropDownPostModel model)
    {
      var type = Type.GetType($"{model.FullType},{model.AssemblyName}");
      if (type == null)
        return RedirectToAction("Index");
      var dropDown = db.Where<DropDownBase>(b => b.Id == model.Id, type.Name).FirstOrDefault();
      if (dropDown == null)
        return RedirectToAction("Index");
      var dropDownPass = dropDown.ConvertToPassingModel();
      dropDownPass.Properties = model.Properties;
      dropDownPass.Name = model.Name;
      db.UpdateDropDown(dropDownPass.ConvertToBaseModel());

      return RedirectToAction("List", new { @area = "Admin", id = type.Name });
    }

  }
}