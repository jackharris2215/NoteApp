using Avalonia.Controls;
using Avalonia.Media;
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
            noteContent = "BAH"
        };

        Canvas main = canvas_container;
        Canvas.SetLeft(noteBlock, 100);
        Canvas.SetTop(noteBlock, 50);
        main.Children.Add(noteBlock);
    }

    public void addNoteHandler(object sender, RoutedEventArgs e)
    {
        // output.Text =
        //   (output.Text == "AhA") ?
        //    "aHa" : "AhA";
        var noteBlock = new CustomControl.Controls.NoteBlock {
            noteContent = "Nah"
        };
        Canvas main = canvas_container;
        Canvas.SetLeft(noteBlock, 600);
        Canvas.SetTop(noteBlock, 50);
        main.Children.Add(noteBlock);
        
    }
}