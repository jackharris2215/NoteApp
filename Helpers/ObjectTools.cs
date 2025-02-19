using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Controls.Shapes;

namespace CustomControl.Helpers{

public static class ObjectTools{

    public static int[] HandleBorder(string object_name,
                                    object sender, 
                                    int[] thicknesses){

        char[] sides = ['l', 't', 'r', 'b'];
        int index = Array.IndexOf(sides, object_name[object_name.Length-1]);
        bool on = thicknesses[index]==2;
        thicknesses[index] = on?0:2;
        var rectangle = sender as Rectangle;
        if (rectangle != null)
            rectangle.Fill = on ? Brushes.Orange : Brushes.Yellow;
        return thicknesses;
        
    }

    public static void HandleResize(UserControl control, 
                                    Grid main_grid, 
                                    int[] size, 
                                    PointerEventArgs args,
                                    PathIcon resize_rect){

        Avalonia.Point pos = args.GetPosition(resize_rect);
        double x_pos = (double)pos.X;
        double y_pos = (double)pos.Y;
        bool changed = false;

        if(x_pos > 10) {
            size[0]+=10;
            changed = true;
        }
        
        if(y_pos > 10) {
            size[1]+=10; 
            changed = true;
        }
        
        if(x_pos < -10 && size[0] > 100) {
            size[0]-=10;
            changed = true;
        }
        
        if(y_pos < -10 && size[1] > 60) {
            size[1]-=10; 
            changed = true;
        }
        
        if(changed){
            control.Width = size[0];
            main_grid.Width = size[0];
            main_grid.ColumnDefinitions.Clear();
            main_grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            control.Height = size[1];
            main_grid.Height = size[1];
            main_grid.RowDefinitions.Clear();
            main_grid.RowDefinitions.Add(new RowDefinition(new GridLength(10)));
            main_grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            main_grid.RowDefinitions.Add(new RowDefinition(new GridLength(10)));
        }
    }

    public static void HandleDrag(UserControl control, 
                                    Grid main_grid, 
                                    double x_offset, 
                                    double y_offset, 
                                    PointerEventArgs args){  

        Avalonia.Point pos = args.GetPosition(main_grid);
        double x_pos = Math.Round((double)pos.X-x_offset);
        double y_pos = Math.Round((double)pos.Y-y_offset);

        if(x_pos > 10)
            control.SetValue(Canvas.LeftProperty, control.GetValue(Canvas.LeftProperty)+10);
        if(x_pos < -10)
            control.SetValue(Canvas.LeftProperty, control.GetValue(Canvas.LeftProperty)-10);
        if(y_pos > 10)
            control.SetValue(Canvas.TopProperty, control.GetValue(Canvas.TopProperty)+10);
        if(y_pos < -10)
            control.SetValue(Canvas.TopProperty, control.GetValue(Canvas.TopProperty)-10);
    }
}
}