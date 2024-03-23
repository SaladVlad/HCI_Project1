using HCI___Fashion.Classes;
using Notification.Wpf;
using System;
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
using System.Windows.Shapes;

namespace HCI___Fashion
{
    /// <summary>
    /// Interaction logic for ViewWindow.xaml
    /// </summary>
    public partial class ViewWindow : Window
    {


        private Helpers.DataIO io;

        public ViewWindow(ItemContainer itemContainer)
        {
            InitializeComponent();
            DataContext = this;

            io = new Helpers.DataIO();

            FillWithData(itemContainer);

        }

        private void FillWithData(ItemContainer itemContainer)
        {
            IDLabel.Content = itemContainer.Id.ToString();
            NameLabel.Content = itemContainer.Name;
            ImageFrame.Source = new BitmapImage(new Uri(itemContainer.ImagePath, UriKind.Absolute));
            io.LoadRtfFile(itemContainer.TextPath, ViewerRichTextBox);

        }


        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
