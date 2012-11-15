// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Ridicak
{
	[Register ("RidicakViewController")]
	partial class RidicakViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView imageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel qView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton b1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton b2 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton b3 { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (imageView != null) {
				imageView.Dispose ();
				imageView = null;
			}

			if (qView != null) {
				qView.Dispose ();
				qView = null;
			}

			if (b1 != null) {
				b1.Dispose ();
				b1 = null;
			}

			if (b2 != null) {
				b2.Dispose ();
				b2 = null;
			}

			if (b3 != null) {
				b3.Dispose ();
				b3 = null;
			}
		}
	}
}
