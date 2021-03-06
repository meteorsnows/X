﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace CoreLib.Extensions
{
    public class ExtensionManifest: IExtensionManifest
    {
        

        public ExtensionManifest(string title, string iconUrl, string publisher, string version, string description, ExtensionInToolbarPositions iconPosition, ExtensionInToolbarPositions panelPosition)
        {
            
            Title = title;
            IconUrl = iconUrl;
            Publisher = publisher;
            Version = version;
            Abstract = description;
            FoundInToolbarPositions = iconPosition;
            LaunchInDockPositions = panelPosition;

        }

        public Guid UniqueID { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string TitleHashed { get { return FlickrNet.UtilityMethods.MD5Hash(Title); } private set { } }
        public string Path { get { return "//"; } set { } }
        public string IconUrl { get; set; }
        public BitmapImage IconBitmap { get; set; }
        public object Icon
        {
            get
            {
                if (IconUrl == "bitmap") return IconBitmap;
                else return IconUrl;
            }
            set { }
        }
        public string Publisher { get; set; }
        public string Version { get; set; }
        public string ContentControl { get; set; }
        public string AssemblyName { get; set; }
        public string DisplayName { get { return Title; } set { } }
        public string Abstract { get; set; }
        public bool IsExtEnabled { get; set; } = true;
        public bool IsUWPExtension { get; set; } = false;
        public bool CanUninstall { get; set; } = true;
        public ExtensionInToolbarPositions FoundInToolbarPositions { get; set; }
        public ExtensionInToolbarPositions LaunchInDockPositions { get; set; }
        public string AppExtensionUniqueID { get; set; }
        public int Size { get; set; }

    }
}
