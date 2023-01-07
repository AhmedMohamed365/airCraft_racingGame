using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AirCraft
{
    public class HUDcontroller : MonoBehaviour
    {
        public TextMeshProUGUI placeText;

        public TextMeshProUGUI timeText;

        public TextMeshProUGUI lapText;

        public Image checkpointIcon;

        public Image checkpointArrow;

        public float indicatorLimit = .7f;

        /// <summary>
        /// The agent this HUD shows info for
        /// </summary>
        public AircraftAgent FollowAgent { get; set; }

        private racemanager raceManager;

        private void Awake()
        {
            raceManager = FindObjectOfType<racemanager>();
        }

        private void Update()
        {
            if (FollowAgent != null)
            {
                UpdatePlaceText();
                UpdateTimeText();
                UpdateLapText();
                UpdateArrow();
            }
        }

        private void UpdatePlaceText()
        {
            string place = raceManager.GetAgentPlace(FollowAgent);
            placeText.text = place;
        }

        private void UpdateTimeText()
        {
            float time = raceManager.GetAgentTime(FollowAgent);
            timeText.text = "Time " + time.ToString("0.0");
        }

        private void UpdateLapText()
        {
            int lap = raceManager.GetAgentLap(FollowAgent);
            lapText.text = "Lap " + lap + "/" + raceManager.numLaps;
        }

        private void UpdateArrow()
        {
            // Find the checkpoint within the viewport
            Transform nextCheckpoint = raceManager.GetAgentNextCheckpoint(FollowAgent);
            Vector3 viewportPoint = raceManager.ActiveCamera.WorldToViewportPoint(nextCheckpoint.transform.position);
            bool behindCamera = viewportPoint.z < 0;
            viewportPoint.z = 0f;

            Vector3 viewportCenter = new Vector3(.5f, .5f, 0f);
            Vector3 fromCenter = viewportPoint - viewportCenter;
            float halfLimit = indicatorLimit / 2f;
            bool showArrow = false;

            if (behindCamera)
            {
              
                fromCenter = -fromCenter.normalized * halfLimit;
                showArrow = true;
            }
            else
            {
                if (fromCenter.magnitude > halfLimit)
                {
                    // Limit distance from center
                    fromCenter = fromCenter.normalized * halfLimit;
                    showArrow = true;
                }
            }

            checkpointArrow.gameObject.SetActive(showArrow);
            checkpointArrow.rectTransform.rotation = Quaternion.FromToRotation(Vector3.up, fromCenter);
            checkpointIcon.rectTransform.position = raceManager.ActiveCamera.ViewportToScreenPoint(fromCenter + viewportCenter);
        }
    }
}
