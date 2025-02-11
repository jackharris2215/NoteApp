using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Media;
using MouseButton = Avalonia.Remote.Protocol.Input.MouseButton;

namespace CustomControl.Controls;

public partial class NoteBlock : UserControl
{
    public double x_offset = 0;
    public double y_offset = 0;
    public bool isPressed = false;
    
    public static readonly StyledProperty<string> NoteNameProperty =
        AvaloniaProperty.Register<NoteBlock, string>(nameof(name));
    
    public static readonly StyledProperty<Window> WindowReference =
        AvaloniaProperty.Register<NoteBlock, Window>(nameof(window));

    public static readonly StyledProperty<string> NoteContentProperty =
        AvaloniaProperty.Register<NoteBlock, string>(nameof(noteContent));

    public static readonly StyledProperty<int[]> PositionProperty =
        AvaloniaProperty.Register<NoteBlock, int[]>(nameof(position));

    public string name{
        get => GetValue(NoteNameProperty);
        set => SetValue(NoteNameProperty, value);
    }
    public Window window{
        get => GetValue(WindowReference);
        set => SetValue(WindowReference, value);
    }
    public string noteContent{
        get => GetValue(NoteContentProperty);
        set => SetValue(NoteContentProperty, value);
    }
    public int[] position{
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }
    public NoteBlock(){
        InitializeComponent();
        DataContext=this;
    }

    public void OnPointerPressed(object sender, PointerPressedEventArgs args)
    {
        if (Parent == null)
            return;
        isPressed = true;
        Avalonia.Point position = args.GetCurrentPoint(sender as Control).Position;
        x_offset = position.X;
        y_offset = position.Y;
        noteContent = args.GetCurrentPoint((Visual)Parent).Position.ToString();
    }
    public void OnPointerReleased(object sender, PointerReleasedEventArgs args)
    {
        if (Parent == null)
            return;
        
        isPressed = false;
    }

    protected override void OnPointerMoved(PointerEventArgs args){
        if (!isPressed)
            return;
        if (Parent == null)
            return;
        double x_pos = (double)args.GetPosition((Visual)Parent).X-x_offset;
        double y_pos = (double)args.GetPosition((Visual)Parent).Y-y_offset;
        if(x_pos < ((Canvas)Parent).Bounds.Width-x_offset && x_pos > 0)
            this.SetValue(Canvas.LeftProperty, x_pos);

        if(y_pos < ((Canvas)Parent).Bounds.Height-y_offset && y_pos > 0)
            this.SetValue(Canvas.TopProperty, y_pos);
    }
    
}