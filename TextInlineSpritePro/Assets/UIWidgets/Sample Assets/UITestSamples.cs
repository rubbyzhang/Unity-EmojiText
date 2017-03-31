using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples {
	public class UITestSamples : MonoBehaviour
	{
		[SerializeField]
		Sprite questionIcon;

		[SerializeField]
		Sprite attentionIcon;

		public void ShowNotifySticky()
		{
			Notify.Template("NotifyTemplateSimple").Show("Sticky Notification. Click on the × above to close.", customHideDelay: 0f);
		}

		// Show 3 notification, one by one in this row:
		// Queue Notification 3.
		// Queue Notification 2.
		// Queue Notification 1.
		public void ShowNotifyStack()
		{
			Notify.Template("NotifyTemplateSimple").Show("Stack Notification 1.",
			                                             customHideDelay: 3f,
			                                             sequenceType: NotifySequence.First);
			Notify.Template("NotifyTemplateSimple").Show("Stack Notification 2.",
			                                             customHideDelay: 3f,
			                                             sequenceType: NotifySequence.First);
			Notify.Template("NotifyTemplateSimple").Show("Stack Notification 3.",
			                                             customHideDelay: 3f,
			                                             sequenceType: NotifySequence.First);
		}

		// Show 3 notification, one by one in this row:
		// Queue Notification 1.
		// Queue Notification 2.
		// Queue Notification 3.
		public void ShowNotifyQueue()
		{
			Notify.Template("NotifyTemplateSimple").Show("Queue Notification 1.",
			                                             customHideDelay: 3f,
			                                             sequenceType: NotifySequence.Last);
			Notify.Template("NotifyTemplateSimple").Show("Queue Notification 2.",
			                                             customHideDelay: 3f,
			                                             sequenceType: NotifySequence.Last);
			Notify.Template("NotifyTemplateSimple").Show("Queue Notification 3.",
			                                             customHideDelay: 3f,
			                                             sequenceType: NotifySequence.Last);
		}

		// Show only one notification and hide current notification from sequence, if exists:
		// Queue Notification 3.
		public void ShowNotifySequenceClear()
		{
			Notify.Template("NotifyTemplateSimple").Show("Stack Notification 1.",
			                                             customHideDelay: 3f,
			                                             sequenceType: NotifySequence.First);
			Notify.Template("NotifyTemplateSimple").Show("Stack Notification 2.",
			                                             customHideDelay: 3f,
			                                             sequenceType: NotifySequence.First);
			Notify.Template("NotifyTemplateSimple").Show("Stack Notification 3.",
			                                             customHideDelay: 3f,
			                                             sequenceType: NotifySequence.First,
			                                             clearSequence: true);
		}

		public void ShowNotifyAutohide()
		{
			Notify.Template("NotifyTemplateAutoHide").Show("Achievement unlocked. Hide after 3 seconds.", customHideDelay: 3f);
		}

		bool CallShowNotifyAutohide()
		{
			ShowNotifyAutohide();
			return true;
		}

		public void ShowNotifyAutohideRotate()
		{
			Notify.Template("NotifyTemplateAutoHide").Show(
				"Achievement unlocked. Hide after 4 seconds.",
				customHideDelay: 4f,
				hideAnimation: Notify.AnimationRotate
			);
		}

		public void ShowNotifyBlack()
		{
			Notify.Template("NotifyTemplateBlack").Show(
				"Another Notification. Hide after 5 seconds or click on the × above to close.",
				customHideDelay: 5f,
				hideAnimation: Notify.AnimationCollapse,
				slideUpOnHide: false
			);
		}

		bool ShowNotifyYes()
		{
			Notify.Template("NotifyTemplateAutoHide").Show("Action on 'Yes' button click.", customHideDelay: 3f);
			return true;
		}

		bool ShowNotifyNo()
		{
			Notify.Template("NotifyTemplateAutoHide").Show("Action on 'No' button click.", customHideDelay: 3f);
			return true;
		}

		public void ShowDialogSimple()
		{
			Dialog.Template("DialogTemplateSample").Show(
				title: "Simple Dialog",
				message: "Simple dialog with only close button.",
				buttons: new DialogActions(){
					{"Close", Dialog.Close},
				},
				focusButton: "Close"
			);
		}

		bool CallShowDialogSimple()
		{
			ShowDialogSimple();
			return true;
		}

		public void ShowDialogYesNoCancel()
		{
			Dialog.Template("DialogTemplateSample").Show(
				title: "Dialog Yes No Cancel",
				message: "Question?",
				buttons: new DialogActions(){
					{"Yes", ShowNotifyYes},
					{"No", ShowNotifyNo},
					{"Cancel", Dialog.Close},
				},
				focusButton: "Yes",
				icon: questionIcon
			);
		}

		public void ShowDialogExtended()
		{
			Dialog.Template("DialogTemplateSample").Show(
				title: "Another Dialog",
				message: "Same template with another position and long text.\nChange\nheight\nto\nfit\ntext.",
				buttons: new DialogActions(){
					{"Show notification", CallShowNotifyAutohide},
					{"Open simple dialog", CallShowDialogSimple},
					{"Close", Dialog.Close},
				},
				focusButton: "Show notification",
				position: new Vector3(40, -40, 0)
			);
		}

		public void ShowDialogModal()
		{
			Dialog.Template("DialogTemplateSample").Show(
				title: "Modal Dialog",
				message: "Simple Modal Dialog.",
				buttons: new DialogActions(){
					{"Close", Dialog.Close},
				},
				focusButton: "Close",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f)
			);
		}

		public void ShowDialogSignIn()
		{
			// create dialog from template
			var dialog = Dialog.Template("DialogSignInTemplateSample");
			// helper component with references to input fields
			var helper = dialog.GetComponent<DialogInputHelper>();
			// reset input fields to default
			helper.Refresh();

			// open dialog
			dialog.Show(
				title: "Sign into your Account",
				buttons: new DialogActions(){
					// on click call SignInNotify
					{"Sign in", () => SignInNotify(helper)},
					// on click close dialog
					{"Cancel", Dialog.Close},
				},
				focusButton: "Sign in",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f)
			);
		}

		// using dialog
		bool SignInNotify(DialogInputHelper helper)
		{
			// return true if Username.text and Password not empty, otherwise false
			if (!helper.Validate())
			{
				// return false to keep dialog open
				return false;
			}

			// using dialog input 
			var message = "Sign in.\nUsername: " + helper.Username.text + "\nPassword: <hidden>";
			Notify.Template("NotifyTemplateAutoHide").Show(message, customHideDelay: 3f);

			// return true to close dialog
			return true;
		}

		public void ShowDialogTreeView()
		{
			// create dialog from template
			var dialog = Dialog.Template("DialogTreeView");
			// helper component with references to input fields
			var helper = dialog.GetComponent<DialogTreeViewInputHelper>();
			
			// open dialog
			dialog.Show(
				title: "Sign into your Account",
				buttons: new DialogActions(){
				// on click close dialog
				{"Close", Dialog.Close},
			},
			focusButton: "Sign in",
			modal: true,
			modalColor: new Color(0, 0, 0, 0.8f)
			);
			
			// reset input fields to default
			helper.Refresh();
		}
	}
}