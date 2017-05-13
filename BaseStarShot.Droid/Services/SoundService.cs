using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseStarShot;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Media;
using System.Threading;

namespace BaseStarShot.Services
{
    public class SoundService : BaseStarShot.Services.ISoundService
    {
        static readonly ReaderWriterLockSlim taskLocker = new ReaderWriterLockSlim();
        static volatile int s_ringtoneId = 0;
        static Dictionary<int, Ringtone> ringtones = new Dictionary<int, Ringtone>();

        readonly Context context;

        public SoundService(Context context)
        {
            this.context = context;
        }

        public int Play(SoundType type)
        {
            taskLocker.EnterWriteLock();
            try
            {
                RingtoneType ringtoneType;
                switch(type)
                {
                    case SoundType.Alarm:
                        ringtoneType = RingtoneType.Alarm;
                        break;
                    case SoundType.Ringtone:
                        ringtoneType = RingtoneType.Ringtone;
                        break;
                    default:
                        ringtoneType = RingtoneType.Notification;
                        break;
                }
                var ringtoneUri = RingtoneManager.GetDefaultUri(ringtoneType);
                var ringtone = RingtoneManager.GetRingtone(context, ringtoneUri);
                var ringtoneId = ++s_ringtoneId;
                ringtones.Add(ringtoneId, ringtone);
                ringtone.Play();
                return ringtoneId;
            }
            finally
            {
                taskLocker.ExitWriteLock();
            }
        }

        public void Stop(int id)
        {
            taskLocker.EnterWriteLock();
            try
            {
                if (ringtones.ContainsKey(id))
                {
                    ringtones[id].Stop();
                    ringtones.Remove(id);
                }
            }
            finally
            {
                taskLocker.ExitWriteLock();
            }
        }
    }
}