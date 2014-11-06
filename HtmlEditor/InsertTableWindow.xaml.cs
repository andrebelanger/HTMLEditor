using System;
using System.Collections.Generic;
using System.Windows;

namespace HtmlEditor
{
    /// <summary>
    /// Interaction logic for InsertTableWindow.xaml
    /// </summary>
    public partial class InsertTableWindow : InsertWindow
    {
        private const int MinSize = 1;
        private const int MaxSize = 99;

        public InsertTableWindow()
        {
            InitializeComponent();
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
                var rows = Convert.ToInt32(RowsBox.Text);
                var columns = Convert.ToInt32(ColumnsBox.Text);

                if (rows < MinSize || rows > MaxSize || columns < MinSize || columns > MaxSize)
                {
					throw new FormatException(string.Format("Number provided was out of the allowed range ({0}-{1})", MinSize, MaxSize));
                }

                var table = new List<string> { "<table>" };


                for (var i = 0; i < rows; i++)
                {
                    table.Add("<tr>");
                    for (var j = 0; j < columns; j++)
                        table.Add("<td></td>");
                    table.Add("</tr>");
                }

                table.Add("</table>");

                Text = table;

	            Close();
            }
            catch (Exception ex)
            {
	            Error.Text = ex.Message;
                Error.Visibility = Visibility.Visible;
            }
        }

        private void CancelPressed(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
