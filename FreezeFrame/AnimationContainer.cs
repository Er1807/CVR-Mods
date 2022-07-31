using UnityEngine;

namespace FreezeFrame
{


    
        public class AnimationContainer
        {
            public string Path;
            public string Property;
            public AnimationCurve Curve = new AnimationCurve();

            public AnimationContainer(string path, string property)
            {
                Path = path;
                Property = property;
            }


            public void Record(float currentTime, float value)
            {
                Curve.AddKey(currentTime, value);
            }
        }
}
