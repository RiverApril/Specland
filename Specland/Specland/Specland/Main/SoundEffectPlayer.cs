using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Specland {
    class SoundEffectPlayer {

        public static SoundEffect SoundPop;
        public static SoundEffect SoundTink;

        public static List<SoundEffectInstance> soundEffects = new List<SoundEffectInstance>();

        public static void playSound(SoundEffect soundEffect) {
            SoundEffectInstance instance = soundEffect.CreateInstance();
            instance.Volume = Game.volume;
            instance.Play();
            soundEffects.Add(instance);
        }

        public static void playSoundWithRandomPitch(SoundEffect soundEffect) {
            SoundEffectInstance instance = soundEffect.CreateInstance();
            instance.Volume = Game.volume;
            instance.Pitch = ((Game.rand.Next(10) / 10f));
            instance.Play();
            soundEffects.Add(instance);
        }

        public static void update() {
            foreach(SoundEffectInstance i in soundEffects){
                if(i.State == SoundState.Stopped){
                    soundEffects.Remove(i);
                    break;
                }
            }
        }

        internal static void loadSounds(ContentManager content) {
            SoundEffectPlayer.SoundPop = content.Load<SoundEffect>("Sounds\\pop");
            SoundEffectPlayer.SoundTink = content.Load<SoundEffect>("Sounds\\tink");
        }
    }
}
