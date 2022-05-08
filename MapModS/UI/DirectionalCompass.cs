using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapModS.UI
{
    internal class DirectionalCompass : MonoBehaviour
    {
        private GameObject entity;

        private GameObject compassInternal;
        private SpriteRenderer sr;

        private Func<bool> Condition;

        private float radius;

        private bool lerp;
        private float lerpDuration;

        private float lerpStartTime;
        private GameObject currentTarget;
        private Vector3 currentDir;
        private float currentAngle;

        public List<GameObject> trackedObjects;

        public static GameObject Create(string name, GameObject entity, Sprite sprite, Color color, float radius, float scale, Func<bool> condition, bool lerp, float lerpDuration)
        {
            // This object is a container for the script. Can be set active/inactive externally to control script
            GameObject compass = new(name);
            DontDestroyOnLoad(compass);

            compass.transform.parent = entity.transform;

            DirectionalCompass dc = compass.AddComponent<DirectionalCompass>();

            // This object is the actual compass sprite. Set active/inactive by the script itself
            dc.compassInternal = new(name + " Internal", typeof(SpriteRenderer));
            DontDestroyOnLoad(dc.compassInternal);
            dc.compassInternal.layer = 18;

            dc.sr = dc.compassInternal.GetComponent<SpriteRenderer>();
            dc.sr.sprite = sprite;
            dc.sr.color = color;

            dc.compassInternal.transform.parent = compass.transform;
            dc.compassInternal.transform.localScale = Vector3.one * scale;

            dc.entity = entity;
            dc.radius = radius;
            dc.Condition = condition;
            dc.lerp = lerp;
            dc.lerpDuration = lerpDuration;

            return compass;
        }

        public void Destroy()
        {
            Destroy(compassInternal);
            Destroy(gameObject);
        }

        public void Update()
        {
            if (entity != null && Condition() && TryGetClosestObject(out GameObject o))
            {
                Vector3 dir = (o.transform.position - entity.transform.position);

                dir.Scale(Vector3.one * 0.5f);

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

                // Clamp to radius
                dir = Vector3.ClampMagnitude(dir, radius);

                // Do lerp stuff
                if (lerp)
                {
                    if (currentTarget == null || currentTarget != o)
                    {
                        currentTarget = o;
                        lerpStartTime = Time.time;
                    }
                    
                    if (Time.time - lerpStartTime < lerpDuration)
                    {
                        dir = Vector3.Lerp(currentDir, dir, (Time.time - lerpStartTime) / lerpDuration);
                        angle = Mathf.LerpAngle(currentAngle, angle, (Time.time - lerpStartTime) / lerpDuration);
                    }
                }

                currentDir = dir;
                currentAngle = angle;

                // Undo reflection when the entity is facing right
                dir.x *= entity.transform.localScale.x;

                transform.localPosition = dir;
                transform.eulerAngles = new(0, 0, angle);
                transform.localScale = dir.magnitude / radius * Vector2.one;
                sr.color = dir.magnitude / radius * Color.white;

                compassInternal.SetActive(true);
            }
            else
            {
                compassInternal.SetActive(false);
            }
        }

        private bool TryGetClosestObject(out GameObject o)
        {
            if (trackedObjects == null || !trackedObjects.Any() || entity == null)
            {
                o = null;

                return false;
            }

            o =  trackedObjects.Aggregate((i1, i2) => SqrDistanceFromEntity(i1) < SqrDistanceFromEntity(i2) ? i1 : i2);
            
            return o != null;
        }

        private float SqrDistanceFromEntity(GameObject o)
        {
            if (o == null || entity == null) return 9999f;

            return (o.transform.position - entity.transform.position).sqrMagnitude;
        }
    }
}
