// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveTokenRelativeQuerySource.cs" company="Sitecore Corporation A/S">
//     © 2016 Sitecore Corporation A/S. All rights reserved.
// </copyright>
// <summary>
//     Defines the ResolveTokenRelativeQuerySource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetLookupSourceItems;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.XA.Foundation.TokenResolution;

namespace Sitecore.Support.XA.Foundation.TokenResolution.Pipelines.GetLookupSourceItems
{
    public class ResolveTokenRelativeQuerySource
    {
        public void Process(GetLookupSourceItemsArgs args)
        {
            Assert.IsNotNull(args, "args");
            if (!args.Source.StartsWith("query:"))
            {
                return;
            }
            Item contextItem = null;
            string url = WebUtil.GetQueryString();
            if (!string.IsNullOrWhiteSpace(url) && url.Contains("hdl"))
            {
                string currentItemId = FieldEditorOptions.Parse(new UrlString(url)).Parameters["contentitem"];
                if (!string.IsNullOrEmpty(currentItemId))
                {
                    ItemUri contentItemUri = new ItemUri(currentItemId);
                    contextItem = Database.GetItem(contentItemUri);
                }
            }

            if (contextItem == null)
            {
                contextItem = args.Item;
            }
            
            string query = TokenResolver.Resolve(args.Source, contextItem, true, false);
          //fix of 9582
            if (query == "query:")
            {
                //fix for 'Empty string is not allowed' exception while resolving query by Sitecore - #10531
                query = string.Empty;
            }
            args.Source = query;
        }
    }
}