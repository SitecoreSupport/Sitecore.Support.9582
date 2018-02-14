// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolvePresentationTokens.cs" company="Sitecore Corporation A/S">
//     © 2016 Sitecore Corporation A/S. All rights reserved.
// </copyright>
// <summary>
//     Defines the ResolvePresentationTokens type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Comparers;
using Sitecore.Data.Items;
using Sitecore.XA.Foundation.Multisite;
using Sitecore.XA.Foundation.Presentation;
using Sitecore.XA.Foundation.TokenResolution.Pipelines.ResolveTokens;

namespace Sitecore.Support.XA.Foundation.Presentation.Pipelines.ResolveTokens
{
    public class ResolvePresentationTokens : ResolveTokensProcessor
    {
        protected readonly IPresentationContext PresentationContext;
        protected readonly ISharedSitesContext SharedSitesContext;
        protected readonly IMultisiteContext MultisiteContext;

        public ResolvePresentationTokens(IPresentationContext presentationContext, ISharedSitesContext sharedSitesContext, IMultisiteContext multisiteContext)
        {
            PresentationContext = presentationContext;
            SharedSitesContext = sharedSitesContext;
            MultisiteContext = multisiteContext;
        }

        public override void Process(ResolveTokensArgs args)
        {
            var query = args.Query;
            var contextItem = args.ContextItem;
            var escapeSpaces = args.EscapeSpaces;

            query = ExpandTokenWithDynamicItemPaths(query, "$pageDesigns", () => InvokeForSiteNodes(contextItem, site => PresentationContext.GetPageDesignsItem(site)), escapeSpaces);
            query = ExpandTokenWithDynamicItemPaths(query, "$partialDesigns", () => InvokeForSiteNodes(contextItem, site => PresentationContext.GetPartialDesignsItem(site)), escapeSpaces);

            args.Query = query;
        }

        protected virtual IEnumerable<Item> InvokeForSiteNodes(Item contextItem, Func<Item, Item> selector)
        {
            var sharedSites = SharedSitesContext.GetSharedSites(contextItem).ToList();
            
            var siteItem = MultisiteContext.GetSiteItem(contextItem);
          //fix of 9582
           if (siteItem != null && sharedSites.All(i => i.ID != siteItem.ID))
            {
                sharedSites.Add(siteItem);
            }

            return sharedSites.Distinct(new ItemIdComparer()).Select(selector);
        }
    }
}