﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Samples
{
    public sealed partial class ucSample02 : UserControl
    {

        bool isLoaded = false;

        public ucSample02()
        {
            this.InitializeComponent();

          
        }

        private async Task PopulateSampleData()
        {
            if (isLoaded) return;

            //http://higlabo.codeplex.com/documentation
            var cl = new HigLabo.Net.Rss.RssClient();
            var fd = await cl.GetRssFeedAsync(new Uri("https://channel9.msdn.com/Feeds/RSS"));

            lbCommon.ItemsSource = fd.Items;
            lbCommon.ItemTemplateToUse = 1;

            rcb.Invalidate();

            isLoaded = true;
        }

        public async void LoadSample() {
            await PopulateSampleData();
        }

        private void rcb_ValueChanged(object sender, RoutedEventArgs e)
        {
            var selectedItem = rcb.Items[((ComboBox)sender).SelectedIndex];

            if (selectedItem is ComboBoxItem) {
                var newIndex = int.Parse((string)((ComboBoxItem)selectedItem).Content);
                lbCommon.ItemTemplateToUse = newIndex;

                rcb.Invalidate();
            }
            
        }
    }
}
