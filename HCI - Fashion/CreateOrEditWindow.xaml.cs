﻿using HCI___Fashion.Classes;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
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
using System.Xml.Linq;
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

        private NotificationManager notificationManager;

        private List<System.Drawing.Color> colorList = new List<System.Drawing.Color>();

        private ItemContainer item;


        public CreateOrEditWindow(ItemContainer itemContainer,bool isAdmin)
        {
            InitializeComponent();
            DataContext = this;

            IsAdmin = isAdmin;
            notificationManager = new NotificationManager(Application.Current.Dispatcher);
            io = new Helpers.DataIO();

            InitUI();
            if(itemContainer!=null)
                FillWithData(itemContainer);

            //passing the reference of the modified item
            item = itemContainer;

        }

        private void FillWithData(ItemContainer itemContainer)
        {
            IDTextBox.Text = itemContainer.Id.ToString();
            NameTextBox.Text = itemContainer.Name;
            if (itemContainer.ImagePath != null)
            {
                ImageFrame.Source = new BitmapImage(new Uri(itemContainer.ImagePath, UriKind.Absolute));

            }
            else
            {
                ImageFrame.Source = new BitmapImage(new Uri("ImgSourceUI/template.jpg", UriKind.RelativeOrAbsolute));
            }
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

                CancelButton.Content = "Exit";
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

        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            bool allGood = true;
            try
            {
                if (!IDTextBox.Text.Equals(string.Empty))
                {
                    int _ = int.Parse(IDTextBox.Text);
                }
                else
                {
                    RaiseToast("emptyID");
                }

            }
            catch
            {
                RaiseToast("badID");
            }

            if (NameTextBox.Text.Equals(""))
            {
                RaiseToast("emptyName");
            }

            if(ImageURLTextBox.Equals("") && 
                ImageFrame.Source == new BitmapImage(
                    new Uri("ImgSourceUI/template.jpg", UriKind.RelativeOrAbsolute)))
            {
                RaiseToast("imageMissing");
            }

            if (allGood)
            {

                item.Id = int.Parse(IDTextBox.Text);
                item.Name = NameTextBox.Text;

                item.ImagePath = ImageFrame.Source.ToString();

                item.TextPath = "item" + IDTextBox.Text + ".rtf";

                item.CreationDate = DateTime.Now;

                io.SaveAsRtfFile(item.TextPath, EditorRichTextBox);


                this.Close();
            }

        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem != null && !EditorRichTextBox.Selection.IsEmpty)
            {
                EditorRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, FontFamilyComboBox.SelectedItem);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBoxResult.Yes;
            //MessageBoxResult res = MessageBox.Show("Do you want to abandon these changes?","Warning",MessageBoxButton.YesNo,MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
            {
                this.Close();
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
            else
            {
                try
                {
                    ImageFrame.Source = new BitmapImage(new Uri(ImageURLTextBox.Text,UriKind.Absolute));
                }
                catch
                {
                    RaiseToast("wrongURL");
                }
            }
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* Some very weird casting and substring magic :) */
            string s = ColorComboBox.SelectedItem.ToString();
            string brushName = s.Substring(s.IndexOf('[')+1,s.IndexOf(']')-s.IndexOf('[')-1);
            Brush brush = (Brush)new BrushConverter().ConvertFromString(brushName);
            ColorRectangle.Fill = brush;

            TextRange selectionRange = new TextRange(EditorRichTextBox.Selection.Start, EditorRichTextBox.Selection.End);

            if (selectionRange != null)
            {
                selectionRange.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            }



        }

        private void ImageURLTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                try
                {
                    ImageFrame.Source = new BitmapImage(new Uri(ImageURLTextBox.Text,UriKind.Absolute));
                }
                catch
                {
                    RaiseToast("wrongURL");
                }
            }
        }

        private void RaiseToast(string semantic)
        {
            if (semantic.Equals("wrongURL"))
            {
                notificationManager.Show("URL is not valid!",NotificationType.Error, "CreateOrUpdateWindowNotificationArea");
            }
            if (semantic.Equals("emptyID"))
            {
                notificationManager.Show("ID can't be Empty!", NotificationType.Error, "CreateOrUpdateWindowNotificationArea");
            }
            if (semantic.Equals("badID"))
            {
                notificationManager.Show("ID is NOT a number!", NotificationType.Error, "CreateOrUpdateWindowNotificationArea");
            }
            if (semantic.Equals("emptyName"))
            {
                notificationManager.Show("Name can't be Empty!", NotificationType.Error, "CreateOrUpdateWindowNotificationArea");
            }
        }

        public int CountWords(RichTextBox richTextBox)
        {
            string text = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;

            MatchCollection matches = Regex.Matches(text, @"[\w']+");
            return matches.Count;
        }

        private void EditorRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //update word count
            WordCountLabel.Content = "Word count: " + CountWords(EditorRichTextBox).ToString();
        }

        private void EditorRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //update the UI elements
            object fontWeight = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            BoldToggleButton.IsChecked = (fontWeight != DependencyProperty.UnsetValue) && (fontWeight.Equals(FontWeights.Bold));

            object fontStyle = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            ItalicToggleButton.IsChecked = (fontStyle != DependencyProperty.UnsetValue) && (fontStyle.Equals(FontStyles.Italic));

            object textDecoration = EditorRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineToggleButton.IsChecked = (textDecoration != DependencyProperty.UnsetValue) && (textDecoration.Equals(TextDecorations.Underline));

            object fontFamily = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            FontFamilyComboBox.SelectedItem = fontFamily;

            object fontSize = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty);
            FontSizeTextBox.Text = fontSize.ToString();


            object fontColor = EditorRichTextBox.Selection.GetPropertyValue(Inline.ForegroundProperty);

            if (fontColor is SolidColorBrush solidColorBrush)
            {
                System.Windows.Media.Color color = solidColorBrush.Color;
                string colorName = GetColorName(color);
                ColorComboBox.SelectedItem = colorName;
            }


        }
        private string GetColorName(System.Windows.Media.Color color)
        {
            foreach (var property in typeof(System.Windows.Media.Colors).GetProperties())
            {
                if ((System.Windows.Media.Color)property.GetValue(null) == color)
                {
                    return property.Name;
                }
            }
            return color.ToString();
        }

        private void FontSizeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //if text size is varying, don't change it because it will mess up the text
            if (FontSizeTextBox.Text.Contains("{D"))
            {
                return;
            }
            try
            {
                int size = int.Parse(FontSizeTextBox.Text);
                ChangeFontSize(size);
            }
            catch
            {
                FontSizeTextBox.Text = "12";
                ChangeFontSize(12);
            }
        }

        private void ChangeFontSize(double size)
        {
           
            TextRange selectionRange = new TextRange(EditorRichTextBox.Selection.Start, EditorRichTextBox.Selection.End);

            if (selectionRange != null)
            {
                selectionRange.ApplyPropertyValue(TextElement.FontSizeProperty, size);
            }
        }

        private void FontSizeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("pressed key: " + e.Key);
            if (e.Key == Key.Enter)
            {

                try
                {
                    double size = int.Parse(FontSizeTextBox.Text);
                    ChangeFontSize(size);
                    FontSizeTextBox.Text = size.ToString();
                }
                catch
                {

                    ChangeFontSize(12.0);
                    FontSizeTextBox.Text = "12";
                }
            }
        }
    }
}
