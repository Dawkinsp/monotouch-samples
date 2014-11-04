using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AVFoundation;
using Accounts;
using AdSupport;
using AddressBook;
using AssetsLibrary;
using CoreBluetooth;
using CoreLocation;
using EventKit;
using Foundation;
using UIKit;

namespace PrivacyPrompts {

	public partial class PrivacyClassesTableViewController : UITableViewController {
		public PrivacyClassesTableViewController (IntPtr handle) : base (handle)
		{
		}

<<<<<<< HEAD
		public override int RowsInSection (UITableView tableview, int section)
=======
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			PrivacyDetailViewController viewController = 
				segue.DestinationViewController as PrivacyDetailViewController;

			DataClass selected = (DataClass)(int)TableView.IndexPathForSelectedRow.Row;
			viewController.Title = selected.ToString ();

			switch (selected) {
			case DataClass.Location:
				viewController.CheckAccess = delegate {
					CheckLocationServicesAuthorizationStatus (CLLocationManager.Status);
				};
				viewController.RequestAccess = RequestLocationServicesAuthorization;
				break;
			case DataClass.Contacts:
				viewController.CheckAccess = CheckAddressBookAccess;
				viewController.RequestAccess = RequestAddressBookAccess;
				break;
			case DataClass.Calendars:
				viewController.CheckAccess = delegate {
					CheckEventStoreAccess (EKEntityType.Event);
				};
				viewController.RequestAccess = delegate {
					RequestEventStoreAccess (EKEntityType.Event);
				};
				break;
			case DataClass.Reminders:
				viewController.CheckAccess = delegate {
					CheckEventStoreAccess (EKEntityType.Reminder);
				};
				viewController.RequestAccess = delegate {
					RequestEventStoreAccess (EKEntityType.Reminder);
				};
				break;
			case DataClass.Photos:
				viewController.CheckAccess = CheckPhotosAuthorizationStatus;
				viewController.RequestAccess = delegate {
					RequestPhotoAccess (false);
					RequestPhotoAccess (true);
				};
				break;
			case DataClass.Microphone:
				viewController.CheckAccess = null;
				viewController.RequestAccess = delegate {
					RequestMicrophoneAccess (true);
					RequestMicrophoneAccess (false);
				};
				break;
			case DataClass.Bluetooth:
				viewController.CheckAccess = CheckBluetoothAccess;
				viewController.RequestAccess = RequestBluetoothAccess;
				break;
			case DataClass.Facebook:
				viewController.CheckAccess = delegate {
					CheckSocialAccountAuthorizationStatus (ACAccountType.Facebook);
				};
				viewController.RequestAccess = RequestFacebookAccess;
				break;
			case DataClass.Twitter:
				viewController.CheckAccess = delegate {
					CheckSocialAccountAuthorizationStatus (ACAccountType.Twitter);
				};
				viewController.RequestAccess = RequestTwitterAccess;
				break;
			case DataClass.SinaWeibo:
				viewController.CheckAccess = delegate {
					CheckSocialAccountAuthorizationStatus (ACAccountType.SinaWeibo);
				};
				viewController.RequestAccess = RequestSinaWeiboAccess;
				break;
			case DataClass.TencentWeibo:
				viewController.CheckAccess = delegate {
					CheckSocialAccountAuthorizationStatus (ACAccountType.TencentWeibo);
				};
				viewController.RequestAccess = RequestTencentWeiboAccess;
				break;
			case DataClass.Advertising:
				viewController.CheckAccess = AdvertisingIdentifierStatus;
				viewController.RequestAccess = null;
				break;
			}
		}

		public override nint RowsInSection (UITableView tableview, nint section)
>>>>>>> PrivacyPrompts ported to 64-bits
		{
			return 1 + (int) DataClass.Advertising;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell ("BasicCell");
			cell.TextLabel.Text = ((DataClass) (int)indexPath.Row).ToString ();
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
<<<<<<< HEAD
			PrivacyDetailViewController viewController = null;
=======
			PerformSegue ("serviceSegue", this);
		}

		#region Location methods

