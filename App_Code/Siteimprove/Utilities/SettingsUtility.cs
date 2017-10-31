using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMS.DataEngine;
using CMS.SiteProvider;

namespace Siteimprove.Utilities
{
    // TODO: You COULD Utilize Kentico's Settings API to give the editor more control on how this plugin functions
    // For demo purposes, this just returns static values
    public static class SettingsUtility
    {
        // All settings should be Site Based

        public static bool GetIsEnabledOnPage()
        {
            return true;
            //return SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".BTKPluginSiteimproveEnabledOnPage");
        }

        public static bool GetIsEnabledOnForm()
        {
            return true;
            //return SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".BTKPluginSiteimproveEnabledOnForm");
        }

        public static bool GetIsEnabledOnPropertyTabs()
        {
            return true;
            //return SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".BTKPluginSiteimproveEnabledOnPropertyTabs");
        }

        public static bool GetIsEnabledOnPreview()
        {
            return true;
            //return SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".BTKPluginSiteimproveEnabledOnPreview");
        }

        internal static bool GetIsEnabledOnOnSiteEditing()
        {
            return true;
            //return SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".BTKPluginSiteimproveEnabledOnOnSiteEditing");
        }

        public static bool GetIsClearCacheOnSave()
        {
            return true;
            //return SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".BTKPluginSiteimproveClearNodeCache");
        }

        public static string GetAuthenticationToken()
        {
            return SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".BTKPluginSiteimproveToken");
        }

        public static void SetAuthenticationToken(string value)
        {
#if K8
            SettingsKeyInfoProvider.SetValue(SiteContext.CurrentSiteName + ".BTKPluginSiteimproveToken", value);
#else
            SettingsKeyInfoProvider.SetValue("BTKPluginSiteimproveToken", SiteContext.CurrentSiteName, value);
#endif
        }

        public static string GetWebsiteUrl()
        {
            return ""; // this will force it to use the current domain. if it is different (maybe a development site, then you need to specify the url that is registered with Siteimprove
            //return SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".BTKPluginSiteimproveWebsiteUrl");
        }

    }
}
