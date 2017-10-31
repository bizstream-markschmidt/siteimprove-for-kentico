using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.OutputFilter;
using CMS.PortalEngine;
using CMS.UIControls;
using Siteimprove.Initializers;
using Siteimprove.Utilities;

/// <summary>
/// Summary description for SiteimprovePlugin
/// </summary>
[assembly: RegisterModule(typeof(SiteimproveInitializationModule))]

namespace Siteimprove.Initializers
{
    public class SiteimproveInitializationModule : Module
    {
        // https://docs.kentico.com/k10/custom-development/handling-global-events

        public SiteimproveInitializationModule()
            : base("SiteimproveInitializationModule")
        {
        }

        // Initializes the module. Called when the application starts.
        protected override void OnInit()
        {
            base.OnInit();

            // Document Change Events
            WorkflowEvents.Publish.After += Publish_After;
            DocumentEvents.Insert.After += Insert_After;
            DocumentEvents.Update.After += Update_After;
            DocumentEvents.Move.Before += Move_Before;
            DocumentEvents.Delete.After += Delete_After;


            // Inject JS into the CMS Desk UI
            CMSPage.OnAfterPageLoad += CMSPage_OnAfterPageLoad;

            // Inject JS into the "Page" tab (ViewModeEnum.Edit) and "On-Site Editng" (ViewModeEnum.EditLive), needs OutputFilter
            RequestEvents.PostMapRequestHandler.Execute += PostMapRequestHandler_Execute;

        }

        private void PostMapRequestHandler_Execute(object sender, EventArgs e)
        {
            ResponseOutputFilter.EnsureOutputFilter();
            OutputFilterContext.CurrentFilter.OnAfterFiltering += CurrentFilter_OnAfterFiltering;
        }

        private void CurrentFilter_OnAfterFiltering(ResponseOutputFilter filter, ref string finalHtml)
        {
            // if it is the live site, we are bailing. Quick Bail, I do not want to affect performance of the live site
            if (PortalContext.ViewMode == ViewModeEnum.LiveSite)
                return;


            bool injectJs = false;
            if (PortalContext.ViewMode == ViewModeEnum.EditLive)
                // On Site Editing
                injectJs = SettingsUtility.GetIsEnabledOnOnSiteEditing();
            else if (PortalContext.ViewMode == ViewModeEnum.Edit)
                // Assume "Page" tab
                injectJs = SettingsUtility.GetIsEnabledOnPage();
            else
                return; // bail on all other ViewModes


            if (!injectJs)
                return;

            // add script to bottom of body
            var jsCode = SiteimproveUtility.GetJavascriptBigBoxSmart(DocumentContext.CurrentDocument);

            finalHtml = CommonUtility.AddJavascriptToEndOfBody(finalHtml, jsCode, false);
        }

        private void Delete_After(object sender, DocumentEventArgs e)
        {
            // If they delete a page, just run a recheck, and Siteimprove will figure out that it does not exists. There is NOT a special call to make for a delete
            SiteimproveUtility.RecheckNode(e.Node);
        }

        private void Move_Before(object sender, DocumentEventArgs e)
        {
            // run a check on the old location (deleted page)
            SiteimproveUtility.RecheckNode(e.Node, true);
        }

        private void Update_After(object sender, DocumentEventArgs e)
        {
            if (e.Node.IsInPublishStep)
                SiteimproveUtility.RecheckNode(e.Node);
        }

        private void Insert_After(object sender, DocumentEventArgs e)
        {
            if (e.Node.IsInPublishStep)
                SiteimproveUtility.RecheckNode(e.Node);
        }

        private void Publish_After(object sender, WorkflowEventArgs e)
        {
            SiteimproveUtility.RecheckNode(e.PublishedDocument);
        }

        private void CMSPage_OnAfterPageLoad(object sender, EventArgs e)
        {
            InjectSiteimproveScriptIntoCmsDeskPages((CMSPage)sender);
        }

        private void InjectSiteimproveScriptIntoCmsDeskPages(CMSPage cmsPage)
        {
            // Looking for a more elgeant solution. I am not a fan of all of these Contains/StartsWith TypeName string stuff


            // bail for Delete page, I'd like to do this a better way
            // This MUST go First, if you attempt to access a property (like DocumentID), it errors hard, even with try catch
            if (cmsPage.TypeName.Contains("_cmsdesk_delete_"))
                return;

            // For the "Page" Tab, this is == 0, which is not what we want, but it is helping us ignore a bunch of other situations
            if (cmsPage.DocumentID == 0)
                return;

            // if already registered, bail
            if (SiteimproveUtility.IsJavascriptAlreadyRegistered())
                return;

            bool overlayIsEnabled = false;

            // this Contains() check is NOT ideal... Looking for a more elgeant solution, based on support talks and documentation, nothing seems to exist
            if (PortalContext.ViewMode == ViewModeEnum.Preview)
                overlayIsEnabled = SettingsUtility.GetIsEnabledOnPreview();
            else if (cmsPage.TypeName.Contains("cmsmodules_content_cmsdesk_properties_"))
                overlayIsEnabled = SettingsUtility.GetIsEnabledOnPropertyTabs();
            else if (PortalContext.ViewMode == ViewModeEnum.EditForm && cmsPage.TypeName.Contains("_cmsdesk_edit_edit_"))
                overlayIsEnabled = SettingsUtility.GetIsEnabledOnForm();
            else
                return;

            if (!overlayIsEnabled)
                return;

            SiteimproveUtility.RegisterJavascriptBigBoxSmart(cmsPage, cmsPage.DocumentID, false);
        }

    }
}