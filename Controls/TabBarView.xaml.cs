using Starter.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Starter.Controls
{
    public partial class TabBarView : UserControl
    {
        private TabBarViewModel VM => (TabBarViewModel)DataContext;

        private DragAdorner? _dragAdorner;
        private AdornerLayer? _adornerLayer;


        private bool _isDragging;
        private Point _dragStart;


        public TabBarView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)


        {
            // 容器生成完成后再更新指示器（修复你之前的 NullReference）
            //ItemsHost.ItemContainerGenerator.StatusChanged += (_, __) =>
            //{
            //    if (ItemsHost.ItemContainerGenerator.Status ==
            //        System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            //    {
            //        UpdateIndicator(true);
            //    }
            //};

            ItemsHost.ItemContainerGenerator.StatusChanged += (_, __) =>
            {
                if (ItemsHost.ItemContainerGenerator.Status ==
                    GeneratorStatus.ContainersGenerated)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        UpdateIndicator(false);
                    }), DispatcherPriority.Loaded);
                }
            };

            VM.SelectedIndex = Math.Max(0, VM.SelectedIndex);

            VM.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(TabBarViewModel.SelectedIndex))
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        UpdateIndicator(false);
                    }), DispatcherPriority.Loaded);
                }
            };
        }

        private void UpdateIndicator(bool immediate)
        {
            if (ItemsHost == null) return;

            int index = VM.SelectedIndex;
            if (index < 0 || index >= ItemsHost.Items.Count) return;

            var container = ItemsHost.ItemContainerGenerator
                .ContainerFromIndex(index) as FrameworkElement;

            // ⚠️ 容器还没生成 → 延迟
            if (container == null)
            {
                Dispatcher.BeginInvoke(
                    DispatcherPriority.Loaded,
                    new Action(() => UpdateIndicator(immediate)));
                return;
            }

            Indicator.Width = container.ActualWidth;

            double targetX = container.TransformToAncestor(this)
                .Transform(new Point(0, 0)).X;

            if (immediate)
            {
                IndicatorTransform.X = targetX;
            }
            else
            {
                var anim = new DoubleAnimation
                {
                    To = targetX,
                    Duration = TimeSpan.FromMilliseconds(220),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                IndicatorTransform.BeginAnimation(
                    System.Windows.Media.TranslateTransform.XProperty, anim);
            }
        }

        private Point _dragStartPoint;


        private void Item_MouseMove(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("Item_MouseMove");
            if (e.LeftButton != MouseButtonState.Pressed)
                return;


            if (sender is ListBoxItem item &&
    item.DataContext is TabItemViewModel vm)

            {
                DragDrop.DoDragDrop(item, vm, DragDropEffects.Move);
            }


            //        if (_dragAdorner != null)
            //            return;

            //        Point current = e.GetPosition(null);
            //        if (Math.Abs(current.X - _dragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
            //            Math.Abs(current.Y - _dragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
            //            return;


            //        if (sender is ListBoxItem item &&
            //item.DataContext is TabItemViewModel vm)
            //        {
            //            // 1️⃣ 创建视觉副本
            //            var presenter = new ContentPresenter
            //            {
            //                Content = vm,
            //                ContentTemplate = ItemsHost.ItemTemplate,
            //                Opacity = 0.85,
            //                RenderTransform = new ScaleTransform(1.05, 1.05),
            //                RenderTransformOrigin = new Point(0.5, 0.5)
            //            };

            //            // 2️⃣ 加到 AdornerLayer
            //            _adornerLayer = AdornerLayer.GetAdornerLayer(this);
            //            _dragAdorner = new DragAdorner(this, presenter);
            //            _adornerLayer?.Add(_dragAdorner);

            //            // 3️⃣ 启动拖拽
            //DragDrop.DoDragDrop(item, vm, DragDropEffects.Move);

            //            // 4️⃣ 拖拽结束清理
            //            _adornerLayer?.Remove(_dragAdorner);
            //            _dragAdorner = null;
            //        }

        }

        private void Item_Drop(object sender, DragEventArgs e)
        {

            InsertIndicator.Visibility = Visibility.Collapsed;

            if (sender is not ListBoxItem targetItem ||
                targetItem.DataContext is not TabItemViewModel target)
                return;

            if (e.Data.GetData(typeof(TabItemViewModel)) is not TabItemViewModel source)
                return;

            var tabs = VM.Tabs;

            int oldIndex = tabs.IndexOf(source);
            int targetIndex = tabs.IndexOf(target);

            if (oldIndex < 0 || targetIndex < 0 || oldIndex == targetIndex)
                return;

            //// 判断插入位置（前 / 后）
            bool insertAfter = e.GetPosition(targetItem).X > targetItem.ActualWidth / 2;
            int newIndex = insertAfter ? targetIndex + 1 : targetIndex;

            if (newIndex > oldIndex)
                newIndex--;


            int oldSelectedIndex = VM.SelectedIndex;

            // 移动
            tabs.Move(oldIndex, newIndex);

            // 🔑 是否需要修正选中项
            if (oldIndex == oldSelectedIndex)
            {
                // 拖的是当前选中项
                VM.SelectedIndex = newIndex;
            }
            else if (oldIndex < oldSelectedIndex && newIndex >= oldSelectedIndex)
            {
                VM.SelectedIndex--;
            }
            else if (oldIndex > oldSelectedIndex && newIndex <= oldSelectedIndex)
            {
                VM.SelectedIndex++;
            }

            // 指示器只跟随选中项
            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateIndicator(false);
            }), DispatcherPriority.Loaded);
            Console.WriteLine("拖拽结束");

            _isDragging = false;
            DragPopup.IsOpen = false;
        }


        private void Item_DragOver(object sender, DragEventArgs e)
        {

            if (sender is not ListBoxItem item)
                return;

            if (!e.Data.GetDataPresent(typeof(TabItemViewModel)))
                return;

            e.Effects = DragDropEffects.Move;
            e.Handled = true;

            var position = e.GetPosition(item);
            bool insertAfter = position.X > item.ActualWidth / 2;

            ShowInsertIndicator(item, insertAfter);

            Console.WriteLine("拖拽移动");

            _isDragging = true;

            DragPopup.IsOpen = true;

            var pos = e.GetPosition(this);

            DragPopup.HorizontalOffset = pos.X + 12;
            DragPopup.VerticalOffset = pos.Y + 12;
        }



        private void ShowInsertIndicator(ListBoxItem target, bool after)
        {

            InsertIndicator.Visibility = Visibility.Visible;

            var p = target.TransformToAncestor(ItemsHost)
                          .Transform(new Point(after ? target.ActualWidth : 0, 0));

            Canvas.SetLeft(InsertIndicator, p.X - 1);
            Canvas.SetTop(InsertIndicator,
                (ItemsHost.ActualHeight - InsertIndicator.Height) / 2);
        }

        private void Item_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TabItemViewModel)))
                return;

            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void Item_DragLeave(object sender, DragEventArgs e)
        {
            InsertIndicator.Visibility = Visibility.Collapsed;

            _isDragging = false;
            DragPopup.IsOpen = false;
            Mouse.SetCursor(Cursors.Hand);

            Console.WriteLine("拖拽离开");
        }

        private void Item_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            //if (_dragAdorner != null)
            //{
            //    // ⚠️ 必须用屏幕坐标 → 转成控件坐标
            //    var pos = Mouse.GetPosition(this);
            //    //Console.WriteLine(pos);
            //    //Console.WriteLine(_dragAdorner);
            //    //Console.WriteLine("============");
            //    _dragAdorner.UpdatePosition(pos.X + 12, pos.Y + 12);
            //}

            Mouse.SetCursor(Cursors.SizeAll);
            e.Handled = true;
        }


        //private Point _dragStart;
        //private bool _isDragging;

        private void Item_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("鼠标按下");
            //_dragStart = e.GetPosition(null);
            //_isDragging = false;

            if (_isDragging)
                e.Handled = true;
        }

        private void Item_PreviewMouseMove0(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || _isDragging)
                return;

            if (_dragAdorner != null)
                return;

            var pos = e.GetPosition(this);

            if (Math.Abs(pos.X - _dragStart.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(pos.Y - _dragStart.Y) < SystemParameters.MinimumVerticalDragDistance)
                return;

            _isDragging = true;

            //if (sender is ListBoxItem item &&
            //    item.DataContext is TabItemViewModel vm)
            //{
            //    var presenter = new ContentPresenter
            //    {
            //        Content = vm,
            //        ContentTemplate = ItemsHost.ItemTemplate,
            //        Opacity = 0.85,
            //        RenderTransform = new ScaleTransform(1.05, 1.05),
            //        RenderTransformOrigin = new Point(0.5, 0.5)
            //    };

            //    _adornerLayer = AdornerLayer.GetAdornerLayer(this);
            //    _dragAdorner = new DragAdorner(this, presenter);
            //    _adornerLayer?.Add(_dragAdorner);

            //    DragDrop.DoDragDrop(item, vm, DragDropEffects.Move);

            //    // 🔚 拖拽结束统一清理
            //    _adornerLayer?.Remove(_dragAdorner);
            //    _dragAdorner = null;
            //}
        }


        private void Item_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Console.WriteLine("鼠标移动");
            if (e.LeftButton != MouseButtonState.Pressed || _isDragging)
                return;

            var pos = e.GetPosition(this);

            if (Math.Abs(pos.X - _dragStart.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(pos.Y - _dragStart.Y) < SystemParameters.MinimumVerticalDragDistance)
                return;

            _isDragging = true;


            if (sender is ListBoxItem item &&
                item.DataContext is TabItemViewModel vm)
            {
                // 设置 Popup 内容
                DragPopupContent.Content = vm;
                DragPopupContent.ContentTemplate = ItemsHost.ItemTemplate;

                // 初始位置
                DragPopup.HorizontalOffset = pos.X + 12;
                DragPopup.VerticalOffset = pos.Y + 12;
                DragPopup.IsOpen = true;

                DragDrop.DoDragDrop(item, vm, DragDropEffects.Move);

                // 拖拽结束清理
                DragPopup.IsOpen = false;
                DragPopupContent.Content = null;
                _isDragging = false;
            }
        }



        private void Item_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            _dragStart = e.GetPosition(this);
            _isDragging = false;
            DragPopup.IsOpen = false;
            Console.WriteLine("鼠标抬起");
        }

        private void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            Console.WriteLine("OnGiveFeedback");
            //if (_dragAdorner != null)
            //{
            //    var pos = Mouse.GetPosition(this);
            //    Console.WriteLine(pos);
            //    Console.WriteLine(_dragAdorner);
            //    _dragAdorner.Visibility = Visibility.Visible;
            //    Console.WriteLine("============");
            //    _dragAdorner.UpdatePosition(pos.X + 12, pos.Y + 12);
            //}

            //Mouse.SetCursor(Cursors.SizeAll);
            //e.Handled = true;


            //if (!_isDragging)
            //    return;

            //var pos = Mouse.GetPosition(this);

            //DragPopup.HorizontalOffset = pos.X + 12;
            //DragPopup.VerticalOffset = pos.Y + 12;
            //Console.WriteLine(DragPopup.HorizontalOffset);
            //Console.WriteLine(DragPopup.VerticalOffset);
            //Mouse.SetCursor(Cursors.SizeAll);
            //e.Handled = true;
        }

        private void Root_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            return;
            //Console.WriteLine(_isDragging);
            //Console.WriteLine(DragPopup.IsOpen);
            if (!_isDragging || !DragPopup.IsOpen)
                return;

            var pos = e.GetPosition(this);

            DragPopup.HorizontalOffset = pos.X + 12;
            DragPopup.VerticalOffset = pos.Y + 12;
            Console.WriteLine(DragPopup.HorizontalOffset);
            Console.WriteLine(DragPopup.VerticalOffset);
        }
    }
}
