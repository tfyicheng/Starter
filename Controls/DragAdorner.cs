using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Starter.Controls
{
    //public class DragAdorner : Adorner
    //{
    //    private readonly UIElement _child;
    //    private double _left;
    //    private double _top;

    //    public DragAdorner(UIElement adornedElement, UIElement child)
    //        : base(adornedElement)
    //    {
    //        _child = child;
    //        IsHitTestVisible = false;
    //        AddVisualChild(_child);
    //    }

    //    public void UpdatePosition(double left, double top)
    //    {
    //        _left = left;
    //        _top = top;
    //        InvalidateArrange();
    //    }

    //    protected override int VisualChildrenCount => 1;

    //    protected override Visual GetVisualChild(int index) => _child;

    //    protected override Size ArrangeOverride(Size finalSize)
    //    {
    //        _child.Arrange(new Rect(new Point(_left, _top), finalSize));
    //        return finalSize;
    //    }
    //}

    public class DragAdorner : Adorner
    {
        private readonly VisualCollection _visuals;
        private readonly FrameworkElement _child;

        private double _left;
        private double _top;

        public DragAdorner(UIElement adornedElement, FrameworkElement child)
            : base(adornedElement)
        {
            _child = child;
            _child.IsHitTestVisible = false;

            _visuals = new VisualCollection(this)
        {
            _child
        };
        }

        public void UpdatePosition(double left, double top)
        {
            _left = left;
            _top = top;

            AdornerLayer.GetAdornerLayer(AdornedElement)?.Update(AdornedElement);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return _child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_left, _top));
            return result;
        }
    }

}
