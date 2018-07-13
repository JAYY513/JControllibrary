using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace JControllibrary
{
    [TemplateVisualState(GroupName = "vsgroup", Name = "mousemoveState")]
    public class JButton : Button
    {
        static JButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(JButton), new FrameworkPropertyMetadata(typeof(JButton)));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.MouseEnter += JButton_MouseEnter;
            this.MouseLeave += JButton_MouseLeave;
            this.MouseMove += JButton_MouseMove;
        }

        private void JButton_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            UpdateVisualStates(e.GetPosition(this).X);
        }

        private void JButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //double x = e.GetPosition(this).X;
            //double y = e.GetPosition(this).Y;

            //XPosition = x > ActualWidth/2 ? ActualWidth : 0;
            //YPosition = y > ActualHeight/2 ? ActualHeight : 0;
            GradientStop gradientStop = (GradientStop)Template.FindName("offset", this);
            if (gradientStop == null) return;
            var colorAnimation = new ColorAnimation(Colors.Transparent,

                new Duration(TimeSpan.FromMilliseconds(150)))
            { BeginTime = TimeSpan.FromMilliseconds(50),
                FillBehavior = FillBehavior.HoldEnd
            };
            gradientStop.BeginAnimation(GradientStop.ColorProperty, colorAnimation);

        }

        private void JButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
           // OffsetColor = Colors.White;
          
            GradientStop gradientStop = (GradientStop)Template.FindName("offset", this);

            if (gradientStop == null) return;
            var colorAnimation = new ColorAnimation(
                Colors.White,
                new Duration(TimeSpan.FromMilliseconds(150)))
            {
                FillBehavior = FillBehavior.HoldEnd
            };
            gradientStop.BeginAnimation(GradientStop.ColorProperty, colorAnimation);
            //XPosition = e.GetPosition(this).X;
            //YPosition = e.GetPosition(this).Y;
        }
    
        public double XPosition
        {
            get { return (double)GetValue(XPositionProperty); }
            set { SetValue(XPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XPositionProperty =
            DependencyProperty.Register("XPosition", typeof(double), typeof(JButton), new PropertyMetadata(0.0));

        public double YPosition
        {
            get { return (double)GetValue(YPositionProperty); }
            set { SetValue(YPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YPositionProperty =
            DependencyProperty.Register("YPosition", typeof(double), typeof(JButton), new PropertyMetadata(0.0));



        public Color OffsetColor
        {
            
            get { return (Color)GetValue(OffsetColorProperty); }
            set { SetValue(OffsetColorProperty, value);  }
        }

        // Using a DependencyProperty as the backing store for OffsetColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetColorProperty =
            DependencyProperty.Register("OffsetColor", typeof(Color), typeof(JButton), new PropertyMetadata(Colors.Transparent));

      

        /// <summary>
        /// 按钮样式，默认值为General
        /// </summary>
        public ButtonStyles ButtonStyle
        {
            get { return (ButtonStyles)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register("ButtonStyle", typeof(ButtonStyles), typeof(JButton), new PropertyMetadata(ButtonStyles.Style4));
        public enum ButtonStyles
        {          
            Style1 = 1,
            Style2,
            Style3,
            Style4,
            Hollow,
        }

        /// <summary>
        /// 鼠标悬浮时遮罩层的背景颜色，默认值为白色。
        /// <para>仅当按钮样式为General时生效。</para>
        /// </summary>
        public SolidColorBrush CoverBrush
        {
            get { return (SolidColorBrush)GetValue(CoverBrushProperty); }
            set { SetValue(CoverBrushProperty, value); }
        }
        public static readonly DependencyProperty CoverBrushProperty = DependencyProperty.Register("CoverBrush", typeof(SolidColorBrush), typeof(JButton), new PropertyMetadata(Brushes.White));
       
        private void UpdateVisualStates(double x)
        {
            
        }

    }
}
