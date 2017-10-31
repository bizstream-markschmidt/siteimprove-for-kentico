using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.Helpers;
using Microsoft.Ajax.Utilities;

namespace Siteimprove.Utilities
{

    /// <summary>
    /// This is a class that I created for the sample code for easily creating a Siteimprove plugin. I only pulled a few methods from each utility class.
    /// In the REAL BizStream Toolkit version, these methods are broken out into MANY different logical classes.
    /// for simplicty sake, everything is in one big "common" file. Not the way to do it in real life.
    /// </summary>
    public static class CommonUtility
    {
        // Originally from VersionUtility
        public static string GetKenticoVersion(bool fullVersion = true)
        {
            string v = CMSVersion.GetVersion(true, true, true, true);
            if (fullVersion)
                return v;

            // if NOT fullVersion, trim down to 8.2 or 10.0, ...
            if (v.Contains('.'))
            {
                string[] numbers = v.Split('.');
                if (numbers.Length >= 2)
                    return string.Format("{0}.{1}", numbers[0], numbers[1]);
            }

            // if for some reson above does not work, do this
            return CMSVersion.MainVersion;
        }


        // Originally from WebClientUtility
        public static string Post(string url, string json, string authorizationHeaderValue = "")
        {
            string response;
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                if (!String.IsNullOrEmpty(authorizationHeaderValue))
                    client.Headers.Add(HttpRequestHeader.Authorization, authorizationHeaderValue);

                byte[] responsebytes = client.UploadData(url, "POST", Encoding.Default.GetBytes(json));

                response = Encoding.UTF8.GetString(responsebytes);

            }
            return response;
        }

        public static string Post(string url, NameValueCollection paramaters, string authorizationHeaderValue = "")
        {
            return Post(url, CommonUtility.ConvertToJson(paramaters), authorizationHeaderValue);
        }

        public static T PostJsonObject<T>(string url, string jsonToPost, string authorizationHeaderValue = "") where T : class
        {
            string json = Post(url, jsonToPost, authorizationHeaderValue);
            if (!String.IsNullOrEmpty(json))
            {
                return CommonUtility.GetObjectFromJson<T>(json);
            }

            return default(T);
        }

        public static string Get(string url, string authorizationHeaderValue = "")
        {
            string text;
            using (WebClient client = new WebClient())
            {
                if (!String.IsNullOrEmpty(authorizationHeaderValue))
                    client.Headers.Add(HttpRequestHeader.Authorization, authorizationHeaderValue);

                text = client.DownloadString(url);
            }
            return text;
        }


        // Originally from JsonUtility
        public static T GetObjectFromUrl<T>(string url, string authorizationHeaderValue = "") where T : class
        {
            string json = Get(url, authorizationHeaderValue);

            if (!String.IsNullOrEmpty(json))
            {
                return CommonUtility.GetObjectFromJson<T>(json);
            }

            return default(T);

        }

        public static T GetObjectFromJson<T>(string json) where T : class
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                return (T)serializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error Deserializing JSON. Core.Utilities.JsonUtility.GetObjectFromJson()<br/>{0}<br/><br/>{1}", json, ex.Message));
            }
        }


        public static string ConvertToJson(NameValueCollection paramaters)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            int x = 0;
            foreach (string key in paramaters)
            {
                if (x != 0) sb.Append(",");

                string value = paramaters[key];

                if (String.IsNullOrEmpty(value))
                    sb.AppendFormat("\"{0}\": null", key);
                else
                    sb.AppendFormat("\"{0}\": {1}", key, HttpUtility.JavaScriptStringEncode(value, true));

                x++;
            }
            sb.Append("}");

            string json = sb.ToString();

            // clean up any empty items
            json = json.Replace(",\"\": \"\"", "");

            return json;
        }

        // Originally from PathUtility
        /// <summary>
        /// Returns Relative Live Url following all of the Kentico Url/SEO Settings
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetLiveUrl(TreeNode node)
        {
            var pageUrl = node.IsLink ? DocumentURLProvider.GetUrl(node.NodeAliasPath) : DocumentURLProvider.GetUrl(node.NodeAliasPath, node.DocumentUrlPath);
            return URLHelper.ResolveUrl(pageUrl);
        }



        // Originally from HtmlUtility
        private static readonly Regex mCommentRegex = new Regex(@"/\*.+?\*/", RegexOptions.Multiline);


        //public static string AddJavascriptToEndOfHead(string originalHtml, string jsCode, bool minify = true, bool wrapWithScriptTag = false)
        //{
        //    jsCode = PrepareJavascript(jsCode, minify, wrapWithScriptTag);

        //    // assumption there is a lowercase </head> tag
        //    return originalHtml.Replace("</head>", jsCode + "</head>");
        //}

        public static string AddJavascriptToEndOfBody(string originalHtml, string jsCode, bool minify = true, bool wrapWithScriptTag = false)
        {

            jsCode = PrepareJavascript(jsCode, minify, wrapWithScriptTag);

            // assumption there is a lowercase </body> tag
            return originalHtml.Replace("</body>", jsCode + "</body>");
        }

        public static string PrepareJavascript(string jsCode, bool minify = true, bool wrapWithScriptTag = false)
        {
            // TODO: Cache

            if (minify)
                jsCode = MinifyJavascript(jsCode);

            if (wrapWithScriptTag)
                jsCode = WrapWithScriptTag(jsCode);

            return jsCode;
        }

        public static string WrapWithScriptTag(string jsCode)
        {
            return string.Format(@"<script type=""text/javascript"">{0}</script>", jsCode);
        }


        /// <summary>
        /// Minifies the specified JavaScript.
        /// </summary>
        /// <param name="resource">The JavaScript to minify.</param>
        /// <returns>The minified JavaScript, if minification was successful; otherwise, the original JavaScript with minification errors appended at the end.</returns>
        public static string MinifyJavascript(string resource)
        {
            if (String.IsNullOrEmpty(resource))
            {
                return resource;
            }

            var settings = new CodeSettings
            {
                AllowEmbeddedAspNetBlocks = false,
                EvalTreatment = EvalTreatment.MakeAllSafe
            };
            var minifier = new Minifier();
            try
            {
                resource = minifier.MinifyJavaScript(resource, settings);
            }
            catch
            {
                var minificationErrors = String.Join(Environment.NewLine, minifier.Errors);
                resource = AppendMinificationErrors(resource, minificationErrors);
            }

            return resource;
        }

        /// <summary>
        /// Appends minification errors at the end of the specified JavaScript as a comment.
        /// </summary>
        /// <param name="resource">The JavaScript to append minification errors to.</param>
        /// <param name="minificationErrors">Minification errors to append.</param>
        /// <returns>The specified JavaScript with minification errors appended at the end as a comment.</returns>
        private static string AppendMinificationErrors(string resource, string minificationErrors)
        {
            var builder = new StringBuilder(resource);
            var epilogue = mCommentRegex.Replace(minificationErrors, String.Empty);
            builder.AppendLine().AppendLine().AppendLine("/* Minification failed").AppendLine(epilogue).Append("*/");

            return builder.ToString();
        }

    }
}