using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Diagnostics;
using Sitecore.Support.XA.Foundation.SitecoreExtensions.Controls;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.CustomFields.FieldTypes
{
  public class SupportMultiRootTreeList: Sitecore.XA.Foundation.SitecoreExtensions.CustomFields.FieldTypes.MultiRootTreeList
  {
    protected override void OnLoad(EventArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      base.OnLoad(args);

      if (!Sitecore.Context.ClientPage.IsEvent)
      {
        // find the existing TreeviewEx that the base OnLoad added, get a ref to its parent, and remove it from controls
        var existingTreeView = (TreeviewEx)WebUtil.FindControlOfType(this, typeof(TreeviewEx));
        var treeviewParent = existingTreeView.Parent;

        existingTreeView.Parent.Controls.Clear(); // remove stock treeviewex, we replace with multiroot

        // find the existing DataContext that the base OnLoad added, get a ref to its parent, and remove it from controls
        
        var dataContext = (DataContext)WebUtil.FindControlOfType(this, typeof(DataContext));
        if (dataContext != null)
        {
          var dataContextParent = dataContext.Parent;

          dataContextParent.Controls.Remove(dataContext); // remove stock datacontext, we parse our own

          // create our MultiRootTreeview to replace the TreeviewEx
          var impostor = new DatasourceMultiRootTreeview();
          impostor.ID = existingTreeView.ID;
          impostor.DblClick = existingTreeView.DblClick;
          impostor.Enabled = existingTreeView.Enabled;
          impostor.DisplayFieldName = existingTreeView.DisplayFieldName;

          // parse the data source and create appropriate data contexts out of it
          var dataContexts = ParseDataContexts(dataContext);

          impostor.DataContext = string.Join("|", dataContexts.Select(x => x.ID));
          foreach (var context in dataContexts) dataContextParent.Controls.Add(context);

          // inject our replaced control where the TreeviewEx originally was
          treeviewParent.Controls.Add(impostor);
        }
        
      }
    }
  }
}