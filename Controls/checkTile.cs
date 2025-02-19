using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Controls.Shapes;

namespace CustomControl.Controls;

public partial class checkTile : UserControl
{
    public checkTile(){
        InitializeComponent();
    }
    public void OnPointerReleased(object sender, PointerReleasedEventArgs args){
        if (Parent == null)
            return;
        if (sender == null)
            return;
        
        var object_name = (sender as Rectangle)?.Name;
        if (object_name == null)
            object_name = (sender as PathIcon)?.Name;
        if (object_name == null)
            return;

        switch(object_name){
            case "check_box":
                check.HorizontalAlignment = check.HorizontalAlignment==Avalonia.Layout.HorizontalAlignment.Stretch?
                                        Avalonia.Layout.HorizontalAlignment.Left:
                                        Avalonia.Layout.HorizontalAlignment.Stretch;
                break;
            case "delete":
                if (Parent is StackPanel s)
                    s.Children.Remove(this);
                break;
        }
        
    }
    public void OnFocusHandler(object sender, GotFocusEventArgs args){
        // delete.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        delete.Height = 20;
        main.ColumnDefinitions.Clear();
        main.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(30)));
        main.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        main.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(30)));
    }
    public void OffFocusHandler(object sender, RoutedEventArgs e){
        // delete.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
        delete.Height = 0;
        main.ColumnDefinitions.Clear();
        main.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(30)));
        main.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        main.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(0)));
    }
}