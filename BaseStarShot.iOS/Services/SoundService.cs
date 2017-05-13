using System;
using System.Threading;
using System.Collections.Generic;
using AVFoundation;
using Foundation;
using AudioToolbox;

namespace BaseStarShot
{
	public class SoundService : BaseStarShot.Services.ISoundService
	{
		static readonly ReaderWriterLockSlim taskLocker = new ReaderWriterLockSlim();
		static volatile int s_ringtoneId = 0;
		List<AVAudioPlayer> players = new List<AVAudioPlayer>();
		public SystemSound systemSound;
		public bool isStop;
		public int Play(SoundType type)
		{
			taskLocker.EnterWriteLock();
			try
			{
//				AVAudioPlayer audioPlayer = new AVAudioPlayer (new NSUrl (NSBundle.FromIdentifier("com.apple.UIKit").PathForResource("Tock","aiff")),
//					"aiff", null);
//				if(audioPlayer.
//				audioPlayer.Play ();
//				players.Add(audioPlayer);

				isStop = false;
				var url = NSUrl.FromFilename("/System/Library/Audio/UISounds/alarm.caf");
				systemSound = new SystemSound (url);
				systemSound.AddSystemSoundCompletion(() => {
					if(!isStop){
						systemSound.PlaySystemSound();
					}
				}, CoreFoundation.CFRunLoop.Main);
				systemSound.PlaySystemSound();

				//systemSound.PlayAlertSound();
//				return ringtoneId;
			}
            catch (Exception ex)
            {
                Logger.WriteError("SoundService", ex);
            }
			finally
			{
				taskLocker.ExitWriteLock();
			}
			return 1;
		}

		public void Stop(int id)
		{
			taskLocker.EnterWriteLock();
			try
			{
				if(systemSound != null){
					isStop = true;
					systemSound.RemoveSystemSoundCompletion();
				}
			}
			finally
			{
				taskLocker.ExitWriteLock();
			}
		}
	}
}
