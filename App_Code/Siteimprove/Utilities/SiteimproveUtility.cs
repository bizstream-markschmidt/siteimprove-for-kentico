using System;
using System.Web.UI;
using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using Siteimprove.Enums;
using Siteimprove.Models;

namespace Siteimprove.Utilities
{
    public static class SiteimproveUtility
    {
        private const string TokenUrl = "https://my2.siteimprove.com/auth/token";
        private const string ApiUrl = "https://api-gateway.siteimprove.com";

        private const string JavascriptIncludeUrl = "https://cdn.siteimprove.net/cms/overlay.js";
        private const string JavascriptRegisterKey = "SiteimproveInitPage";

        /// <summary>
        /// 
        /// </summary>
        /// <returns>something like: "Kentico-v8.2" or "Kentico-v11.0"</returns>
        private static string GetCmsNameAndVersion()
        {
            var v = CommonUtility.GetKenticoVersion(false);
            return string.Format("Kentico-v{0}", v);
        }

        private static string GetAuthToken(SiteInfo siteInfo)
        {

            string token = SettingsUtility.GetAuthenticationToken();
            if (!String.IsNullOrEmpty(token))
                return token;

            // If the setting was empty, then we need to get a NEW token and save it
            // get token: https://my2.siteimprove.com/auth/token?cms=Kentico-v8.2

            var url = string.Format("{0}?cms={1}", TokenUrl, GetCmsNameAndVersion());

            var authenticationToken = CommonUtility.GetObjectFromUrl<AuthenticationToken>(url);
            token = authenticationToken.token;

            // Save it to the Site Settings
            SettingsUtility.SetAuthenticationToken(token);

            return token;
        }

        private static string GetSiteRoot(SiteInfo siteInfo)
        {
            var siteRoot = SettingsUtility.GetWebsiteUrl();

            // if setting is empty, get Current url root of site
            if (String.IsNullOrEmpty(siteRoot))
                siteRoot = URLHelper.GetAbsoluteUrl("~/");

            // chop "/"
            if (siteRoot.EndsWith("/"))
                siteRoot = siteRoot.TrimEnd('/');

            return siteRoot;
        }

        public static void RecheckNode(TreeNode node, bool delayIt = false)
        {
            // Clear the output cache WITHOUT the children (if it is a "recrawl site", we may need the children cleared
            if (SettingsUtility.GetIsClearCacheOnSave())
                node.ClearOutputCache(true, false);

            // delay it after clearing cache. This is for thigns like a MOVE
            if (delayIt)
            {
                System.Threading.Timer timer = null;
                timer = new System.Threading.Timer((obj) =>
                {
                    PostToSiteimproveServer(node, PushType.recheck);
                    timer.Dispose();
                },
                    null, 5000, System.Threading.Timeout.Infinite);


            }

            PostToSiteimproveServer(node, PushType.recheck);
        }

        public static bool PostToSiteimproveServer(TreeNode node, PushType pushType)
        {
            var pp = new PushParameters();

            pp.url = GetFullUrl(node);

            // if it is a full site recrawl, get the root
            if (pushType == PushType.recrawl)
                pp.url = GetSiteRoot(node.Site);

            pp.token = GetAuthToken(node.Site);
            pp.type = pushType.ToString();

            var paramaters = new System.Collections.Specialized.NameValueCollection();
            // TODO: Map it automatically
            paramaters.Add("url", pp.url);
            paramaters.Add("token", pp.token);
            paramaters.Add("type", pp.type);

            // TODO: Error handling

            var response = CommonUtility.Post(ApiUrl + "/cms-recheck", paramaters);

            if (response == @"{ ""ordered"": true }")
                return true;
            else
                throw new Exception("Siteimprove Error: " + response);

        }

        public static string GetFullUrl(CMS.DocumentEngine.TreeNode node)
        {
            var pageUrl = CommonUtility.GetLiveUrl(node);

            pageUrl = GetSiteRoot(node.Site) + pageUrl;

            return pageUrl;
        }

        /// <summary>
        /// Includes https://cdn.siteimprove.net/cms/overlay.js
        /// </summary>
        /// <param name="page"></param>
        private static void JavascriptInclude(Page page)
        {
            ScriptManager.RegisterClientScriptInclude(page, typeof(string), "SiteimproveInclude", JavascriptIncludeUrl);
        }


