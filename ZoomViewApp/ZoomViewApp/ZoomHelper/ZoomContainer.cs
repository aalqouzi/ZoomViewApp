using System;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ZoomGesture
{
	public class ZoomContainer : ContentView
	{
		//pinch-to-zoom vars
		double currentScale = 1;
		double startScale = 1;
		double xOffset = 0;
		double yOffset = 0;

		//pan vars
		double x, y;

		int doubleTapCount = 0;

		public ZoomContainer ()
		{
			var pinchGesture = new PinchGestureRecognizer ();
			pinchGesture.PinchUpdated += OnPinchUpdated;
			GestureRecognizers.Add (pinchGesture);

			// Set PanGestureRecognizer.TouchPoints to control the 
			// number of touch points needed to pan
			var panGesture = new PanGestureRecognizer();
			panGesture.PanUpdated += OnPanUpdated;
			GestureRecognizers.Add(panGesture);

			var doubleTap = new TapGestureRecognizer();
			doubleTap.NumberOfTapsRequired = 2;
            doubleTap.Tapped += DoubleTap_Tapped;
			GestureRecognizers.Add(doubleTap);
		}

        private void DoubleTap_Tapped(object sender, EventArgs e)
        {
			if (doubleTapCount == 0)
			{
				// Apply scale factor
				Content.Scale *= 2;
				currentScale = Content.Scale * 2;

				doubleTapCount = 1;
			}
			else
            {
				doubleTapCount = 0;
				Content.Scale = 1;
				currentScale = 1;

				//reset view to original position
				Content.TranslationX = 0;
				Content.TranslationY = 0;
            }
		}

		void OnPinchUpdated (object sender, PinchGestureUpdatedEventArgs e)
		{
			if (e.Status == GestureStatus.Started) {
				// Store the current scale factor applied to the wrapped user interface element,
				// and zero the components for the center point of the translate transform.
				startScale = Content.Scale;
				Content.AnchorX = 0;
				Content.AnchorY = 0;
			}
			if (e.Status == GestureStatus.Running) {
				// Calculate the scale factor to be applied.
				currentScale += (e.Scale - 1) * startScale;
				currentScale = Math.Max (1, currentScale);

				// The ScaleOrigin is in relative coordinates to the wrapped user interface element,
				// so get the X pixel coordinate.
				double renderedX = Content.X + xOffset;
				double deltaX = renderedX / Width;
				double deltaWidth = Width / (Content.Width * startScale);
				double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

				// The ScaleOrigin is in relative coordinates to the wrapped user interface element,
				// so get the Y pixel coordinate.
				double renderedY = Content.Y + yOffset;
				double deltaY = renderedY / Height;
				double deltaHeight = Height / (Content.Height * startScale);
				double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

				// Calculate the transformed element pixel coordinates.
				double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
				double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

				// Apply translation based on the change in origin.
				Content.TranslationX = targetX.Clamp (-Content.Width * (currentScale - 1), 0);
				Content.TranslationY = targetY.Clamp (-Content.Height * (currentScale - 1), 0);

				// Apply scale factor
				Content.Scale = currentScale;
			}
			if (e.Status == GestureStatus.Completed) {
				// Store the translation delta's of the wrapped user interface element.
				xOffset = Content.TranslationX;
				yOffset = Content.TranslationY;
			}
		}

		void OnPanUpdated(object sender, PanUpdatedEventArgs e)
		{
			if (Content.Scale == 1)
				return;

			double screenWidth = DeviceDisplay.MainDisplayInfo.Width;
			double screenHeight = DeviceDisplay.MainDisplayInfo.Height;

			switch (e.StatusType)
			{
				case GestureStatus.Running:
					// Translate and ensure we don't pan beyond the wrapped user interface element bounds.
					Content.TranslationX = Math.Max(Math.Min(0, x + e.TotalX), -Math.Abs(Content.Width - screenWidth));
					Content.TranslationY = Math.Max(Math.Min(0, y + e.TotalY), -Math.Abs(Content.Height - screenHeight));
					break;

				case GestureStatus.Completed:
					// Store the translation applied during the pan
					x = Content.TranslationX;
					y = Content.TranslationY;
					break;
			}
		}
	}
}
