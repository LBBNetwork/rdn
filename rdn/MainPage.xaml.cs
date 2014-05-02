/* Filename: MainPage.xaml.cs
 * Author(s): neko2k, binki
 * Contact: support@beige-box.com
 * Copyright (c) 2014 The Little Beige Box & Oh! No! Publishing
 * http://www.beige-box.com & http://ohnopub.net
 * 
 * Description: Main cs file for the RDN status.net client.
 * This is really basic and primitive right now so don't
 * be too surprised if nothing works right.
 * 
 * Basic API support for posting to the site but that's
 * about it right now.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading;
using System.Xml;
using System.Text;
using rdn.Resources;

namespace rdn
{
        public partial class MainPage : PhoneApplicationPage
        {
            
            // Constructor
            public MainPage()
            {
                InitializeComponent();
            }

                // Sample code to localize the ApplicationBar
            //    BuildLocalizedApplicationBar();

            private void sendmsg_Click(object sender, EventArgs e)
            {


                var statusText = statusbox.Text;
                var req = WebRequest.Create("http://rainbowdash.net/api/statuses/update.xml");
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";

                if (userbox.Text == "")
                {
                    MessageBox.Show("Please enter your username");
                }
                else
                {
                    if (passbox.Password == "")
                    {
                        MessageBox.Show("Please enter your password");
                    }
                    else
                    {
                        req.Credentials = new NetworkCredential(userbox.Text, passbox.Password);
                        req.BeginGetRequestStream(
                        ar =>
                        {
                            try
                            {
                                using (var requestStream = req.EndGetRequestStream(ar))
                                {
                                    var data = Encoding.UTF8.GetBytes("status=" + Uri.EscapeDataString(statusText));
                                    requestStream.Write(data, 0, data.Length);
                                }
                            }
                            catch (WebException ex)
                            {
                                if (ex.Response != null)
                                    HandleResponse(ex.Response);
                                else
                                    MessageBox.Show(ex.Response.ToString());
                                // Don’t try to call req.BeginGetResponse() *now*, heh.
                                return;
                            }

                            req.BeginGetResponse(
                                _ar =>
                                {
                                    try
                                    {
                                        HandleResponse(req.EndGetResponse(_ar));
                                    }
                                    catch (WebException ex)
                                    {
                                        if(ex.Response != null)
                                            HandleResponse(ex.Response);
                                        else
                                            // ex.ToString() might be used to give the user information, like hostname unresolvable, etc.. Basically anything where we didn’t manage to get a response at all from the sserver.
                                            MessageBox.Show(ex.ToString());
                                    }
                                   // var something = req.EndGetResponse(_ar);
                                  //  MessageBox.Show("Something went wrong");
                            }, null);
                        }, null);

                        statusbox.Text = "";
                    }
                }
                



            }

        /// <summary>
        ///   We will handle, interact with the user about, and call Dispose() on response for you.
        /// </summary>
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
                        good = true;
                }
                if (!good)
                {
                    // See if we can find <hash><error/></hash> and the textcontent of <error/>.
                    using (var reader = XmlReader.Create(response.GetResponseStream()))
                        if (reader.ReadToFollowing("error"))
                            MessageBox.Show(reader.ReadElementContentAsString());
                }
            }
        }



            // Sample code for building a localized ApplicationBar
            //private void BuildLocalizedApplicationBar()
            //{
            //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
            //    ApplicationBar = new ApplicationBar();

            //    // Create a new button and set the text value to the localized string from AppResources.
            //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
            //    appBarButton.Text = AppResources.AppBarButtonText;
            //    ApplicationBar.Buttons.Add(appBarButton);

            //    // Create a new menu item with the localized string from AppResources.
            //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            //    ApplicationBar.MenuItems.Add(appBarMenuItem);
            //}
        }
    }
    
