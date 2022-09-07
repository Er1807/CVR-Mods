using ABI_RC.Core.Savior;
using EnableFingerTrackingByDefault;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: MelonInfo(typeof(EnableFingerTrackingByDefaultMod), "EnableFingerTrackingByDefault", "1.0.0", "Eric van Fandenfart")]
[assembly: MelonGame]

namespace EnableFingerTrackingByDefault
{
    internal class EnableFingerTrackingByDefaultMod : MelonMod
    {

        public override void OnApplicationStart()
        {

            MelonCoroutines.Start(WaitForInputManager());
        }
        private IEnumerator WaitForInputManager()
        {
            while (CVRInputManager.Instance == null)
            {
                yield return null;
            }
            CVRInputManager.Instance.individualFingerTracking = true;
        }
    }
}
