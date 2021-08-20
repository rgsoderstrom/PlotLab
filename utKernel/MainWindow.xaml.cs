using System;
using System.Windows;
using System.Collections.Generic;


namespace utKernel
{
    public partial class MainWindow : Window
    {
        KernelTest kernelTest = null;

        public MainWindow ()
        {
            InitializeComponent ();
            Left = 800;
        }

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            try
            {
                kernelTest = new KernelTest (this);
                kernelTest.NextTest ();
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception: " + ex.Message);
            }
        }

        private void Next_Button_Click (object sender, RoutedEventArgs e)
        {
            kernelTest.NextTest ();
        }

        private void Window_Closed (object sender, EventArgs e)
        {            
            App.Current.Shutdown ();
        }
    }
}
