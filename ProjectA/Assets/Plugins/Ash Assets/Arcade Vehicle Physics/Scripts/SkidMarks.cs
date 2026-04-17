using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeVP
{
    public class SkidMarks : MonoBehaviour
    {
        public ArcadeVehicleController carController;
        
        private TrailRenderer skidMark;
        private ParticleSystem smoke;
        private float fadeOutSpeed;
        private Material skidMaterial;

        private void Awake()
        {
            smoke = GetComponent<ParticleSystem>();
            skidMark = GetComponent<TrailRenderer>();
            skidMark.emitting = false;
            skidMark.startWidth = carController.skidWidth;
            skidMaterial = skidMark.material;
        }

        private void OnEnable()
        {
            skidMark.enabled = true;
        }

        private void OnDisable()
        {
            skidMark.enabled = false;
        }

        private void FixedUpdate()
        {
            if (carController.grounded())
            {
                if (Mathf.Abs(carController.carVelocity.x) > 10)
                {
                    fadeOutSpeed = 0f;
                    skidMaterial.color = Color.black;
                    skidMark.emitting = true;
                }
                else
                {
                    skidMark.emitting = false;
                }
            }
            else
            {
                skidMark.emitting = false;
            }

            if (!skidMark.emitting)
            {
                fadeOutSpeed += Time.deltaTime / 2;
                Color mColor = Color.Lerp(Color.black, Color.clear, fadeOutSpeed);
                skidMaterial.color = mColor;
                
                if (fadeOutSpeed > 1)
                {
                    skidMark.Clear();
                }
            }

            if (skidMark.emitting)
            {
                if (!smoke.isPlaying) smoke.Play();
            }
            else 
            { 
                if (smoke.isPlaying) smoke.Stop();
            }
        }
    }
}
