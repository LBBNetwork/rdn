using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Text;
using System.IO;


namespace rdn
{
    public partial class HomePage : PhoneApplicationPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void Pivot_Loaded(object sender, RoutedEventArgs e)
        { 
               var req = WebRequest.Create("http://rainbowdash.net/api/statuses/public_timeline.xml");
                req.BeginGetResponse(
                                _ar =>
                                {
                                    try
                                    {
                                        HandleResponse(req.EndGetResponse(_ar));
                                    }
                                    catch (WebException ex)
                                    {
                                        if (ex.Response != null)
                                            HandleResponse(ex.Response);
                                        else
                                            // ex.ToString() might be used to give the user information, like hostname unresolvable, etc.. Basically anything where we didn’t manage to get a response at all from the sserver.
                                            MessageBox.Show(ex.ToString());
                                    }
                                    var something = req.EndGetResponse(_ar);
         
                                  //  MessageBox.Show("Something went wrong");
                            }, null);
                        

        }

        private static void HandleResponse(WebResponse response)
        {
            // The caller will not call Dispose, so put in using() ourselves.
            using (var _response = (HttpWebResponse)response)
            {
                var good = false;
                // Hopefully this came from an HTTP request?
                var httpResponse = response as HttpWebResponse;
                if (httpResponse != null)
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        good = true;

                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                           // MessageBox.Show(response.ToString());
                        });

                    }
                }
                if (!good)
                {
                    // See if we can find <hash><error/></hash> and the textcontent of <error/>.
                   // using (var reader = XmlReader.Create(response.GetResponseStream()))
                       // if (reader.ReadToFollowing("error"))
                         //   MessageBox.Show(reader.ReadElementContentAsString());
                }
            }
        }

        private void post_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}