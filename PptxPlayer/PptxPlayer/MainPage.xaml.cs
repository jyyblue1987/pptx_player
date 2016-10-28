﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PptxPlayer
{
    public sealed partial class MainPage : Page
    {
        public Library lib = new Library();
        //private Stream stream = new MemoryStream();
        //private CancellationTokenSource cts;

        private int m_nCurrentIndex = 0;
        private JToken [] m_pathArray = null; 

        public MainPage()
        {
            this.InitializeComponent();
        }
        private void Go_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                showSlids(Int32.Parse(Value.Text) - 1);
                //Display.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(Value.Text));
            }
        }

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".ppt");
            openPicker.FileTypeFilter.Add(".pptx");
                        
            StorageFile file = await openPicker.PickSingleFileAsync();


            //bool ret = await UploadFile(file, "http://localhost:50174/Upload/Upload");       

            //Chilkat.Upload upload = new Chilkat.Upload();

            ////  Specify the page (ASP, ASP.NET, Perl, Python, Ruby, CGI, etc)
            ////  that will receive and process the HTTP Upload.
            //upload.Hostname = "localhost";
            //upload.Port = 50174;
            //upload.Path = "/Upload/Upload";
            //upload.Ssl = false;


            ////  Add one or more files to be uploaded.            
            //upload.AddFileReference("file", file.Path);


            ////  Do the upload.  The method returns when the upload
            ////  is completed.
            ////  This component also includes asynchronous upload capability,
            ////  which is demonstrated in another example.
            //bool success = await upload.BlockingUploadAsync();
            //if (success != true)
            //{
            //    Debug.WriteLine(upload.LastErrorText);
            //}
            //else {
            //    Debug.WriteLine("Files uploaded!");
            //}

            StorageFile[] array = new StorageFile[1];
            array[0] = file;

            String result = await UploadFiles(array);
            try {
                JObject ret = JObject.Parse(result);
                JToken [] temp_array = ret["foo"].ToArray();
                
                if (temp_array.Count() < 1)
                    return;

                m_pathArray = temp_array;

                showSlids(0);
            } catch
            {

            }
        }

        private void showSlids(int index)
        {
            if (m_pathArray == null)
                return;

            int count = m_pathArray.Count();
            if (index < 0)
                index = 0;
            if (index >= count)
                index = count - 1; ;
            m_nCurrentIndex = index;

            Value.Text = (index + 1) + "";
            page_count.Text = "/" + count;

            string server_path = (string)m_pathArray[m_nCurrentIndex].ToString();
            Display.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(server_path));
        }

        public async Task<bool> UploadFile(StorageFile file, string upload_url)
        {
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                var content = new MultipartFormDataContent();
                if (file != null)
                {
                    var streamData = await file.OpenReadAsync();
                    var bytes = new byte[streamData.Size];
                    using (var dataReader = new DataReader(streamData))
                    {
                        await dataReader.LoadAsync((uint)streamData.Size);
                        dataReader.ReadBytes(bytes);
                    }
                    var streamContent = new ByteArrayContent(bytes);
                    content.Add(streamContent, "file");
                }
                //client.DefaultRequestHeaders.Add("Access-Token", AccessToken);
                var response = await client.PostAsync(new Uri(upload_url, UriKind.Absolute), content);
                if (response.IsSuccessStatusCode)
                    return true;
            }
            catch { return false; }

            return false;
        }

        public async static Task<string> UploadFiles(StorageFile[] files)
        {
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                var content = new System.Net.Http.MultipartFormDataContent();
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        var streamData = await file.OpenReadAsync();
                        var bytes = new byte[streamData.Size];
                        using (var dataReader = new DataReader(streamData))
                        {
                            await dataReader.LoadAsync((uint)streamData.Size);
                            dataReader.ReadBytes(bytes);
                        }
                        string fileToUpload = file.Path;
                        using (var fstream = await file.OpenReadAsync())
                        {
                            var streamContent = new System.Net.Http.ByteArrayContent(bytes);
                            streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                            {
                                Name = "files[]",
                                FileName = Path.GetFileName(fileToUpload),

                            };
                            content.Add(streamContent);
                        }
                    }
                }
                var response = await client.PostAsync(new Uri("http://localhost:50174/Upload/Upload", UriKind.Absolute), content);
                var contentResponce = await response.Content.ReadAsStringAsync();
                return contentResponce;
            }
            catch { return ""; }

        }

        private void Pitch_Click(object sender, RoutedEventArgs e)
        {
            lib.Rotate("X", ref Display);
        }

        private void Yaw_Click(object sender, RoutedEventArgs e)
        {
            lib.Rotate("Y", ref Display);
        }

        private void Roll_Click(object sender, RoutedEventArgs e)
        {
            lib.Rotate("Z", ref Display);
        }

        private void Transition_Click(object sender, RoutedEventArgs e)
        {
            lib.Transition("X", ref Display);
        }

        private void Top_Click(object sender, RoutedEventArgs e)
        {
            lib.Transition("Y", ref Display);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            showSlids(m_nCurrentIndex - 1);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            showSlids(m_nCurrentIndex + 1);
        }
    }
}