        /// <summary>
        /// Runs either JavascriptBigBoxSite or JavascriptBigBoxPage based on if we are at the ROOT node in the tree or not
        /// </summary>
        /// <param name="page"></param>
        /// <param name="documentId"></param>
        public static void RegisterJavascriptBigBoxSmart(Page page, int documentId, bool autoShowBox = true)
        {
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
            var node = DocumentHelper.GetDocument(documentId, tree);

            if (node == null)
                return;

            RegisterJavascriptBigBoxSmart(page, node, autoShowBox);
        }

        private static bool IsNodeRoot(TreeNode node)
        {
            // TODO: is there a better way? NodeLevel = 0?
            return (node.ClassName.ToLower() == "cms.root" || node.NodeLevel == 0);
        }

        /// <summary>
        /// Registers Code Block for either JavascriptBigBoxSite or JavascriptBigBoxPage based on if we are at the ROOT node in the tree or not
        /// </summary>
        /// <param name="page"></param>
        /// <param name="node"></param>
        public static void RegisterJavascriptBigBoxSmart(Page page, TreeNode node, bool autoShowBox = true)
        {
            if (IsNodeRoot(node))
                RegisterJavascriptBigBoxSite(page, node.Site, autoShowBox);
            else
                RegisterJavascriptBigBoxPage(page, node, autoShowBox);
        }

        /// <summary>
        /// Returns a string of javascript code for either JavascriptBigBoxSite or JavascriptBigBoxPage based on if we are at the ROOT node in the tree or not
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetJavascriptBigBoxSmart(TreeNode node)
        {
            if (IsNodeRoot(node))
                return GetJavascriptBigBoxSite(node.Site);
            else
                return GetJavascriptBigBoxPage(node);
        }

        private static void RegisterJavascriptBigBoxSite(Page page, SiteInfo siteinfo, bool autoShowBox = true)
        {
            var domain = GetSiteRoot(siteinfo);

            JavascriptInclude(page);
            JavascriptRegisterPush(page, siteinfo, domain, "domain", autoShowBox);
        }

        private static void RegisterJavascriptBigBoxPage(Page page, TreeNode node, bool autoShowBox = true)
        {
            var url = GetFullUrl(node);

            JavascriptInclude(page);
            JavascriptRegisterPush(page, node.Site, url, "input", autoShowBox);
        }

        private static string GetJavascriptInclude()
        {
            return string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>", JavascriptIncludeUrl);
        }

        private static string GetJavascriptBigBoxSite(SiteInfo siteinfo)
        {
            var domain = GetSiteRoot(siteinfo);

            return GetJavascriptInclude() + GetPushJavascript(domain, SiteContext.CurrentSite, "domain", false);
        }

        private static string GetJavascriptBigBoxPage(TreeNode node)
        {
            var url = GetFullUrl(node);

            return GetJavascriptInclude() + GetPushJavascript(url, SiteContext.CurrentSite, "input", false);
        }

        private static string GetPushJavascript(string url, SiteInfo siteInfo, string command, bool autoShowBox = true)
        {
            var pageUrlSafe = url.Replace("\\", "\\\\")
                .Replace("'", "\\'");

            var token = GetAuthToken(siteInfo);

            var autoShowJs = "";
            if (autoShowBox)
            {
                autoShowJs = @"
                /* wrapping in a try just incase Siteimprove changed their DOM */
                try {
                    /* force open */
                    $cmsj('.si-boxes-container .si-button:first').trigger('click');

                    /* force to side */
                    $cmsj('.si-pos-bottom').removeClass('si-pos-bottom').addClass('si-pos-side');

                    var setToPosition = 'si-pos-west-south';

                    /* force to left */
                    $cmsj('.si-pos-side')
                        .removeClass('si-pos-south')
                        .removeClass('si-pos-west')
                        .removeClass('si-pos-west-south')
                        .removeClass('si-pos-east')
                        .removeClass('si-pos-east-south')
                        .addClass(setToPosition);
                } catch( e ) { /* error quietly, do nothing */ } ";

            }

            return @"<script type=""text/javascript"">" +
            @"var _si = window._si || [];" +
            @"_si.push(['" + command + @"', '" + pageUrlSafe + @"', '" + token + @"', function ()" +
            @"{" +
            autoShowJs +
            @"}])" +
            @"</script>";

        }

        public static bool IsJavascriptAlreadyRegistered()
        {
            return ScriptHelper.IsClientScriptBlockRegistered(JavascriptRegisterKey);
        }

        private static void JavascriptRegisterPush(Page page, SiteInfo siteInfo, string url, string command, bool autoShowBox = true)
        {
            var js = GetPushJavascript(url, siteInfo, command, autoShowBox);

            ScriptManager.RegisterClientScriptBlock(page, typeof(string), JavascriptRegisterKey, js, false);
        }

    }
}