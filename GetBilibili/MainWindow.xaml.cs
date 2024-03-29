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
using UC_Pub.Enums;

namespace GetBilibili
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataViewModel VM;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }


        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (VM.DownLoading) return;
            if (MessageBox.Show("正在下载中是否强制退出!",
                    "退出提示", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;
            VM = new DataViewModel();
            DataContext = VM;
        }

        private void Analysis_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(VM.KeyWord))
            {
                MessageBox.Show("关键字不能为空!", "提示");
                return;
            }
            string result = VM.Analysis();
            MessageBox.Show(result == Enums.成功 ? "成功" : $"失败\r\n{result}");
        }

        private void DownLoad_Click(object sender, RoutedEventArgs e)
        {
            if (VM.UrlList.Count <= 0)
            {
                MessageBox.Show("Url地址不能为空!\r\n请重新解析");
                return;
            }
            VM.Start();
        }

        private void Addition_OnClick(object sender, RoutedEventArgs e)
        {
            VM.Addition();
        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            VM.Delete();
        }

        private void FilePath_OnClick(object sender, RoutedEventArgs e)
        {
            VM.SelectFilePath();
        }
    }
}
