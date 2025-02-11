using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;

namespace CustomControl.Controls;

public partial class NoteBlock : UserControl
{
    public static readonly StyledProperty<string> NoteContentProperty =
        AvaloniaProperty.Register<NoteBlock, string>(nameof(noteContent));

    public string noteContent
    {
        get => GetValue(NoteContentProperty);
        set => SetValue(NoteContentProperty, value);
    }
    public NoteBlock()
    {
        InitializeComponent();
        DataContext=this;
    }
    
}