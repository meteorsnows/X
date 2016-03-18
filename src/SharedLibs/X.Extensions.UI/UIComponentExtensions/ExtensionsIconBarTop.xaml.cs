﻿using CoreLib.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WeakEvent;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using X.Extensions;
using X.Extensions.ThirdParty;
using X.UI.Toolbar;

namespace X.Extensions.UIComponentExtensions
{
    public sealed partial class ExtensionsIconBarTop : UserControl, IExtension
    {
        private static ExtensionsService _extensionsService;


        public ExtensionsIconBarTop()
        {
            this.InitializeComponent();
            ExtensionManifest = new ExtensionManifest("Bottom Extensions Toolbar", string.Empty, "Sample Extensions", "1.0", "A UI to manage all the installed extensions in the Top Toolbar", ExtensionInToolbarPositions.None, ExtensionInToolbarPositions.None);
            layoutRoot.DataContext = this;
        }

        private void LoadExtensions()
        {
            _SendMessageSource?.Raise(this, new RequestListOfTopToolbarExtensionsEventArgs() { ReceiverType = ExtensionType.UIComponent });
            
        }

        public async void InstallMyself(ExtensionsService extensionsService) {
            
            _extensionsService = extensionsService;
            await _extensionsService.Install(this);


            var el = new _Template(new ExtensionManifest("Installed Extensions UI", "ms-appx:///Extensions/UIComponentExtensions/InstalledExtensionList/InstalledExtensionList.png", "Sample Extensions", "1.0", "A UI to manage all the installed extensions", ExtensionInToolbarPositions.None, ExtensionInToolbarPositions.Right) { ContentControl = "X.Extensions.UIComponentExtensions.InstalledExtensionList" });
            _extensionsService.Install(el);
            butExtensionStore.ExtensionUniqueId = el.ExtensionManifest.UniqueID;


            LoadExtensions();

        }

        public void UnloadExtensions()
        {

            //foreach (var ext in spExtensions.Children)
            //{
            //    ext.PointerReleased -= NewIcon_PointerReleased;
            //}

            //spExtensions.Children.Clear();
        }




        //IEXTENSION


        private readonly WeakEventSource<EventArgs> _SendMessageSource = new WeakEventSource<EventArgs>();
        public event EventHandler<EventArgs> SendMessage
        {
            add { _SendMessageSource.Subscribe(value); }
            remove { _SendMessageSource.Unsubscribe(value); }
        }

        public IExtensionManifest ExtensionManifest { get; set; }
        public ExtensionType ExtensionType { get; set; } = ExtensionType.UIComponent;
        public string Path { get; set; }
        

        private bool _isExtEnabled = true;
        public bool IsExtEnabled { get { return _isExtEnabled; } set { _isExtEnabled = value; } }

        private bool _canUninstall = false;
        public bool CanUninstall { get { return _canUninstall; } set { _canUninstall = value; } }
        

        public void RecieveMessage(object message)
        {
            if (message is ResponseListOfTopToolbarExtensionsEventArgs)
            {
                var ea = (ResponseListOfTopToolbarExtensionsEventArgs)message;
                
                foreach (var ext in ea.ExtensionsMetadata) {
                    try
                    {
                        spExtensions.AddItem(ext.IconUrl, 20, Guid.Parse((string)ext.UniqueID));
                    }
                    catch {

                    }
                }
            }
        }

        public void OnPaneLoad()
        {

        }

        public void OnPaneUnload()
        {

        }

        private void NewIcon_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _SendMessageSource?.Raise(this, new LaunchExtensionEventArgs() { ReceiverType = ExtensionType.UIComponent, ExtensionUniqueGuid = ((ImageButton)sender).ExtensionUniqueId });
        }



        private void butExtensionStore_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _SendMessageSource?.Raise(this, new LaunchExtensionEventArgs() { ReceiverType = ExtensionType.UIComponent, ExtensionUniqueGuid = ((ImageButton)butExtensionStore).ExtensionUniqueId });
            //LoadExtensions();
        }

        private void spExtensions_Click(object sender, RoutedEventArgs e)
        {
            if (e is ToolbarRoutedEventArgs) {
                _SendMessageSource?.Raise(this, new LaunchExtensionEventArgs() { ReceiverType = ExtensionType.UIComponent, ExtensionUniqueGuid = ((ToolbarRoutedEventArgs)e).UniqueGuid });
            }

        }




    }
}