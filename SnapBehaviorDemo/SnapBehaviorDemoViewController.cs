using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace SnapBehaviorDemo
{
	public class SnapBehaviorDemoViewController : UIViewController
	{
		UIPanGestureRecognizer panGesture;
		UIImageView imageView;
		UIDynamicAnimator animator;
		UISnapBehavior snap;
		UIView snapArea;
		RectangleF snapRect;
		PointF snapPoint;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			AddSnapArea ();

			animator = new UIDynamicAnimator (View);

			// offsets used to position image relative to touch point while being dragged
			float dx = 0;
			float dy = 0;

			using (var image = UIImage.FromFile ("monkey.png")) {
				imageView = new UIImageView (image){ Frame = new RectangleF (new PointF (0, 0), image.Size) };
				imageView.UserInteractionEnabled = true;
				View.AddSubview (imageView);
			}

			panGesture = new UIPanGestureRecognizer ((pg) => {
				if ((pg.State == UIGestureRecognizerState.Began || pg.State == UIGestureRecognizerState.Changed) && (pg.NumberOfTouches == 1)) {

					if (snap != null)
						animator.RemoveBehavior (snap);

					var p0 = pg.LocationInView (View);

					if (dx == 0)
						dx = p0.X - imageView.Center.X;

					if (dy == 0)
						dy = p0.Y - imageView.Center.Y;

					var p1 = new PointF (p0.X - dx, p0.Y - dy);

					imageView.Center = p1;
				} else if (pg.State == UIGestureRecognizerState.Ended) {

					// reset offsets when dragging ends so that they will be recalculated for next touch and drag that occurs
					dx = 0;
					dy = 0;

					SnapImageIntoPlace (pg.LocationInView (View));
				}
			});

			imageView.AddGestureRecognizer (panGesture);

			View.BackgroundColor = UIColor.White;
		}

		void SnapImageIntoPlace (PointF touchPoint)
		{
			if (snapRect.Contains (touchPoint)) {
				if (snap != null)
					animator.RemoveBehavior (snap);

				snap = new UISnapBehavior (imageView, snapPoint);
				animator.AddBehavior (snap);
			}
		}

		void AddSnapArea ()
		{
			snapRect = new RectangleF (View.Center.X - 100, View.Center.Y - 100, 200, 200);
			snapPoint = new PointF (snapRect.GetMidX (), snapRect.GetMidY ());

			snapArea = new UIView (snapRect) {
				BackgroundColor = UIColor.LightGray
			};

			View.AddSubview (snapArea);
		}
	}
}



