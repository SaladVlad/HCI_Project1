using HCI___Fashion.Classes;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace HCI___Fashion
{
    /// <summary>
    /// Interaction logic for CreateOrEditWindow.xaml
    /// </summary>
    public partial class CreateOrEditWindow : Window
    {
        
        public bool IsAdmin { get; set; }

        private Helpers.DataIO io;

        private NotificationManager notificationManager = new NotificationManager();

        private List<System.Drawing.Color> colorList = new List<System.Drawing.Color>();

        public CreateOrEditWindow(bool isAdmin)
        {
            InitializeComponent();
            IsAdmin = isAdmin;
            io = new Helpers.DataIO();
            InitUI();

        }

        public CreateOrEditWindow(ItemContainer itemContainer,bool isAdmin)
        {
            InitializeComponent();
            IsAdmin = isAdmin;
            io = new Helpers.DataIO();

            InitUI();

            FillWithData(itemContainer);
        }

        private void FillWithData(ItemContainer itemContainer)
        {
            IDTextBox.Text = itemContainer.Id.ToString();
            NameTextBox.Text = itemContainer.Name;
            ImageFrame.Source = new BitmapImage(new Uri(itemContainer.ImagePath, UriKind.RelativeOrAbsolute));
            io.LoadRtfFile(itemContainer.TextPath, EditorRichTextBox);
            
        }

        private void InitUI()
        {
            if (!IsAdmin)
            {
                IDTextBox.IsEnabled = false;
                NameTextBox.IsEnabled = false;

                ImageURLTextBox.Visibility = Visibility.Hidden;
                ImageURLTextBox.IsEnabled = false;

                EditorToolBar.Visibility = Visibility.Hidden;
                EditorToolBar.IsEnabled = false;
                
                SaveButton.Visibility= Visibility.Hidden;
                SaveButton.IsEnabled = false;

                WordCountPanel.Visibility = Visibility.Hidden;
            }
            else
            {

                FontFamilyComboBox.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
                FontFamilyComboBox.SelectedIndex = 0;

                foreach(KnownColor inbuiltColor in Enum.GetValues(typeof(KnownColor)))
                {
                    System.Drawing.Color loadColor = System.Drawing.Color.FromKnownColor(inbuiltColor);

                    if(loadColor.IsSystemColor == false)
                    {
                        if (loadColor.Name != "Transparent")
                            colorList.Add(loadColor);
                    }

                }
                ColorComboBox.ItemsSource = colorList;

            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Do you want to abandon these changes?","Warning",MessageBoxButton.YesNo,MessageBoxImage.Warning);
            if(res == MessageBoxResult.Yes) 
                this.Close();
            else
            {
                //do nothing
            }
        }

        private void ImageURLTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(ImageURLTextBox.Text.Equals("Paste new URL..."))
            {
                ImageURLTextBox.Text = "";
                ImageURLTextBox.Foreground = Brushes.Black;
            }
        }

        private void ImageURLTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ImageURLTextBox.Text.Equals(""))
            {
                ImageURLTextBox.Text = "Paste new URL...";
                ImageURLTextBox.Foreground = Brushes.Gray;
            }
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* Some very weird casting and substring magic :) */
            string s = ColorComboBox.SelectedItem.ToString();
            string brushName = s.Substring(s.IndexOf('[')+1,s.IndexOf(']')-s.IndexOf('[')-1);
            Brush brush = (Brush)new BrushConverter().ConvertFromString(brushName);
            ColorRectangle.Fill = brush;
        }

        private void ImageURLTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                try
                {
                    ImageFrame.Source = new BitmapImage(new Uri(ImageURLTextBox.Text));
                }
                catch (Exception ex)
                {
                    RaiseToast("wrongURL");
                }
            }
        }

        private void RaiseToast(string semantic)
        {
            if (semantic.Equals("wrongURL"))
            {
                notificationManager.Show("URL is not valid!",NotificationType.Error,"Error");
            }
        }

        public int CountWords(RichTextBox richTextBox)
        {
            string text = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
            // Remove any non-word characters (e.g., punctuation, whitespace) and count the remaining words
            MatchCollection matches = Regex.Matches(text, @"[\w']+");
            return matches.Count;
        }

}
}
