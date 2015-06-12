using UnityEngine;

using System;
using System.Collections;

namespace Game5835
{
    public class SoundManager : MonoBehaviour
    {

        public AudioSource audiosource;

        public AudioClip[] audioclips;

        float m_volume;

        public void Play(int idx_music, bool loop)
        {
            //LogServer.Instance.print("idx_music " + idx_music);

            try
            {
                if (audiosource != null)
                    audiosource.Stop();

                audiosource.volume = m_volume;

                this.audiosource.loop = loop;

                if (this.audiosource.loop == false)
                {
                    audiosource.clip = audioclips[idx_music];
                    audiosource.Play();
                }
                else
                {
                    audiosource.clip = audioclips[idx_music];
                    audiosource.Play();
                }
            }
            catch (Exception EX)
            {
                //Global.print("SoundManager Play Exception " + EX);
            }
        }

        public void SetVolume(float value)
        {
            m_volume = value;
        }

        public void AddPlay(int idx_sound)
        {
            StartCoroutine(Func_PlayNext(idx_sound));
        }

        IEnumerator Func_PlayNext(int idx_sound)
        {

            bool done = false;
            while (!done)
            {
                //print("audiosource.isPlaying " + audiosource.isPlaying);
                if (!audiosource.isPlaying)
                {
                    Play(idx_sound, false);
                    done = true;
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}