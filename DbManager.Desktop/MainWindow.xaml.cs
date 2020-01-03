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

namespace DbManager.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Grpc.Net.Client.GrpcChannel grpcChannel = Grpc.Net.Client.GrpcChannel.ForAddress(@"https://localhost:5001");
            DBDevelopService.DevelopServer.DevelopServerClient client = new DBDevelopService.DevelopServer.DevelopServerClient(grpcChannel);
            var sid = await client.LoginAsync(new DBDevelopService.LoginRequest() { UserName = "admin", Password = "12345", Database = "local" });
            System.Windows.MessageBox.Show(sid.LoginId);
        }
    }
}
