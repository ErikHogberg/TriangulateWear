using System;
using System.Threading;
using Android.Views;
using Android.Support.Wearable.Watchface;
using Android.Service.Wallpaper;
using Android.Graphics;
using Android.Text.Format;
//using System.Timers;
using Java.Util.Concurrent;

//using Android.App;
//using Android.Util;
//using Android.Content;
//using Android.OS;
//using Android.Graphics.Drawables;

namespace TriangulateWatchface {
	class MyWatchFaceService : CanvasWatchFaceService {

		public override WallpaperService.Engine OnCreateEngine() {
			return new MyWatchFaceEngine(this);
		}

		public class MyWatchFaceEngine : CanvasWatchFaceService.Engine {

			static long InterActiveUpdateRateMs = TimeUnit.Seconds.ToMillis(1);

			readonly CanvasWatchFaceService owner;

			//public Time time;
			Timer timerSeconds;

			private Paint textPaint;
			private Paint secondsHandPaint;
			private Paint minutesHandPaint;
			private Paint hoursHandPaint;
			private Paint secondsTrianglePaint;
			private Paint minutesTrianglePaint;
			private Paint hoursTrianglePaint;
			private Paint dotPaint;

			float padding = 5f;


			public MyWatchFaceEngine(CanvasWatchFaceService owner) : base(owner) {
				this.owner = owner;
			}

			public override void OnCreate(ISurfaceHolder holder) {
				base.OnCreate(holder);

				SetWatchFaceStyle(new WatchFaceStyle.Builder(owner)
					//.SetCardPeekMode(WatchFaceStyle.PeekModeShort)
					//.SetBackgroundVisibility(WatchFaceStyle.BackgroundVisibilityInterruptive)
					//.SetShowSystemUiTime(false)
					.Build());

				textPaint = new Paint {
					Color = Color.Orange,
					TextSize = 18f,
					AntiAlias = true,
					TextAlign = Paint.Align.Left
				};


				secondsHandPaint = new Paint {
					Color = Color.Bisque,
					StrokeWidth = 3f,
					AntiAlias = true
				};

				minutesHandPaint = new Paint {
					Color = Color.Azure,
					StrokeWidth = 3f,
					AntiAlias = true
				};

				hoursHandPaint = new Paint {
					Color = Color.BlanchedAlmond,
					StrokeWidth = 3f,
					AntiAlias = true
				};


				secondsTrianglePaint = new Paint {
					Color = Color.Green,
					StrokeWidth = 1f,
					Alpha = 125,
					AntiAlias = true

				};
				secondsTrianglePaint.SetStyle(Paint.Style.FillAndStroke);


				minutesTrianglePaint = new Paint {
					Color = Color.Blue,
					StrokeWidth = 1f,
					Alpha = 125,
					AntiAlias = true

				};
				minutesTrianglePaint.SetStyle(Paint.Style.FillAndStroke);

				hoursTrianglePaint = new Paint {
					Color = Color.Red,
					StrokeWidth = 1f,
					Alpha = 125,
					AntiAlias = true
				};
				hoursTrianglePaint.SetStyle(Paint.Style.FillAndStroke);

				dotPaint = new Paint {
					Color = Color.White,
					//StrokeWidth = 5f
				};

				//time = new Time();

				// Start a timer for redrawing the click face (second hand)
				// every second.
				// How to stop the timer? It shouldn't run in ambient mode...
				timerSeconds = new Timer(
					new TimerCallback(state => {
						// TODO: dont update every second when ambient
						Invalidate();
					}),
					null,
					TimeSpan.FromMilliseconds(InterActiveUpdateRateMs),
					TimeSpan.FromMilliseconds(InterActiveUpdateRateMs)
				);
			}

