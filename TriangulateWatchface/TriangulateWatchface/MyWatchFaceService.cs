using System;
using Android.Views;
using Android.Support.Wearable.Watchface;
using Android.Service.Wallpaper;
using Android.Graphics;

namespace TriangulateWatchface {
	class MyWatchFaceService : CanvasWatchFaceService {

		public override WallpaperService.Engine OnCreateEngine() {
			return new MyWatchFaceEngine(this);
		}

		public class MyWatchFaceEngine : CanvasWatchFaceService.Engine {
			readonly CanvasWatchFaceService owner;
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
					TextSize = 18f
				};


				secondsHandPaint = new Paint {
					Color = Color.Bisque,
					StrokeWidth = 3f
				};

				minutesHandPaint = new Paint {
					Color = Color.Azure,
					StrokeWidth = 3f
				};

				hoursHandPaint = new Paint {
					Color = Color.BlanchedAlmond,
					StrokeWidth = 3f
				};


				secondsTrianglePaint = new Paint {
					Color = Color.Green,
					StrokeWidth = 1f,
					Alpha = 125

				};
				secondsTrianglePaint.SetStyle(Paint.Style.FillAndStroke);


				minutesTrianglePaint = new Paint {
					Color = Color.Blue,
					StrokeWidth = 1f,
					Alpha = 125

				};

				hoursTrianglePaint = new Paint {
					Color = Color.Red,
					StrokeWidth = 1f,
					Alpha = 125

				};

				dotPaint = new Paint {
					Color = Color.White,
					//StrokeWidth = 5f
				};

			}

			public override void OnDraw(Canvas canvas, Rect frame) {
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

				float secondsX = handLength * MathF.Sin(secondsAngle) * secondsLengthPercent + centerX;
				float secondsY = handLength * MathF.Cos(secondsAngle) * secondsLengthPercent + centerY;
				float minutesX = handLength * MathF.Sin(minutesAngle) * minutesLengthPercent + centerX;
				float minutesY = handLength * MathF.Cos(minutesAngle) * minutesLengthPercent + centerY;
				float hoursX = handLength * MathF.Sin(hoursAngle) * hoursLengthPercent + centerX;
				float hoursY = handLength * MathF.Cos(hoursAngle) * hoursLengthPercent + centerY;

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


				canvas.DrawLine(centerX, centerY, secondsX, secondsY, secondsHandPaint);
				canvas.DrawLine(centerX, centerY, minutesX, minutesY, minutesHandPaint);
				canvas.DrawLine(centerX, centerY, hoursX, hoursY, hoursHandPaint);

				canvas.DrawCircle(centerX, centerY, 3f, dotPaint);

				var str = DateTime.Now.ToString("h:mm tt ss");
				canvas.DrawText(str,
					//(float)(frame.Left + 70),
					centerX - 100,
					//(float)(frame.Top + 80), 
					centerY,
					textPaint
				);

				canvas.DrawText($"sa: {secondsAngle.ToString("0.00")}, {secondsX.ToString("0.00")} {secondsY.ToString("0.00")}",
					//(float)(frame.Left + 70),
					30,
					//(float)(frame.Top + 80), 
					centerY + 50,
					textPaint
				);


			}

			public override void OnTimeTick() {
				Invalidate();
			}

			public override void OnTouchEvent(MotionEvent e) {
				base.OnTouchEvent(e);

				Invalidate();
			}
		}
	}
}
