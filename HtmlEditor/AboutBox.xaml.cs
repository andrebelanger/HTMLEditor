using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HtmlEditor
{
	/// <summary>
	/// Interaction logic for AboutBox.xaml
	/// </summary>
	public partial class AboutBox : Window
	{
		private readonly MediaPlayer _mp;
		private bool _playing;

		public AboutBox()
		{
			InitializeComponent();

			Gif.StartAnimation();

			_mp = new MediaPlayer();
			_mp.MediaEnded += (o, e) =>
			{
				_playing = false;
				Gif.StopAnimation();
				Gif.GifSource = "/HtmlEditor;component/Images/click.gif";
				Gif.StartAnimation();
			};
			_mp.Open(new Uri("Images/pbj.mp3", UriKind.Relative));
		}

		private void GifClick(object sender, EventArgs e)
		{
			if (_playing)
			{
				_mp.Stop();
				Gif.StopAnimation();
				Gif.GifSource = "/HtmlEditor;component/Images/click.gif";
				Gif.StartAnimation();
			}
			else
			{
				_mp.Play();
				Gif.StopAnimation();
				Gif.GifSource = "/HtmlEditor;component/Images/pbj.gif";
				Gif.StartAnimation();
			}

			_playing = !_playing;
		}

		private void About_Closed(object sender, EventArgs e)
		{
			_mp.Stop();
		}
	}

	class GifImage : Image
	{
		private bool _isInitialized;
		private GifBitmapDecoder _gifDecoder;
		private Int32Animation _animation;

		public int FrameIndex
		{
			get { return (int)GetValue(FrameIndexProperty); }
			set { SetValue(FrameIndexProperty, value); }
		}

		private void Initialize()
		{
			_gifDecoder = new GifBitmapDecoder(new Uri("pack://application:,,," + this.GifSource), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
			_animation = new Int32Animation(0, _gifDecoder.Frames.Count - 1,
				new Duration(
					TimeSpan.FromMilliseconds(
						((ushort)(_gifDecoder.Frames[0].Metadata as BitmapMetadata).GetQuery("/grctlext/Delay")) * 10 * _gifDecoder.Frames.Count)))
			{
				RepeatBehavior = RepeatBehavior.Forever
			};

			this.Source = _gifDecoder.Frames[0];

			_isInitialized = true;
		}

		static GifImage()
		{
			VisibilityProperty.OverrideMetadata(typeof(GifImage),
				new FrameworkPropertyMetadata(VisibilityPropertyChanged));
		}

		private static void VisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if ((Visibility)e.NewValue == Visibility.Visible)
			{
				((GifImage)sender).StartAnimation();
			}
			else
			{
				((GifImage)sender).StopAnimation();
			}
		}

		public static readonly DependencyProperty FrameIndexProperty =
			DependencyProperty.Register("FrameIndex", typeof(int), typeof(GifImage), new UIPropertyMetadata(0, new PropertyChangedCallback(ChangingFrameIndex)));

		static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
		{
			var gifImage = obj as GifImage;
			gifImage.Source = gifImage._gifDecoder.Frames[(int)ev.NewValue];
		}

		/// <summary>
		/// Defines whether the animation starts on it's own
		/// </summary>
		public bool AutoStart
		{
			get { return (bool)GetValue(AutoStartProperty); }
			set { SetValue(AutoStartProperty, value); }
		}

		public static readonly DependencyProperty AutoStartProperty =
			DependencyProperty.Register("AutoStart", typeof(bool), typeof(GifImage), new UIPropertyMetadata(false, AutoStartPropertyChanged));

		private static void AutoStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
				(sender as GifImage).StartAnimation();
		}

		public string GifSource
		{
			get { return (string)GetValue(GifSourceProperty); }
			set { SetValue(GifSourceProperty, value); }
		}

		public static readonly DependencyProperty GifSourceProperty =
			DependencyProperty.Register("GifSource", typeof(string), typeof(GifImage), new UIPropertyMetadata(string.Empty, GifSourcePropertyChanged));

		private static void GifSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			(sender as GifImage).Initialize();
			
		}

		/// <summary>
		/// Starts the animation
		/// </summary>
		public void StartAnimation()
		{
			if (!_isInitialized)
				this.Initialize();

			BeginAnimation(FrameIndexProperty, _animation);
		}

		/// <summary>
		/// Stops the animation
		/// </summary>
		public void StopAnimation()
		{
			BeginAnimation(FrameIndexProperty, null);
		}
	}
}
