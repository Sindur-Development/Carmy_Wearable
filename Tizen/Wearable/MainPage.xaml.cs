using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tizen.Wearable.CircularUI.Forms;
using Samsung.Sap;
using Tizen.Applications;

namespace Wearable
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : CirclePage
    {
        private Agent Agent;
        private Connection Connection;
        private Peer Peer;
        private Channel ChannelId;
        private Boolean isLocked = true;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            Connect();
        }

        private void Button_Clicked_Lock(object sender, EventArgs e)
        {
            ReceivedMessage = Peer.Status.ToString() + " " + Peer.ToString(); 
            try
            {
                if (Peer != null)
                {
                    if (isLocked) { 
                        Connection.Send(ChannelId, Encoding.UTF8.GetBytes("Unlock"));
                        ShowMessage("Unlock command sent to phone");
                    }
                    else
                    {
                        Connection.Send(ChannelId, Encoding.UTF8.GetBytes("Lock"));
                        ShowMessage("Lock command sent to phone");
                    }

                }
                else
                {
                    ShowMessage("Connection to phone is missing, trying to connect");
                    Connect();
                }
               

            }
            catch (Exception ex)
            {
                Console.WriteLine("Command error: " + ex);
            }
            
        }

        private void Button_Clicked_LaunchStore(object sender, EventArgs e)
        {
            DeepLinkLaunchStore();
        }

        private void Button_Clicked_Precondition(object sender, EventArgs e)
        {
            try
            {
                if (Peer != null)
                {
                    Connection.Send(ChannelId, Encoding.UTF8.GetBytes("Precondition"));
                    ReceivedMessage = "Precondition command sent to phone";
                }
                else
                {
                    ReceivedMessage = "Connection to phone is missing, trying to connect";
                    Connect();
                }

                if (Peer.Status != PeerStatus.Found)
                {
                    ReceivedMessage = "Connection to phone is missing, trying to connect";
                    Connect();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Command error: " + ex);
            }
        }

        // connect to phone using the Samsung.SAP nuget package
        private async void Connect()
        {
            try
            {
                Agent = await Agent.GetAgent("/example/companion");
                var peers = await Agent.FindPeers();
                ChannelId = Agent.Channels.First().Value;
                if (peers.Count() > 0)
                {
                    Console.WriteLine("Connection to phone established");
                    Peer = peers.First();
                    Connection = Peer.Connection;
                    Connection.DataReceived -= Connection_DataReceived;
                    Connection.DataReceived += Connection_DataReceived;
                    await Connection.Open();
                    ShowMessage("Connected");
                }
                else
                {
                    ReceivedMessage = "Phone not found";
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message);
            }
        }

        // broadcaster that looks for messages from phone
        private void Connection_DataReceived(object sender, DataReceivedEventArgs e)
        {
            ShowMessage("Command status received");
            ReceivedMessage = System.Text.Encoding.ASCII.GetString(e.Data);
        }

        private string receivedMessage = "Label will show message from phone";
        public string ReceivedMessage
        {
            get => receivedMessage;
            set
            {
                receivedMessage = value;
                OnPropertyChanged();
                if (value == "Locked" || value=="Unlocked")
                {
                    isLocked = !isLocked;
                    lockButton.Text = value == "Locked" ? "Unlock" : "Lock";
                }
                if (value == "Car Unlocked")
                {
                    isLocked = !isLocked;
                    lockButton.Text = "Lock";
                }
            }
        }

        // toast to show messages
        private void ShowMessage(string message, string debugLog = null)
        {
            Toast.DisplayText(message, 1000);
            if (debugLog != null)
            {
                debugLog = message;
            }
            Console.WriteLine("[DEBUG] " + message);
        }

        private void DeepLinkLaunchStore()
        {
            AppControl launchStore = new AppControl();
            string storeUrl = @"https://play.google.com/store/apps/details?id=[PACKAGE-NAME-HERE]";
            launchStore.Operation = AppControlOperations.Default;
            launchStore.ApplicationId = "com.samsung.w-manager-service";
            launchStore.ExtraData.Add("deeplink", storeUrl);
            launchStore.ExtraData.Add("type", "phone");

            try
            {
                AppControl.SendLaunchRequest(launchStore);
            }
            catch (Exception e)
            {
                Console.WriteLine("Store launch error: " + e);
            }
        }

     
    }
}