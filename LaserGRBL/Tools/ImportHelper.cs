using LaserGRBL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tools
{
    public static class ImportHelper
    {
        private static readonly string _tempImageDir = Path.Combine(System.IO.Path.GetTempPath(), "LaserGRBL", "imgs");

        public static string GetFileFromDropEvent(IDataObject dataObject)
        {
            string[] files = (string[])dataObject.GetData(DataFormats.FileDrop);
            string htmlData = (string)dataObject.GetData(DataFormats.Html);

            if (files != null && files.Length == 1)
            {
                return files[0];
            }
            else if (htmlData.Length > 0)
            {
                try
                {
                    Match htmlMatches = Regex.Match(htmlData, "src=\\\"(.*?)\\\"");
                    string fileUrl = htmlMatches.Groups[1].Value;
                    if (!fileUrl.StartsWith("data"))
                    {
                        return null;
                    }
                    string filename = createTempFilenameFromUrl(fileUrl);
;
                    if (!Directory.Exists(_tempImageDir)) Directory.CreateDirectory(_tempImageDir);

                    string absolutePath = Path.Combine(_tempImageDir, filename);
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(new Uri(fileUrl), absolutePath);
                        var headerFilename = findFilenameInHeaders(client.ResponseHeaders, filename);
                        if (headerFilename.Length > 0)
                        {
                            var newPath = Path.Combine(_tempImageDir, headerFilename);
                            File.Move(absolutePath, newPath);
                            absolutePath = newPath;
                        }
                        return absolutePath;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException("Drop-Import (HTML)", ex);
                }
            }

            return null;
        }

        public static string GetImageFileFromClipboard()
        {
            Image img = Clipboard.GetImage();
            if (img == null)
            {
                return null;
            }

            if (!Directory.Exists(_tempImageDir)) Directory.CreateDirectory(_tempImageDir);
            string path = Path.Combine(_tempImageDir, DateTime.Now.ToString("yyyyMMddHHmmssffff"));
            path += ".png";
            img.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            return path;
        }

        private static string findFilenameInHeaders(WebHeaderCollection responseHeaders, string filename)
        {
            var contentDispo = responseHeaders["content-disposition"];
            if (contentDispo != null && contentDispo.Contains("filename="))
            {
                Match match = Regex.Match(contentDispo, "filename=\\\"(.*?)\\\"(;| |$)");
                if (match.Success)
                {
                    return match.Groups[1].ToString();
                }
            }
            return "";
        }

        private static string createTempFilenameFromUrl(string fileUrl)
        {
            string filename = Path.GetFileNameWithoutExtension(fileUrl);
            filename += DateTime.Now.ToString("yyyyMMddHHmmssffff");

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c.ToString(), "");
            }
            if (Path.HasExtension(fileUrl))
            {
                filename += "." + Path.GetExtension(fileUrl);
            }
            return filename;
        }

    }
}
