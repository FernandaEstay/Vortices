using System;
using Gamelogic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Memoria
{
    public class ButtonPanel : PanelController
    {
        [Header("ZoomIn")]
        public Button zoomIn3DButton;
        public EventTrigger zoomInEventTrigger;

        [Header("ZoomOut")]
        public Button zoomOut3DButton;
        public EventTrigger zoomOutEventTrigger;

        [Header("Accept")]
        public Button accept3DButton;
        public Text acceptText;
        public EventTrigger acceptEventTrigger;
        public Color negativeAcceptNormalColor;
        public Color negativeAcceptPressedColor;
        public Color negativeAcceptHighlightedColor;

        private ColorBlock _originalAcceptColorBlock;

        public override void Initialize(DIOManager dioManager)
        {
            base.dioManager = dioManager;

            DisableZoomIn();        //boton elegir
            zoomIn3DButton.gameObject.SetActive(base.dioManager.useLeapMotion && !dioManager.usePitchGrab);

            DisableZoomOut();       //boton atras
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

        public void EnableAccept()
        {
            if (dioManager.lookPointerInstance.actualPitchGrabObject == null)
            {
                if (dioManager.lookPointerInstance.posibleActualPitchGrabObject.isSelected)
                    NegativeAcceptButton();
                else
                    PositiveAcceptButton();
            }
            else
            {
                if (dioManager.lookPointerInstance.actualPitchGrabObject.isSelected)
                    NegativeAcceptButton();
                else
                    PositiveAcceptButton();
            }

            EnableButton(accept3DButton, acceptEventTrigger);
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

        public void Accept()
        {
            dioManager.lookPointerInstance.AcceptObject();
        }

        public override void Inside()
        {
            dioManager.MoveSphereInside(1, dioManager.initialSphereAction, dioManager.finalSphereAction);
        }

        public override void Outside()
        {
            dioManager.MoveSphereOutside(1, dioManager.initialSphereAction, dioManager.finalSphereAction);
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
        #endregion
    }
}