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
			private Paint hoursPaint;
			private Paint secondsTrianglePaint;
			private Paint secondsTrianglePaint2;
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

				hoursPaint = new Paint {
					Color = Color.Orange,
					TextSize = 18f
				};

				secondsTrianglePaint = new Paint {
					Color = Color.Lime,
					StrokeWidth = 3f

				};
				secondsTrianglePaint.SetStyle(Paint.Style.FillAndStroke);

				secondsTrianglePaint2 = new Paint {
					Color = Color.Bisque,
					StrokeWidth = 3f
				};

				minutesTrianglePaint = new Paint {
					Color = Color.AliceBlue,
					StrokeWidth = 4f

				};

				hoursTrianglePaint = new Paint {
					Color = Color.LimeGreen,
					StrokeWidth = 5f

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
				secondsAngle = (secondsAngle / 60f) * MathF.PI * 2f;
				float minutesAngle = (DateTime.Now.Minute / 60f) * MathF.PI * 2f;
				float hoursAngle = ((DateTime.Now.Hour % 12f) / 12f) * MathF.PI * 2f;

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
				path.LineTo(secondsX, secondsX);
				//path.MoveTo(frame.CenterX(), frame.Top);
				path.LineTo(minutesX, minutesY);
				//path.AddArc(new RectF(frame.CenterX(), frame.Top, frame.Right, frame.CenterY()), 90, 180);
				//path.ArcTo(new RectF(frame.CenterX(), top, right, frame.CenterY()), -90, 0);
				path.Close();

				canvas.DrawPath(path, secondsTrianglePaint);


				canvas.DrawLine(centerX, centerY, secondsX, secondsY, secondsTrianglePaint2);
				canvas.DrawLine(centerX, centerY, minutesX, minutesY, minutesTrianglePaint);
				canvas.DrawLine(centerX, centerY, hoursX, hoursY, hoursTrianglePaint);

				canvas.DrawCircle(centerX, centerY, 3f, dotPaint);

				var str = DateTime.Now.ToString("h:mm tt ss");
				canvas.DrawText(str,
					//(float)(frame.Left + 70),
					centerX - 100,
					//(float)(frame.Top + 80), 
					centerY,
					hoursPaint
				);

				canvas.DrawText($"sa: {secondsAngle.ToString("0.00")}, {secondsX.ToString("0.00")} {secondsY.ToString("0.00")}",
					//(float)(frame.Left + 70),
					0,
					//(float)(frame.Top + 80), 
					centerY + 50,
					hoursPaint
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
