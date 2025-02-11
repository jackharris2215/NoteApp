using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Input;

namespace CustomControl.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var noteBlock = new CustomControl.Controls.NoteBlock {
            name = "Note1",
            window = this,
            noteContent = "BAH",
            position = new int[2] {50, 100}
        };

        Canvas main = canvas_container;
        Canvas.SetLeft(noteBlock, 100);
        Canvas.SetTop(noteBlock, 50);
        main.Children.Add(noteBlock);
    }

    public void addNoteHandler(object sender, RoutedEventArgs e)
    {
        var noteBlock = new CustomControl.Controls.NoteBlock {
            name = "Note2",
            window = this,
            noteContent = "Nah",
            position = new int[2] {200, 140}
        };
        Canvas main = canvas_container;
        Canvas.SetLeft(noteBlock, 600);
        Canvas.SetTop(noteBlock, 50);
        main.Children.Add(noteBlock);
        
    }
}