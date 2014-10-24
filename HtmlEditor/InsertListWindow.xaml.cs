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
using System.Windows.Shapes;

namespace HtmlEditor
{
    /// <summary>
    /// Interaction logic for InsertListWindow.xaml
    /// </summary>
    public partial class InsertListWindow : Window
    {
        private HtmlEditorWindow _editorWindow;
        private const int MIN_SIZE = 1;
        private const int MAX_SIZE = 99;
        public InsertListWindow(HtmlEditorWindow editorWindow)
        {
            InitializeComponent();
            _editorWindow = editorWindow;
            error.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Call the InsertList() function from HtmlEditorWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertPressed(object sender, RoutedEventArgs e)
        {
            try
            {
                int columns = Convert.ToInt32(ColumnsBox.Text);
                if (columns < MIN_SIZE || columns > MAX_SIZE)
                {
                    throw new ArgumentOutOfRangeException("Number provided was out of the specified range");
                }
                if (OrderedBox.IsChecked == true)
                {
                    _editorWindow.InsertOrderedList(columns);
                }
                else
                {
                    _editorWindow.InsertUnorderedList(columns);
                }
                error.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                error.Visibility = Visibility.Visible;
            }
        }

        private void CancelPressed(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
