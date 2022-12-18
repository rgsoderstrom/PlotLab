﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml;

namespace utHelpWindow3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {

        List<string> helpTextFiles = new List<string> ()
        {
            @"C:\Users\rgsod\Documents\Visual Studio 2022\Projects\PlotLab\utHelpWindow3\HelpTextFiles\GenaralPlotting.xml",
            @"C:\Users\rgsod\Documents\Visual Studio 2022\Projects\PlotLab\utHelpWindow3\HelpTextFiles\MathFunctions.xml",
        };

        public HelpWindow ()
        {
            InitializeComponent ();
        }

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            HelpTreeViewItem.textPane = HelpTextBlock;
            HelpTreeViewItem.scrollViewer = HelpTextScroller;
            HelpTreeViewItem.clearButton = HelpTextClearButton;

            List<HelpList> lst = new List<HelpList> ();

            foreach (string filename in helpTextFiles)
                lst.Add (new HelpList (filename));

            foreach (HelpList hl in lst)
                foreach (HelpTreeViewItem htvi in hl)
                    htvi.LookUpSubtopics ();

            foreach (HelpList hl in lst)
                TreeView.Items.Add (hl [0]);
        }

        private void HelpTextClearButton_Click (object sender, RoutedEventArgs e)
        {
            HelpTextBlock.Inlines.Clear ();
            HelpTextClearButton.IsEnabled = false;
            (TreeView.SelectedItem as TreeViewItem).IsSelected = false;
        }
    }
}