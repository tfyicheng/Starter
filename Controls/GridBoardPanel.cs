using System;
using System.Windows;
using System.Windows.Controls;

namespace Starter.Controls
{
    public class GridBoardPanel : Panel
    {
        public double CellSize
        {
            get => (double)GetValue(CellSizeProperty);
            set => SetValue(CellSizeProperty, value);
        }

        public static readonly DependencyProperty CellSizeProperty =
            DependencyProperty.Register(
                nameof(CellSize),
                typeof(double),
                typeof(GridBoardPanel),
                new FrameworkPropertyMetadata(80d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsArrange));

        public int Columns { get; private set; }
        public int Rows { get; private set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsInfinity(availableSize.Width))
                availableSize.Width = 0;

            Columns = Math.Max(1, (int)(availableSize.Width / CellSize));

            int index = 0;
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(new Size(CellSize, CellSize));
                index++;
            }

            Rows = (int)Math.Ceiling((double)InternalChildren.Count / Columns);

            return new Size(
                Columns * CellSize,
                Rows * CellSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Columns = Math.Max(1, (int)(finalSize.Width / CellSize));

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                int row = i / Columns;
                int col = i % Columns;

                var rect = new Rect(
                    col * CellSize,
                    row * CellSize,
                    CellSize,
                    CellSize);

                InternalChildren[i].Arrange(rect);
            }

            Rows = (int)Math.Ceiling((double)InternalChildren.Count / Columns);

            return new Size(
                Columns * CellSize,
                Rows * CellSize);
        }
    }
}
