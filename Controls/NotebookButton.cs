using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Controls.Shapes;
using CustomControl.Helpers;

namespace CustomControl.Controls;

public partial class NotebookButton : UserControl
{

    public static readonly StyledProperty<string> NotebookNameProperty =
        AvaloniaProperty.Register<NotebookButton, string>(nameof(name));
    public static readonly StyledProperty<string> SelectedProperty =
        AvaloniaProperty.Register<NotebookButton, string>(nameof(selected));

    public string name{
        get => GetValue(NotebookNameProperty);
        set => SetValue(NotebookNameProperty, value);
    }
    public string selected{
        get => GetValue(SelectedProperty);
        set => SetValue(SelectedProperty, value);
    }

    public NotebookButton()
    {
        InitializeComponent();
    }
    public void KeyStrokeHandler(object sender, KeyEventArgs args){
        if(args.Key != Key.Enter){
            enter_box.Background = Brushes.White;
            enter_box.Foreground = Brushes.Black;
            return;
        }
        if(enter_box.Text == null)
            return;
        if(enter_box.Text.Length > 10 || 
                enter_box.Text == "Notebooks" || 
                enter_box.Text.Replace(" ", "") == "" || 
                enter_box.Text.Replace(" ", "") != enter_box.Text)
                {
            enter_box.Background = Brushes.Red;
            enter_box.Foreground = Brushes.White;
        }
        else{
            text_box.Width = 150;
            text_box.Text = enter_box.Text;
            name = enter_box.Text;
            main.Children.Remove(enter_box);
        }
    }
    public void refresh(){
        if(name != ""){
            text_box.Width = 150;
            text_box.Text = name;
            main.Children.Remove(enter_box);
        }
        if(selected=="t"){
            text_box.Foreground = Brushes.DarkGreen;
        } else {
            text_box.Foreground = Brushes.White;
        }
    }
    public void OnPointerPressedHandler(object sender, PointerPressedEventArgs args){
        selected = "t";
        text_box.Foreground = Brushes.DarkGreen;
    }
}