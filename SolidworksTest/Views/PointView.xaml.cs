﻿using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SolidworksTest.Views;
/// <summary>
/// Interaction logic for PointView.xaml
/// </summary>
public partial class PointView : UserControl
{
    public PointView()
    {
        InitializeComponent();
    }

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        Regex _regex = new Regex("[^0-9.-]+");
        e.Handled = _regex.IsMatch(e.Text);
    }
}
