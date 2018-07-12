using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JControllibrary
{
    public class HollowText : FrameworkElement, IAddChild
    {
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static HollowText()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HollowText), new FrameworkPropertyMetadata(typeof(HollowText)));
        }

        BitmapSource bitmapSource;
        Timer timer = new Timer();
        public HollowText()
        {
            this.Loaded += HollowText_Loaded;
            ClipToBounds = true;
        }

        double waveWidth;
        double FPS = 60;
        private void HollowText_Loaded(object sender, RoutedEventArgs e)
        {
            tempValue = ValueNum;
            tempY = Y;
            bitmapSource = new BitmapImage(new Uri("pack://application:,,,/JControllibrary;component/Resources/wave.png", UriKind.Absolute));
            waveWidth = ActualWidth * 0.25;

            timer.Interval = FPS;
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        double x;
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.InvokeAsync(() => {
                if (bitmapSource != null && x >= ActualWidth * 1.5 * 0.25)
                {
                    x = 0;
                }
                x += 1;

                if (tempValue < ValueNum)
                {
                    tempValue += (textHeight / (double)(Maximum - Minimum) * (double)ValueNum / 1500.0 * FPS);
                    tempValue = Math.Min(tempValue, ValueNum);
                    tempY = Y;
                }
                else if (tempValue > ValueNum)
                {
                    tempValue -= (textHeight / (double)(Maximum - Minimum) * ((Maximum - Minimum) - (double)ValueNum) / 1500.0 * FPS);
                    tempValue = Math.Max(tempValue, ValueNum);
                    tempY = Y;
                }
                InvalidateVisual();
            });
        }

        #region Private Fields
        /// <summary>
        /// 文字几何形状
        /// </summary>
        private Geometry m_TextGeometry;
        #endregion

        #region Private Methods

        /// <summary>     
        /// 当依赖项属性改变文字无效时，创建新的空心文字对象来显示。
        /// </summary>     
        /// <param name="d"></param>     
        /// <param name="e"></param>     
        private static void OnOutlineTextInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Convert.ToString(e.NewValue) != Convert.ToString(e.OldValue))
            {
                ((HollowText)d).CreateText();
            }
        }
        #endregion

        /// <summary>
        /// New outlinetext
        /// </summary>
        public void SetTempValueNum(int value)
        {
            tempValue = value;
            tempY = Y;
            ValueNum = value;
        }


        #region FrameworkElement Overrides
        double tempValue;
        DrawingVisual boardVisual = new DrawingVisual();
        double tempY;
        /// <summary>     
        /// 重写绘制文字的方法。   
        /// </summary>     
        /// <param name="drawingContext">空心文字控件的绘制上下文。</param>     
        protected override void OnRender(DrawingContext drawingContext)
        {
            CreateText();
            if (_IsProgressBar)
            {
                if (ValueNum < Maximum)
                {
                    ImageBrush imageBrush = new ImageBrush(bitmapSource)
                    {
                        TileMode = TileMode.Tile,
                        Viewport = new Rect(0, 0, 0.25, 1)
                    };
                    drawingContext.DrawRectangle(imageBrush, null, new Rect(-waveWidth * 2 + x, tempY - textHeight / 2, ActualWidth + waveWidth * 2, textHeight * 2));

                    RectangleGeometry rectangleGeometry = new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight), 0, 0);
                    CombinedGeometry combinedGeometry = new CombinedGeometry
                    {
                        Geometry1 = m_TextGeometry,
                        Geometry2 = rectangleGeometry,
                        GeometryCombineMode = GeometryCombineMode.Xor
                    };
                    drawingContext.DrawGeometry(Fill, new Pen(Stroke, StrokeThickness), combinedGeometry);

                }
                else
                {
                    drawingContext.DrawRectangle(Fill, new Pen(Stroke, StrokeThickness), new Rect(0, 0, ActualWidth, ActualHeight));
                    drawingContext.DrawGeometry(Brushes.White, null, m_TextGeometry);
                }
            }
            else
            {
                ImageBrush imageBrush = new ImageBrush(bitmapSource)
                {
                    TileMode = TileMode.Tile,
                    Viewport = new Rect(0, 0, 0.25, 1)
                };
                drawingContext.DrawRectangle(imageBrush, null, new Rect(-waveWidth * 2 + x, (ActualHeight - textHeight) / 2, ActualWidth + waveWidth * 2, textHeight));
                
                RectangleGeometry rectangleGeometry = new RectangleGeometry(new Rect(-1, -1, ActualWidth + 2, ActualHeight + 2), 0, 0);

                CombinedGeometry combinedGeometry = new CombinedGeometry
                {
                    Geometry1 = m_TextGeometry,
                    Geometry2 = rectangleGeometry,
                    GeometryCombineMode = GeometryCombineMode.Xor
                };
                drawingContext.DrawGeometry(Fill, new Pen(Stroke, StrokeThickness), combinedGeometry);
            }
        }

        double textHeight;
        /// <summary>     
        /// 基于格式化文字创建文字的几何轮廓。    
        /// </summary>     
        public void CreateText()
        {
            FontStyle fontStyle = FontStyles.Normal;
            FontWeight fontWeight = FontWeights.Medium;
            if (Bold == true)
                fontWeight = FontWeights.Bold;
            if (Italic == true)
                fontStyle = FontStyles.Italic;
            // 基于设置的属性集创建格式化的文字。        
            FormattedText formattedText = new FormattedText(
                Text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                new Typeface(Font, fontStyle, fontWeight, FontStretches.Normal),
                FontSize, Brushes.White)
            {
                Trimming = TextTrimming.CharacterEllipsis
            };
            // 同步控件大小等于字体大小
            Width = formattedText.Width;
            Height= formattedText.Height;

            // 基于格式化文字的大小设置空心文字的大小。         
            textHeight = formattedText.Height;

            HorizontalAlignment horizontalAlignment = (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty);

            // 创建表示文字的几何对象。        
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    {
                        m_TextGeometry = formattedText.BuildGeometry(new Point((ActualWidth - formattedText.Width) / 2, (ActualHeight - textHeight) / 2));
                        break;
                    }
                default:
                    {
                        m_TextGeometry = formattedText.BuildGeometry(new Point(0, 0));
                        break;
                    }
            }
        }
        #endregion

        private bool _IsProgressBar = true;

        public bool IsProgressBar
        {
            get { return _IsProgressBar = true; ; }
            set { _IsProgressBar = value; }
        }


        public static HorizontalAlignment GetHorizontalContentAlignment(DependencyObject obj)
        {

            return (HorizontalAlignment)obj.GetValue(HorizontalContentAlignmentProperty);
        }

        public static void SetHorizontalContentAlignment(DependencyObject obj, HorizontalAlignment value)
        {
            obj.SetValue(HorizontalContentAlignmentProperty, value);
        }

        // Using a DependencyProperty as the backing store for HorizontalContentAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.RegisterAttached("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(HollowText), new PropertyMetadata(HorizontalAlignment.Center));
        
        public int ValueNum
        {
            get { return (int)GetValue(ValueNumProperty); }
            set { SetValue(ValueNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueNumProperty =
            DependencyProperty.RegisterAttached("ValueNum", typeof(int), typeof(HollowText), new PropertyMetadata(0, OnValueNumChanged));
        
        private static void OnValueNumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HollowText HollowText = (HollowText)d;
            HollowText.InvalidateVisual();
        }

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.RegisterAttached("Maximum", typeof(int), typeof(HollowText), new PropertyMetadata(100, OnMaximumChanged));

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.RegisterAttached("Minimum", typeof(int), typeof(HollowText), new PropertyMetadata(0, OnMinimumChanged));

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }


        public double Y
        {
            get
            {
                double y = -(textHeight) / (double)(Maximum - Minimum) * (double)tempValue + (ActualHeight - textHeight * 2) / 2 + textHeight;
                return y;
            }
        }
        
        #region DependencyProperties



        /// <summary>     
        /// 指定字体是否加粗。   
        /// </summary>     
        public bool Bold
        {
            get { return (bool)GetValue(BoldProperty); }
            set { SetValue(BoldProperty, value); }
        }
        /// <summary>     
        /// 指定字体是否加粗依赖属性。    
        /// </summary>     
        public static readonly DependencyProperty BoldProperty = DependencyProperty.Register(
            "Bold", typeof(bool), typeof(HollowText),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnOutlineTextInvalidated), null));

        /// <summary>     
        /// 指定填充字体的画刷颜色。    
        /// </summary>     
        /// 
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        /// <summary>     
        /// 指定填充字体的画刷颜色依赖属性。    
        /// </summary>     
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            "Fill", typeof(Brush), typeof(HollowText),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.LightSteelBlue), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnOutlineTextInvalidated), null));

        /// <summary>     
        /// 指定文字显示的字体。   
        /// </summary>    
        public FontFamily Font
        {
            get { return (FontFamily)GetValue(FontProperty); }
            set { SetValue(FontProperty, value); }
        }
        /// <summary>     
        /// 指定文字显示的字体依赖属性。     
        /// </summary>     
        public static readonly DependencyProperty FontProperty = DependencyProperty.Register(
            "Font", typeof(FontFamily), typeof(HollowText),
            new FrameworkPropertyMetadata(new FontFamily("Arial"), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnOutlineTextInvalidated), null));

        /// <summary>     
        /// 指定字体大小。
        /// </summary>     
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        /// <summary>     
        /// 指定字体大小依赖属性。     
        /// </summary>     
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize", typeof(double), typeof(HollowText),
            new FrameworkPropertyMetadata((double)18, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnOutlineTextInvalidated), null));

        /// <summary>     
        /// 指定字体是否显示斜体字体样式。  
        /// </summary>     
        public bool Italic
        {
            get { return (bool)GetValue(ItalicProperty); }
            set { SetValue(ItalicProperty, value); }
        }
        /// <summary>     
        /// 指定字体是否显示斜体字体样式依赖属性。   
        /// </summary>     
        public static readonly DependencyProperty ItalicProperty = DependencyProperty.Register(
            "Italic", typeof(bool), typeof(HollowText),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnOutlineTextInvalidated), null));

        /// <summary>     
        /// 指定绘制空心字体边框画刷的颜色。    
        /// </summary>     
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        /// <summary>     
        /// 指定绘制空心字体边框画刷的颜色依赖属性。    
        /// </summary>     
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke", typeof(Brush), typeof(HollowText),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Teal), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnOutlineTextInvalidated), null));

        /// <summary>     
        /// 指定空心字体边框大小。  
        /// </summary>     
        public ushort StrokeThickness
        {
            get { return (ushort)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        /// <summary>     
        /// 指定空心字体边框大小依赖属性。      
        /// </summary>     
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness",
            typeof(ushort), typeof(HollowText),
            new FrameworkPropertyMetadata((ushort)0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnOutlineTextInvalidated), null));

        /// <summary>    
        /// 指定要显示的文字字符串。  
        /// </summary>     
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>     
        /// 指定要显示的文字字符串依赖属性。  
        ///  </summary>    
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(HollowText),
            new FrameworkPropertyMetadata("",
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnOutlineTextInvalidated),
                null));

        #endregion
        
        #region Public Methods

        /// <summary>
        /// 添加子对象。
        /// </summary>
        /// <param name="value">要添加的子对象。</param>
        public void AddChild(Object value)
        { }

        /// <summary>
        /// 将节点的文字内容添加到对象。
        /// </summary>
        /// <param name="value">要添加到对象的文字。</param>
        public void AddText(string value)
        {
            Text = value;
        }
        
        #endregion
    }
}
