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

public partial class NotePreview : UserControl
{
    public bool selected = false;

    public static readonly StyledProperty<string> NoteContentProperty =
        AvaloniaProperty.Register<NotePreview, string>(nameof(content));

    public string content{
        get => GetValue(NoteContentProperty);
        set => SetValue(NoteContentProperty, value);
    }

    public NotePreview()
    {
        InitializeComponent();
    }
    public void PointerPressedHandler(object sender, PointerPressedEventArgs args){
        switch(selected){
            case true:
                unfocus();
                break;
            case false:
                focus();
                break;
        }
        
    }
    public void focus(){
        main_grid.Background = new SolidColorBrush(Colors.Gray, 0.1);
        selected = true;
    }
    public void unfocus(){
        main_grid.Background = new SolidColorBrush(Colors.Gray, 0.0);
        selected = false;
    }
    // workaround for content not loading upon initialization
    public void refresh(){
        sub_heading.Text = content;
    }
}