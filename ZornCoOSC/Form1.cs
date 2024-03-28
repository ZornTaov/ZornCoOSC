using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSoft.VRChat.Osc;
using BuildSoft.VRChat.Osc.Avatar;
using static System.Windows.Forms.AxHost;
using static ZornCoOSC.VKeyboard;


namespace ZornCoOSC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {

            OscUtility.Initialize();
            OscAvatarConfig avatarConfig = null;
            //TODO: Move console into Form
            Console.WriteLine($"[NOTIFICATION] Reading now... Try to \"Reset Avatar.\"");
            avatarConfig = await OscAvatarConfig.WaitAndCreateAtCurrentAsync();
            Console.WriteLine($"[NOTIFICATION] Read avatar config. Name: {avatarConfig.Name}");
            OscAvatarParameterChangedEventHandler handler = (sparameter, se) =>
            {
                DateTime now = DateTime.Now;
                //TODO: make these configurable in Form
                if (sparameter.Name.StartsWith("HoodString") || sparameter.Name.StartsWith("Voice") || sparameter.Name.StartsWith("Viseme")) return;
                Console.WriteLine($"[{now.ToShortDateString()} {now.ToShortTimeString()}] " +
                    $"{sparameter.Name}: {se.OldValue} => {se.NewValue}");

                if(sparameter.Name.Contains("OSC"))
                {
                    string[] strings = sparameter.Name.Split('/');
                    switch(strings[1])
                    {
                        case "Media":
                            HandleMedia(strings[2], (bool)se.NewValue);
                            break;
                        case "Discord":
                            HandleDiscord(strings[2], (bool)se.NewValue);
                            break;
                    }
                }
            };
            OscAvatarUtility.AvatarChanged += (ssender, se) =>
            {
                avatarConfig.Parameters.ParameterChanged -= handler;

                avatarConfig = OscAvatarConfig.CreateAtCurrent();
                Console.WriteLine($"[NOTIFICATION] Changed avatar. Name: {avatarConfig.Name}");

                avatarConfig.Parameters.ParameterChanged += handler;
            };
            avatarConfig.Parameters.ParameterChanged += handler;
        }

        private void HandleDiscord(string v, bool state)
        {
            if (!state)
            {
                return;
            }
            switch (v)
            {
                case "Mic":
                    SendWithShiftAndCtrl(ScanCodeShort.KEY_M);
                    break;
                case "Deafen":
                    SendWithShiftAndCtrl(ScanCodeShort.KEY_D);
                    break;
            }
        }

        private void HandleMedia(string v, object value)
        {
            switch (v)
            {
                case "Play":
                    if ((bool)value)
                        SendMediaKey(VirtualKeyShort.MEDIA_PLAY_PAUSE);
                    break;
            }
        }
    }
}
