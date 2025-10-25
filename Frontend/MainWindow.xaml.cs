using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Frontend
{
    public partial class MainWindow
    {
        private bool _mRestoreForDragMove;
        private Point _mClickPosition;
        private Point _mClickScreenPoint;
        private double _mClickPercentHorizontal;

        public MainWindow()
        {
            InitializeComponent();
            StateChanged += MainWindow_StateChanged;
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            var isMax = WindowState == WindowState.Maximized;
            MaximizeButton.Content = isMax ? "\uE923" : "\uE922";
            RootGrid.Margin = isMax ? new Thickness(5) : new Thickness(0);
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (ResizeMode != ResizeMode.CanResize &&
                    ResizeMode != ResizeMode.CanResizeWithGrip)
                {
                    return;
                }

                MaximizeRestore();
            }
            else
            {
                if (WindowState == WindowState.Maximized)
                {
                    _mRestoreForDragMove = true;

                    _mClickPosition = e.GetPosition(this);

                    var screenPt = PointToScreen(_mClickPosition);
                    _mClickScreenPoint = screenPt;

                    var marginOffset = RootGrid.Margin.Left;
                    var adjustedClickX = (_mClickPosition.X - marginOffset);
                    var adjustedWidth = (ActualWidth - (marginOffset * 2));
                    _mClickPercentHorizontal = Math.Clamp(adjustedClickX / Math.Max(1.0, adjustedWidth), 0.0, 1.0);
                }
                else
                {
                    DragMove();
                }
            }
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_mRestoreForDragMove)
                return;

            _mRestoreForDragMove = false;

            if (e.LeftButton != MouseButtonState.Pressed) return;

            var source = PresentationSource.FromVisual(this);

            var transformFromDevice = source?.CompositionTarget?.TransformFromDevice ??
                                      System.Windows.Media.Matrix.Identity;
            var clickScreenInDips = transformFromDevice.Transform(_mClickScreenPoint);
            var targetHorizontal = RestoreBounds.Width * _mClickPercentHorizontal;

            var newLeft = clickScreenInDips.X - targetHorizontal;
            var newTop = clickScreenInDips.Y - _mClickPosition.Y;

            WindowState = WindowState.Normal;
            Left = newLeft;
            Top = newTop;
            DragMove();
        }

        private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _mRestoreForDragMove = false;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            MaximizeRestore();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MaximizeRestore()
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
    }
}