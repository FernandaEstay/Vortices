using System;
using Gamelogic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Memoria
{
	public class ButtonPanel : GLMonoBehaviour
	{
		[Header("ZoomIn")]
		public Button zoomIn3DButton;
		public EventTrigger zoomInEventTrigger;

		[Header("ZoomOut")]
		public Button zoomOut3DButton;
		public EventTrigger zoomOutEventTrigger;

		[Header("Inside")]
		public Button moveCameraInside3DButton;
		public EventTrigger moveCameraInsideEventTrigger;

		[Header("Outside")]
		public Button moveCameraOutside3DButton;
		public EventTrigger moveCameraOutsideEventTrigger;

		[Header("Accept")]
		public Button accept3DButton;
		public Text acceptText;
		public EventTrigger acceptEventTrigger;
		public Color negativeAcceptNormalColor;
		public Color negativeAcceptPressedColor;
		public Color negativeAcceptHighlightedColor;

		private DIOManager _dioManager;
		private ColorBlock _originalAcceptColorBlock;

		public void Initialize(DIOManager dioManager)
		{
			_dioManager = dioManager;

			DisableZoomIn();
			zoomIn3DButton.gameObject.SetActive(_dioManager.useLeapMotion && !dioManager.usePitchGrab);

			DisableZoomOut();
			zoomOut3DButton.gameObject.SetActive(dioManager.useLeapMotion);

			EnableMoveCameraInside();
			EnableMoveCameraOutside();
			DisableAccept();

			_originalAcceptColorBlock = new ColorBlock
			{
				normalColor = accept3DButton.colors.normalColor,
				pressedColor = accept3DButton.colors.pressedColor,
				highlightedColor = accept3DButton.colors.highlightedColor,
				disabledColor = accept3DButton.colors.disabledColor,
				fadeDuration = accept3DButton.colors.fadeDuration,
				colorMultiplier = accept3DButton.colors.colorMultiplier
			};
		}

		#region Enable Disable

		public void DisableZoomIn()
		{
			DisableButton(zoomIn3DButton, zoomInEventTrigger);
		}

		public void DisableZoomOut()
		{
			DisableButton(zoomOut3DButton, zoomInEventTrigger);
		}

		public void DisableMoveCameraInside()
		{
			DisableButton(moveCameraInside3DButton, moveCameraInsideEventTrigger);
		}

		public void DisableMoveCameraOutside()
		{
			DisableButton(moveCameraOutside3DButton, moveCameraOutsideEventTrigger);
		}

		public void DisableAccept()
		{
			DisableButton(accept3DButton, acceptEventTrigger);
		}

		public void EnableZoomIn()
		{
			EnableButton(zoomIn3DButton, zoomInEventTrigger);
		}

		public void EnableZoomOut()
		{
			EnableButton(zoomOut3DButton, zoomOutEventTrigger);
		}

		public void EnableMoveCameraInside()
		{
			if (!_dioManager.InLastVisualization)
				EnableButton(moveCameraInside3DButton, moveCameraInsideEventTrigger);
			else
				DisableButton(moveCameraInside3DButton, moveCameraInsideEventTrigger);
		}

		public void EnableMoveCameraOutside()
		{
			if (!_dioManager.InFirstVisualization)
				EnableButton(moveCameraOutside3DButton, moveCameraOutsideEventTrigger);
			else
				DisableButton(moveCameraOutside3DButton, moveCameraOutsideEventTrigger);
		}

		public void EnableAccept()
		{
			if (_dioManager.lookPointerInstance.actualPitchGrabObject == null)
			{
				if (_dioManager.lookPointerInstance.posibleActualPitchGrabObject.isSelected)
					NegativeAcceptButton();
				else
					PositiveAcceptButton();
			}
			else
			{
				if (_dioManager.lookPointerInstance.actualPitchGrabObject.isSelected)
					NegativeAcceptButton();
				else
					PositiveAcceptButton();
			}

			EnableButton(accept3DButton, acceptEventTrigger);
		}

		private void EnableButton(Button button, EventTrigger eventTrigger)
		{
			button.interactable = true;
			eventTrigger.enabled = true;
		}

		private void DisableButton(Button button, EventTrigger eventTrigger)
		{
			button.interactable = false;
			eventTrigger.enabled = false;
		}

		#endregion

		public void ZoomIn()
		{
			DisableZoomIn();
			DisableMoveCameraInside();
			DisableMoveCameraOutside();

			_dioManager.lookPointerInstance.DirectZoomInCall(() =>
			{
				EnableZoomOut();
				EnableAccept();
			});
		}

		public void ZoomOut()
		{
			DisableZoomOut();
			DisableAccept();

			_dioManager.lookPointerInstance.DirectZoomOutCall(() =>
			{
				EnableMoveCameraInside();
				EnableMoveCameraOutside();
			});
		}

		public void Accept()
		{
			_dioManager.lookPointerInstance.AcceptObject();
		}

		public void Inside()
		{
			_dioManager.MoveSphereInside(1, _dioManager.initialSphereAction, _dioManager.finalSphereAction);
		}

		public void Outside()
		{
			_dioManager.MoveSphereOutside(1, _dioManager.initialSphereAction, _dioManager.finalSphereAction);
		}

		public void PositiveAcceptButton()
		{
			acceptText.text = "Marcar";

			accept3DButton.colors = _originalAcceptColorBlock;
		}

		public void NegativeAcceptButton()
		{
			acceptText.text = "Demarcar";

			var colorBlock = new ColorBlock
			{
				normalColor = negativeAcceptNormalColor,
				pressedColor = negativeAcceptPressedColor,
				highlightedColor = negativeAcceptHighlightedColor,
				disabledColor = accept3DButton.colors.disabledColor,
				fadeDuration = 0.05f,
				colorMultiplier = 1
			};

			accept3DButton.colors = colorBlock;
		}
	}
}