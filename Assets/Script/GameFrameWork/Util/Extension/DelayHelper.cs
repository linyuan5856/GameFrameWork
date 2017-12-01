using System;
using System.Collections;
using UnityEngine;

namespace GFW
{
    public sealed class DelayHelper : MonoBehaviour
    {
        private static DelayHelper _delayHelper;
        static DelayHelper GetSelf()
        {
            if (_delayHelper==null)
            {
               _delayHelper= GameUtil.AddComponentToP<DelayHelper>(MainGame.Instance.gameObject);
            }
            return _delayHelper;
        }

        public static void DelayDo<T>(float seconds, Action<T> callback, T arg)
        {
            if (callback != null)
            {
                GetSelf().DelayDo2(seconds, callback, arg);
            }
        }

        public static void DelayDo(float seconds, Action callback)
        {
            if (callback != null)
            {
                GetSelf().DelayDo2(seconds, callback);
            }
        }

        public static void AfterFrameDo<T>(int x_frame, Action<T> callback, T arg)
        {
            if (callback != null)
            {
                GetSelf().AfterFrameDo2(x_frame, callback, arg);
            }
        }

        public static void AfterFrameDo(int x_frame, Action callback)
        {
            if (callback != null)
            {
                GetSelf().AfterFrameDo2(x_frame, callback);
            }
        }

        public static void NextFrameDo<T>(Action<T> callback, T arg)
        {
            if (callback != null)
            {
                GetSelf().NextFrameDo2(callback, arg);
            }
        }

        public static void NextFrameDo(Action callback)
        {
            if (callback != null)
            {
                GetSelf().NextFrameDo2(callback);
            }
        }

        //Delay-T
        private void DelayDo2<T>(float seconds, Action<T> callback, T arg = default(T))
        {
            if (callback != null)
            {
                Action cb = () =>
                {
                    callback(arg);
                };
                this.StartCoroutine(this.delay_do(seconds, cb));
            }
        }

        //Delay-Void
        private void DelayDo2(float seconds, Action callback)
        {
            if (seconds <= 0)
            {
                if (callback != null)
                    callback();
            }
            else
                this.StartCoroutine(this.delay_do(seconds, callback));
        }

        private IEnumerator delay_do(float seconds, Action callback)
        {
            if (seconds > 0)
                yield return new WaitForSeconds(seconds);

            if (callback != null)
                callback();
        }


        //Delay NextFrame
        private void AfterFrameDo2<T>(int x_frame, Action<T> callback, T arg = default(T))
        {
            if (callback != null)
            {
                Action cb = () =>
                {
                    callback(arg);
                };
                this.StartCoroutine(this.after_frame_do(x_frame, cb));
            }
        }

        private void AfterFrameDo2(int x_frame, Action callback)
        {
            if (callback != null)
                this.StartCoroutine(this.after_frame_do(x_frame < 0 ? 0 : x_frame, callback));
        }

        private IEnumerator after_frame_do(int x_frame, Action callback)
        {
            for (int i = 0; i < x_frame; i++)
                yield return new WaitForEndOfFrame();

            if (callback != null)
                callback();
        }

        //NextFrameDo
        private void NextFrameDo2<T>(Action<T> callback, T arg = default(T))
        {
            this.AfterFrameDo2(1, callback, arg);
        }

        public void NextFrameDo2(Action callback)
        {
            this.AfterFrameDo2(1, callback);
        }

    }

}

