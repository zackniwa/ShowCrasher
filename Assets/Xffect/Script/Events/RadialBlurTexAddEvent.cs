using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xft
{
    public class RadialBlurTexAddEvent : XftEvent
    {
        protected Camera m_camera;
        protected XftRadialBlurTexAdd m_radialBlurComp;
        protected bool m_supported = true;

        
        public RadialBlurTexAddEvent (XftEventComponent owner)
            : base(XftEventType.CameraRadialBlurMask, owner)
        {

        }

        protected void FindCamera ()
        {
            int layerMask = 1 << m_owner.gameObject.layer;
            Camera[] cameras = GameObject.FindSceneObjectsOfType (typeof(Camera)) as Camera[];
            for (int i = 0, imax = cameras.Length; i < imax; ++i) {
                Camera cam = cameras [i];

                if ((cam.cullingMask & layerMask) != 0) {
                    m_camera = cam;
                    return;
                }
            }
            Debug.LogError ("can't find proper camera for RadialBlurEvent!");
        }

        protected void ToggleCameraComponent (bool flag)
        {
            m_radialBlurComp = m_camera.gameObject.GetComponent<XftRadialBlurTexAdd> ();
            if (m_radialBlurComp == null) {
                m_radialBlurComp = m_camera.gameObject.AddComponent<XftRadialBlurTexAdd> ();
            }
            //assign the shader here.
            m_radialBlurComp.Init (m_owner.RadialBlurTexAddShader);
            m_radialBlurComp.enabled = flag;
        }

        public override void Initialize ()
        {
            FindCamera ();
            ToggleCameraComponent (false);
            m_elapsedTime = 0f;
            m_supported = m_radialBlurComp.CheckSupport ();
            if (!m_supported)
                Debug.LogWarning ("can't support Image Effect: Radial Blur Mask on this device!");
        }

        public override void Reset ()
        {
            m_radialBlurComp.enabled = false;
            m_elapsedTime = 0f;
            m_editorFlag = false;
        }
     
        public override void EditorDone ()
        {
            m_editorFlag = true;
        }

        public override void Update (float deltaTime)
        {
         
            if (!m_supported) {
                m_owner.enabled = false;
                return;
            }
         
            if (m_editorFlag)
                return;
            m_elapsedTime += deltaTime;

            if (m_elapsedTime > m_owner.EndTime) {
                m_radialBlurComp.enabled = false;
                return;
            }

            if (m_elapsedTime > m_owner.StartTime) {
                m_radialBlurComp.enabled = true;
                m_radialBlurComp.SampleDist = m_owner.RBMaskSampleDist;
                
                float strength = 1f;
                
                if (m_owner.RBMaskStrengthType == MAGTYPE.Fixed)
                 strength = m_owner.RBMaskSampleStrength;
             else
                 strength = m_owner.RBMaskSampleStrengthCurve.Evaluate(m_elapsedTime - m_owner.StartTime);
                
                m_radialBlurComp.SampleStrength = strength;
                m_radialBlurComp.Mask = m_owner.RadialBlurMask;
            }
        }
    }

}