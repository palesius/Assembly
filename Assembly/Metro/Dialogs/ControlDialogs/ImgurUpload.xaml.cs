﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Assembly.Metro.Native;

namespace Assembly.Metro.Dialogs.ControlDialogs
{
    /// <summary>
    /// Interaction logic for UpCapUpload.xaml
    /// </summary>
    public partial class ImgurUpload : Window
    {
        private string _uploadString;

        public ImgurUpload(string uploadString)
        {
            InitializeComponent();
            DwmDropShadow.DropShadowToWindow(this);
            _uploadString = uploadString;

            lblFriendlyLink.Text = string.Format("[url='http://imgur.com/{0}'][img]http://i.imgur.com/{0}.jpg[/img][/url]", _uploadString);
            lblEvilLink.Text = string.Format("[img]http://i.imgur.com/{0}.jpg[/img]", _uploadString);
            lblViewLink.Text = string.Format("http://imgur.com/{0}", _uploadString);
            lblDirectLink.Text = string.Format("http://i.imgur.com/{0}.jpg", _uploadString);
        }

        private void btnOkay_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }
        private void headerThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Left = Left + e.HorizontalChange;
            Top = Top + e.VerticalChange;
        }
    }
}
