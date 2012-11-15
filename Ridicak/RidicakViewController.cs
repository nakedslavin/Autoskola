using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;

using ServiceStack.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.Diagnostics;

namespace Ridicak
{
	public partial class RidicakViewController : UIViewController
	{
		#region Gesture BoilerPlate
		public Selector RightSwipeSelector
		{
			get
			{
				return new Selector("HandleRightSwipe");
			}
		}

		public Selector LeftSwipeSelector
		{
			get
			{
				return new Selector("HandleLeftSwipe");
			}
		}

		public class SwipeRecogniserDelegate : UIGestureRecognizerDelegate
		{
			public override bool ShouldReceiveTouch (UIGestureRecognizer recognizer, UITouch touch)
			{
				return true;
			}
		}
		#endregion

		[Export("HandleRightSwipe")]
		public void HandleRightSwipe(UISwipeGestureRecognizer recogniser)
		{
			Debug.WriteLine("Got a right swipe.");

			SetRandomCurrent ();
			
			RenderCurrent ();
		
		}


		[Export("HandleLeftSwipe")]
		public void HandleLeftSwipe(UISwipeGestureRecognizer recogniser)
		{
			Debug.WriteLine("Got a left swipe.");

			SetRandomCurrent ();
			
			RenderCurrent ();
		}


		public RidicakViewController () : base ("RidicakViewController", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		

		public List<Otazka> Otazky { get; set; }
		public Otazka Current { get; set; }

		private void RenderCurrent ()
		{
			if (this.Current.Url == "placeholder") {
				this.imageView.Hidden = true;
			}
			else {


				NSUrl nsurl = NSUrl.FromString (this.Current.Url);
				NSData nsdata = NSData.FromUrl (nsurl);
				UIImage uiimage = UIImage.LoadFromData (nsdata);
				this.imageView.Image = uiimage;
				this.imageView.Hidden = false;
			}

			this.b1.SetTitle (this.Current.Answer1, UIControlState.Normal);
			this.b2.SetTitle (this.Current.Answer2, UIControlState.Normal);
			if (this.Current.Answer3 == null)
				this.b3.Hidden = true;
			else {
				this.b3.Hidden = false;
				this.b3.SetTitle(this.Current.Answer3, UIControlState.Normal);
			}

			this.qView.Text = this.Current.Question;
		}

		private void GestureFuch ()
		{
			UISwipeGestureRecognizer sgrRight = new UISwipeGestureRecognizer ();
			UISwipeGestureRecognizer sgrLeft = new UISwipeGestureRecognizer ();
			sgrRight.AddTarget (this, RightSwipeSelector);
			sgrLeft.AddTarget (this, LeftSwipeSelector);
			sgrRight.Direction = UISwipeGestureRecognizerDirection.Right;
			sgrLeft.Direction = UISwipeGestureRecognizerDirection.Left;
			sgrRight.Delegate = new SwipeRecogniserDelegate ();
			sgrLeft.Delegate = new SwipeRecogniserDelegate ();
			View.AddGestureRecognizer (sgrLeft);
			View.AddGestureRecognizer (sgrRight);
		}

		private void SetRandomCurrent ()
		{
			var random = new Random ();
			var index = random.Next (0, this.Otazky.Count - 1);
			this.Current = this.Otazky.ElementAt (index);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();


			SetRandomCurrent ();

			RenderCurrent ();

			GestureFuch ();
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