		public void CheckLocationServicesAuthorizationStatus (CLAuthorizationStatus status)
		{
			switch (status) {
			case CLAuthorizationStatus.NotDetermined:
				ShowAlert (DataClass.Location, "not determined");
				break;
			case CLAuthorizationStatus.Restricted:
				ShowAlert (DataClass.Location, "restricted");
				break;
			case CLAuthorizationStatus.Denied:
				ShowAlert (DataClass.Location, "denied");
				break;
			case CLAuthorizationStatus.Authorized:
				ShowAlert (DataClass.Location, "granted");
				break;
			}
		}

		public void RequestLocationServicesAuthorization ()
		{
			locationManager = new CLLocationManager ();
			locationManager.Failed += delegate {
				locationManager.StopUpdatingLocation ();
			};
			locationManager.LocationsUpdated += delegate {
				locationManager.StopUpdatingLocation ();
			};
			locationManager.AuthorizationChanged += delegate (object sender, CLAuthorizationChangedEventArgs e) {
				CheckLocationServicesAuthorizationStatus (e.Status);
			};
			locationManager.StartUpdatingLocation ();
		}

		#endregion

		#region Contacts methods

		public void CheckAddressBookAccess ()
		{
			ABAuthorizationStatus status = ABAddressBook.GetAuthorizationStatus ();

			switch (status) {
			case ABAuthorizationStatus.NotDetermined:
				ShowAlert (DataClass.Contacts, "not determined");
				break;
			case ABAuthorizationStatus.Restricted:
				ShowAlert (DataClass.Contacts, "restricted");
				break;
			case ABAuthorizationStatus.Denied:
				ShowAlert (DataClass.Contacts, "denied");
				break;
			case ABAuthorizationStatus.Authorized:
				ShowAlert (DataClass.Contacts, "granted");
				break;
			}
		}

		public void RequestAddressBookAccess ()
		{
			NSError error;
			addressBook = ABAddressBook.Create (out error);

			if (addressBook != null) {
				addressBook.RequestAccess (delegate (bool granted, NSError accessError) {
					ShowAlert (DataClass.Contacts, granted ? "granted" : "denied");
				});
			}
		}

		#endregion

		#region EventStore methods

		public void CheckEventStoreAccess (EKEntityType type)
		{
			EKAuthorizationStatus status = EKEventStore.GetAuthorizationStatus (type);
			DataClass dc = type == EKEntityType.Event ? DataClass.Calendars : DataClass.Reminders;
			switch (status) {
			case EKAuthorizationStatus.NotDetermined:
				ShowAlert (dc, "not determined");
				break;
			case EKAuthorizationStatus.Restricted:
				ShowAlert (dc, "restricted");
				break;
			case EKAuthorizationStatus.Denied:
				ShowAlert (dc, "denied");
				break;
			case EKAuthorizationStatus.Authorized:
				ShowAlert (dc, "granted");
				break;
			}
		}

		public void RequestEventStoreAccess (EKEntityType type)
		{
			if (eventStore == null)
				eventStore = new EKEventStore ();

			eventStore.RequestAccess (type, delegate (bool granted, NSError error) {
				ShowAlert (type == EKEntityType.Event ? DataClass.Calendars : DataClass.Reminders,
				           granted ? "granted" : "denied");
			});
		}

		#endregion

		#region Photos methods

		public void CheckPhotosAuthorizationStatus ()
		{
			switch (ALAssetsLibrary.AuthorizationStatus) {
			case ALAuthorizationStatus.NotDetermined:
				ShowAlert (DataClass.Photos, "not determined");
				break;
			case ALAuthorizationStatus.Restricted:
				ShowAlert (DataClass.Photos, "restricted");
				break;
			case ALAuthorizationStatus.Denied:
				ShowAlert (DataClass.Photos, "denied");
				break;
			case ALAuthorizationStatus.Authorized:
				ShowAlert (DataClass.Photos, "granted");
				break;
			}
		}

