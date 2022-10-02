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
            if (files != null && files.Length == 1)
            {
                return files[0];
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

        public static void cleanupTempDir()
        {
            if (Directory.Exists(_tempImageDir))
            {
                var dir = new DirectoryInfo(_tempImageDir);
                foreach (var file in dir.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (IOException) { }
                }
            }
        }
    }
}