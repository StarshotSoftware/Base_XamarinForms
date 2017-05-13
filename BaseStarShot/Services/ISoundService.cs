using System;
namespace BaseStarShot.Services
{
    public interface ISoundService
    {
        /// <summary>
        /// Plays a specific sound type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Id for the sound.</returns>
        int Play(SoundType type);

        /// <summary>
        /// Stops a sound from playing.
        /// </summary>
        /// <param name="id">Id for the sound.</param>
        void Stop(int id);
    }
}