			public override void OnDraw(Canvas canvas, Rect frame) {

				// TODO: ambient mode draw

				canvas.DrawColor(Color.DarkGray);

				float top = frame.Top - padding;
				//float right = frame.Right - padding;
				//float bottom = frame.Bottom - padding;
				//float left = frame.Left - padding;

				float centerX = frame.CenterX();
				float centerY = frame.CenterY();

				float secondsAngle = DateTime.Now.Second;
				if (!IsInAmbientMode)
					secondsAngle += DateTime.Now.Millisecond * .001f;
				secondsAngle = -(secondsAngle / 60f) * MathF.PI * 2f;
				float minutesAngle = -(DateTime.Now.Minute / 60f) * MathF.PI * 2f;
				float hoursAngle = -((DateTime.Now.Hour % 12f) / 12f) * MathF.PI * 2f;

				float secondsLengthPercent = .8f;
				float minutesLengthPercent = .7f;
				float hoursLengthPercent = .6f;

				float handLength = top - frame.CenterX();

				string hoursText = $"{DateTime.Now.Hour}h";
				string minutesText = $"{DateTime.Now.Minute}m";
				string secondsText = $"{DateTime.Now.Second}s";

				Rect hoursTextRect = new Rect();
				textPaint.GetTextBounds(hoursText, 0, hoursText.Length, hoursTextRect);
				Rect minutesTextRect = new Rect();
				textPaint.GetTextBounds(hoursText, 0, minutesText.Length, minutesTextRect);
				Rect secondsTextRect = new Rect();
				textPaint.GetTextBounds(hoursText, 0, secondsText.Length, secondsTextRect);

				float secondsX = handLength * MathF.Sin(secondsAngle) * secondsLengthPercent + centerX - secondsTextRect.CenterX();
				float secondsY = handLength * MathF.Cos(secondsAngle) * secondsLengthPercent + centerY  - secondsTextRect.CenterY();
				float minutesX = handLength * MathF.Sin(minutesAngle) * minutesLengthPercent + centerX - minutesTextRect.CenterX();
				float minutesY = handLength * MathF.Cos(minutesAngle) * minutesLengthPercent + centerY - minutesTextRect.CenterY();
				float hoursX = handLength * MathF.Sin(hoursAngle) * hoursLengthPercent + centerX - hoursTextRect.CenterX();
				float hoursY = handLength * MathF.Cos(hoursAngle) * hoursLengthPercent + centerY - hoursTextRect.CenterY();

				//canvas.DrawVertices(Canvas.VertexMode.TriangleFan, 3, )
				Path path = new Path();
				path.SetFillType(Path.FillType.EvenOdd);
				path.MoveTo(frame.CenterX(), frame.CenterY());
				path.LineTo(secondsX, secondsY);
				path.LineTo(minutesX, minutesY);
				path.Close();

				canvas.DrawPath(path, secondsTrianglePaint);

				path.Reset();
				path.MoveTo(frame.CenterX(), frame.CenterY());
				path.LineTo(minutesX, minutesX);
				path.LineTo(hoursX, hoursY);
				path.Close();
				canvas.DrawPath(path, minutesTrianglePaint);

				path.Reset();
				path.MoveTo(frame.CenterX(), frame.CenterY());
				path.LineTo(hoursX, hoursY);
				path.LineTo(secondsX, secondsY);
				path.Close();
				canvas.DrawPath(path, hoursTrianglePaint);

				// IDEA: triangle to touch location

				canvas.DrawLine(centerX, centerY, secondsX, secondsY, secondsHandPaint);
				canvas.DrawLine(centerX, centerY, minutesX, minutesY, minutesHandPaint);
				canvas.DrawLine(centerX, centerY, hoursX, hoursY, hoursHandPaint);

				canvas.DrawCircle(centerX, centerY, 3f, dotPaint);

				//var str = DateTime.Now.ToString("h:mm tt ss");
				//canvas.DrawText(str,
				//	//(float)(frame.Left + 70),
				//	centerX - 100,
				//	//(float)(frame.Top + 80), 
				//	centerY,
				//	textPaint
				//);

				//canvas.DrawText($"sa: {secondsAngle.ToString("0.00")}, {secondsX.ToString("0.00")} {secondsY.ToString("0.00")}",
				//	//(float)(frame.Left + 70),
				//	30,
				//	//(float)(frame.Top + 80), 
				//	centerY + 50,
				//	textPaint
				//);

				// TODO: better hand texts positioning
				// TODO: circles around hand texts

				canvas.DrawText(hoursText,
					hoursX,
					hoursY,
					textPaint
				);

				canvas.DrawText(minutesText,
					minutesX,
					minutesY,
					textPaint
				);

				canvas.DrawText(secondsText,
					secondsX,
					secondsY,
					textPaint
				);

			}

			public override void OnTimeTick() {
				base.OnTimeTick();
				Invalidate();
			}

			public override void OnTouchEvent(MotionEvent e) {
				base.OnTouchEvent(e);

				Invalidate();
			}

			public override void OnVisibilityChanged(bool visible) {
				base.OnVisibilityChanged(visible);

				//if (visible) {
				//	RegisterTimezoneReceiver();
				//	time.Clear(Java.Util.TimeZone.Default.ID);
				//	time.SetToNow();
				//} else
				//	UnregisterTimezoneReceiver();
			}
		}
	}
}
