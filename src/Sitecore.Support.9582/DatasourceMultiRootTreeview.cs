// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatasourceMultiRootTreeview.cs" company="Sitecore Corporation A/S">
//     © 2017 Sitecore Corporation A/S. All rights reserved.
// </copyright>
// <summary>
//     Defines the DatasourceMultiRootTreeview type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Text;
using System.Web.UI;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.XA.Foundation.SitecoreExtensions.Controls;
using Sitecore.XA.Foundation.SitecoreExtensions.Services;

namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.Controls
{
    public class DatasourceMultiRootTreeview : SourcedMultiRootTreeview
    {
        protected virtual string InitialDataContext { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (this.Page.ClientScript.IsClientScriptIncludeRegistered("DatasourceMultiRootTreeview"))
                return;
            this.Page.ClientScript.RegisterClientScriptInclude("DatasourceMultiRootTreeview", "/sitecore/shell/Controls/DatasourceMultiRootTreeview/DatasourceMultiRootTreeview.js");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (CurrentDataContext.Equals(string.Empty))
            {
                //fix of 9582
                DataContext context = GetDataContext();
                if (context != null)
                {
                    InitialDataContext = context.ID;
                }
                //end of fix
            }
            else
            {
                InitialDataContext = CurrentDataContext;
            }
            
        }

        protected override void RenderNodeBegin(HtmlTextWriter output, IDataView dataView, string filter, Item item, bool active, bool isExpanded)
        {
            base.RenderNodeBegin(output, dataView, filter, item, active,isExpanded);

            if (active && InitialDataContext.Equals(CurrentDataContext))
            {
                var commandString = new StringBuilder();
                commandString.Append("<span class=\"treeNodeCommnds\">");
                var commandsService = ServiceLocator.ServiceProvider.GetService<IRenderingDatasourceCommandService>();
                if (commandsService == null)
                {
                    return;
                }

                var commands = commandsService.GetCommands(item);
                
                foreach (var command in commands)
                {
                    commandString.Append(command);
                }
                commandString.Append("</span>");

                output.Write(commandString.ToString());
            }
        }
    }
}