using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LiteDBTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFolder folder = ApplicationData.Current.LocalFolder;
        public MainPage()
        {
            this.InitializeComponent();
            LiteDB.LitePlatform.Initialize(new LiteDB.Platform.LitePlatformWindowsStore(folder));
        }

        private async void btnPick_Click(object sender, RoutedEventArgs e)
        {
            var fp = new FolderPicker();
            fp.FileTypeFilter.Add(".dblite");
            var f = await fp.PickSingleFolderAsync();

            if (f != null)
            {
                folder = f;
                LiteDB.LitePlatform.Initialize(new LiteDB.Platform.LitePlatformWindowsStore(folder));
                StorageApplicationPermissions.FutureAccessList.Add(folder);
            }
        }

        private async void btnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var file = await folder.GetFileAsync("test.dblite");
                await file.DeleteAsync();
            } catch (FileNotFoundException fnfe)
            {

            }

            var sw = System.Diagnostics.Stopwatch.StartNew();

            await Task.Run(() =>
           {
               var _db = new LiteDatabase("test.dblite");
               var coll = _db.GetCollection("intial");

               for (int i = 0; i < 100; i++)
               {
                   BsonDocument doc = new BsonDocument();
                   doc["Id"] = 1;

                   coll.Insert(doc);
               }
           });

            var md = new Windows.UI.Popups.MessageDialog("Time:" + sw.Elapsed + ":" + folder.Path);

            await md.ShowAsync();

            System.Diagnostics.Debug.WriteLine("Folder:" + folder.Path);

            System.Diagnostics.Debug.WriteLine("Elapsed:" + sw.Elapsed);
        }
    }
}
