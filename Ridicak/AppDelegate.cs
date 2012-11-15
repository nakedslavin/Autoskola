using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using MonoTouch.Dialog;

namespace Ridicak
{

	public class Olenenok : DialogViewController {
		public Olenenok (RootElement root) : base(root) 
		{
			Autorotate = true;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			NSNotificationCenter.DefaultCenter.AddObserver("UIDeviceOrientationDidChangeNotification", DeviceRotated );
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear(animated);
			UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
		}
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear(animated);
			UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications();
		}

		private void DeviceRotated(NSNotification notification){
			OnOrientationChanged(EventArgs.Empty);
		}


		public event EventHandler OrientationChanged;

		protected virtual void OnOrientationChanged(EventArgs e) 
		{
			if (OrientationChanged != null)
				OrientationChanged(this, e);
		}

	}


	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
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

			
			SetRandom();
			SetRootSections();

		}
		
		
		[Export("HandleLeftSwipe")]
		public void HandleLeftSwipe(UISwipeGestureRecognizer recogniser)
		{
			SetRandom();
			SetRootSections();
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
			viewController.View.AddGestureRecognizer (sgrLeft);
			viewController.View.AddGestureRecognizer (sgrRight);
		}

		public static List<Otazka> Otazky { get; set; }
		public static Otazka Current { get; set; }


		//ew Uri("file://"+System.IO.Path.GetFullPath("clusterfuckthree.JPG"))
		// class-level declarations
		UIWindow window;
		static Olenenok viewController;
		DB db = new DB();

		static Otazka GetOne ()
		{
			var otazky = Otazky;
			var random = new Random ();
			var index = random.Next (0, otazky.Count - 1);
			var current = otazky.ElementAt (index);
			current.Answer1 = FirstCharToUpper(current.Answer1.TrimStart(new char[]{ '1','2','3',' ',')'}));
			current.Answer2 = FirstCharToUpper(current.Answer2.TrimStart(new char[]{ '1','2','3',' ',')'}));
			current.Answer3 = current.Answer3 == null ? null : FirstCharToUpper(current.Answer3.TrimStart(new char[]{ '1','2','3',' ',')'}));
			return current;

		}

		static void SetRandom() {
			Current = GetOne();
		}

		static StyledMultilineElement MakeElement(string caption, bool bold = false) {
			UIFont font = UIFont.SystemFontOfSize(14f);
			if(bold)
				font = UIFont.BoldSystemFontOfSize(14f);

			var q = new StyledMultilineElement(caption);
			q.Font = font;
			return q;
		}

		static RootElement root = null;

		static List<Section> sections = new List<Section>();
		static Section picSection;

		static void SetRootSections() {
			if(root == null) {
				root = new RootElement(null); 
				sections = new List<Section>();
			}
			
			else {
				sections.ForEach(s =>
				                 root.Remove(s, UITableViewRowAnimation.Fade));
				sections = new List<Section>();
				
			}

			var queSection = new Section() { MakeElement(Current.Question, true) };

			var answers = new Section();


			picSection = new Section();
			if(Current.Url != "placeholder") {

				float scale = 
					(UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || 
					 UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight) ? 1.04f : 1.6f;

				var i = UIImage.FromBundle("Images/"+ Current.Url);
				var i2 = new UIImage(i.CGImage, scale, UIImageOrientation.Up);

				var img = new UIImageView(i2);

				i.Dispose();
				i2.Dispose();

				picSection.Add(img);

				root.Add(picSection);
				sections.Add(picSection);


			}
			

			root.Add(queSection);
			sections.Add(queSection);

			var a1 = MakeElement(Current.Answer1);
			var a2 = MakeElement(Current.Answer2);
			var a3 = MakeElement(Current.Answer3);
			
			answers.Add(a1);
			answers.Add(a2); 

			if(Current.Answer3 != null) {
				answers.Add(a3);
			}
			
			root.Add(answers);
			sections.Add(answers);
			
			NSAction action = () => {
				if(Current.Right == 1) {
					a1.Accessory = UITableViewCellAccessory.Checkmark;
					a1.GetImmediateRootElement().Reload(answers, UITableViewRowAnimation.Fade);
				}
				if(Current.Right == 2) {
					a2.Accessory = UITableViewCellAccessory.Checkmark;
					a2.GetImmediateRootElement().Reload(answers, UITableViewRowAnimation.Fade);
				}
				if(Current.Right == 3) {
					a3.Accessory = UITableViewCellAccessory.Checkmark;
					a3.GetImmediateRootElement().Reload(answers, UITableViewRowAnimation.Fade);
				}
			};
			
			a1.Tapped += action;
			a2.Tapped += action;
			a3.Tapped += action;

		}
		public static string FirstCharToUpper(string input)
		{
			if (String.IsNullOrEmpty(input))
				throw new ArgumentException("ARGH!");
			return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
		}

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			Otazky = db.Get();
			SetRandom();


			SetRootSections();

			var o = new Olenenok(root);

			viewController = o;

			window.RootViewController = viewController;

			viewController.OrientationChanged += (object sender, EventArgs e) => {
				if(Current.Url != "placeholder") {
				
				float _scale = 
					(UIDevice.CurrentDevice.Orientation.ToString() == UIInterfaceOrientation.LandscapeLeft.ToString() || 
					 UIDevice.CurrentDevice.Orientation.ToString() == UIInterfaceOrientation.LandscapeRight.ToString()) ? 1.04f : 1.6f;
				
				var _i = UIImage.FromBundle("Images/"+ Current.Url);
				var _i2 = new UIImage(_i.CGImage, _scale, UIImageOrientation.Up);
				
				var _img = new UIImageView(_i2);
				
				
				picSection.Clear();
				picSection.Add(_img);
				
				_i.Dispose(); _i2.Dispose();
				
				//picSection.Reload(picSection, UITableViewRowAnimation.Fade);
				root.Reload(picSection, UITableViewRowAnimation.Fade);
				}
				
			};


			GestureFuch();


			window.MakeKeyAndVisible ();

			
			return true;
		}
	}

	public class Otazka {
		public string Url { get; set; }
		public string Category { get; set; }
		public string Question { get; set; }
		public string Answer1 { get; set; }
		public string Answer2 { get; set; }
		public string Answer3 { get; set; }
		public int Right { get; set; }
	}


	public class DB {

		public List<Otazka> Get() {
			var file = System.IO.File.ReadAllText("Otazky.js");


			JsonArrayObjects jobjects = JsonArrayObjects.Parse(file);
			List<Otazka> lotazky = new List<Otazka>();
			
			lotazky = jobjects.Select(x => {
				var o = new Otazka { 
					Url =  x["Url"],
					Question = x["Question"],
					Answer1 = x["Answer1"],
					Answer2 = x["Answer2"] };
				o.Right = Int32.Parse(x["Right"]);
				o.Category = x["Category"];
				string answer3 = string.Empty;
				
				x.TryGetValue("Answer3", out answer3);
				o.Answer3 = answer3;
				return o;
			}).ToList();
			
			return lotazky;
		}

	}

	public class KartinkaElement: MonoTouch.Dialog.StyledStringElement, IElementSizing {
 		
		private float h;


		public KartinkaElement (float height = 0) : base (string.Empty)
		{
			h = height;
			BackgroundColor = UIColor.White;
		}

		public float GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			/*
			float margin = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? 40f : 110f;
			SizeF size = new SizeF (tableView.Bounds.Width - margin, float.MaxValue);
			UIFont font = UIFont.BoldSystemFontOfSize (17);
			string c = Caption;
			// ensure the (single-line) Value will be rendered inside the cell
			if (String.IsNullOrEmpty (c) && !String.IsNullOrEmpty (Value))
				c = " ";
			return tableView.StringSize (c, font, size, UILineBreakMode.WordWrap).Height + 10;
			*/
			return h;
		}
	}

}