		public void RequestPhotoAccess (bool useImagePicker)
		{
			if (useImagePicker) {
				UIImagePickerController picker = new UIImagePickerController ();
				picker.Delegate = this;
				PresentViewController (picker, true, null);
			} else {
				if (assetLibrary == null) 
					assetLibrary = new ALAssetsLibrary ();

				assetLibrary.Enumerate (ALAssetsGroupType.All,
				                        delegate { }, delegate { });
			}
		}

		#endregion

		#region Microphone methods

		public void RequestMicrophoneAccess (bool usePermissionAPI)
		{
			AVAudioSession audioSession = new AVAudioSession (null);
			if (!usePermissionAPI) {
				NSError error;
				audioSession.SetCategory (AVAudioSession.CategoryRecord, out error);
			} else {
				audioSession.RequestRecordPermission (delegate(bool granted) {
					ShowAlert (DataClass.Microphone, granted ? "granted" : "denied");
				});
			}
		}

		#endregion

		#region Bluetooth methods

		public void CheckBluetoothAccess ()
		{
			if (cbManager == null)
				cbManager = new CBCentralManager ();

			CBCentralManagerState state = cbManager.State;
			switch (state) {
			case CBCentralManagerState.Unknown:
				ShowAlert (DataClass.Bluetooth, "unknown");
				break;
			case CBCentralManagerState.Unauthorized:
				ShowAlert (DataClass.Bluetooth, "denied");
				break;
			default:
				ShowAlert (DataClass.Bluetooth, "granted");
				break;
			}
		}

		public void RequestBluetoothAccess ()
		{
			if (cbManager == null)
				cbManager = new CBCentralManager ();

			if (cbManager.State == CBCentralManagerState.PoweredOn)
				cbManager.ScanForPeripherals (new CBUUID [0]);
			else {
				UIAlertView alert = new UIAlertView ("Error", "Bluetooth must be enabled",
				                                     null, "Okay", null);
				alert.Show ();
			}
		}

		#endregion

		#region Social methods

		public void CheckSocialAccountAuthorizationStatus (NSString accountTypeIdentifier)
		{
			if (accountStore == null)
				accountStore = new ACAccountStore ();

			ACAccountType socialAccount = accountStore.FindAccountType (accountTypeIdentifier);

			DataClass dataClass;

			if (accountTypeIdentifier == ACAccountType.Facebook)
				dataClass = DataClass.Facebook;
			else if (accountTypeIdentifier == ACAccountType.Twitter)
				dataClass = DataClass.Twitter;
			else if (accountTypeIdentifier == ACAccountType.SinaWeibo)
				dataClass = DataClass.SinaWeibo;
			else
				dataClass = DataClass.TencentWeibo;

			ShowAlert (dataClass, socialAccount.AccessGranted ? "granted" : "denied");
		}

		public void RequestFacebookAccess ()
		{
			if (accountStore == null)
				accountStore = new ACAccountStore ();

			ACAccountType facebookAccount = accountStore.FindAccountType (ACAccountType.Facebook);

			AccountStoreOptions options = new AccountStoreOptions () { FacebookAppId = "MY_CODE" };
			options.SetPermissions (ACFacebookAudience.Friends, new [] { "email", "user_about_me" });

			accountStore.RequestAccess (facebookAccount, options, delegate (bool granted, NSError error) {
				ShowAlert (DataClass.Facebook, granted ? "granted" : "denied");
			});
		}

		public void RequestTwitterAccess ()
		{
			if (accountStore == null)
				accountStore = new ACAccountStore ();

			ACAccountType twitterAccount = accountStore.FindAccountType (ACAccountType.Twitter);

			accountStore.RequestAccess (twitterAccount, null, delegate (bool granted, NSError error) {
				ShowAlert (DataClass.Twitter, granted ? "granted" : "denied");
			});
		}
>>>>>>> PrivacyPrompts ported to 64-bits

			DataClass selected = (DataClass)TableView.IndexPathForSelectedRow.Row;

			viewController = PrivacyDetailViewController.CreateFor (selected);
			viewController.Title = selected.ToString ();

			NavigationController.PushViewController (viewController, true);
		}
	}
}