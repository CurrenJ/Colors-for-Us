using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class SpriteEffect
    {
        private float startTime;
        private float duration;
        private float startVal;
        public float endVal;
        private bool backToStart;
        private String id;
        private List<EffectSwitch> consequence;
        private bool enabled;

        public SpriteEffect(String id, float startTime, float duration, float startVal, float endVal, bool backToStart, List<EffectSwitch> consequence) {
            this.id = id;
            this.startTime = startTime;
            this.duration = duration;
            this.endVal = endVal;
            this.startVal = startVal;
            this.backToStart = backToStart;
            this.consequence = consequence;
            this.enabled = false;
        }

        public SpriteEffect(String id, float startTime, float duration, float startVal, float endVal, bool backToStart, bool enabled, List<EffectSwitch> consequence)
        {
            this.id = id;
            this.startTime = startTime;
            this.duration = duration;
            this.endVal = endVal;
            this.startVal = startVal;
            this.backToStart = backToStart;
            this.consequence = consequence;
            this.enabled = enabled;
        }

        public double update(float currentTime) {
            return easingFunction(currentTime);
        }

        public double easingFunction(float currentTime) {
            float eased = (currentTime - startTime) / duration;
            if (eased >= 1)
                return backToStart ? startVal : endVal;
            else if (eased < 0)
                return startVal;
            else if (!backToStart)
            {
                return -(endVal-startVal) * Math.Pow(eased - 1F, 2) + endVal;
            }
            else {
                return -4 * (endVal - startVal) * Math.Pow(eased - 0.5F, 2) + endVal;
            }
        }

        public bool isFinished(float currentTime) {
            return currentTime - startTime > duration;
        }

        public String getID() {
            return id;
        }

        public List<EffectSwitch> getConsequence() {
            return consequence;
        }

        public bool isEnabled() {
            return enabled;
        }

        public String printEffect() {
            return (id + ", " + startTime + " > "  + duration + ", " + startVal + " > " + endVal + " | " + enabled);
        }
    }
}
