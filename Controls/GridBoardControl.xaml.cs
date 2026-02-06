using Starter.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

public class GridBoardControl : ItemsControl
{
    static GridBoardControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(GridBoardControl),
            new FrameworkPropertyMetadata(typeof(GridBoardControl)));
    }

    public double CellSize
    {
        get => (double)GetValue(CellSizeProperty);
        set => SetValue(CellSizeProperty, value);
    }

    public static readonly DependencyProperty CellSizeProperty =
        DependencyProperty.Register(
            nameof(CellSize),
            typeof(double),
            typeof(GridBoardControl),
            new PropertyMetadata(80d));

    public Size IdealSize { get; private set; }

    public event EventHandler<Size>? IdealSizeChanged;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        LayoutUpdated += OnLayoutUpdated;
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        if (ItemsPanelRoot is GridBoardPanel panel)
        {
            var size = new Size(
                panel.Columns * CellSize,
                panel.Rows * CellSize);

            if (size != IdealSize)
            {
                IdealSize = size;
                IdealSizeChanged?.Invoke(this, size);
            }
        }
    }
}
