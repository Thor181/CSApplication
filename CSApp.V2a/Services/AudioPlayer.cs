using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace CSApp.V2a.Services
{
#pragma warning disable CA1416 // Validate platform compatibility
    public class AudioPlayer
    {
        private readonly Dictionary<string, SoundPlayer> _players = [];

        public void Load(string path, string name)
        {
            using var fs = new FileStream(path, FileMode.Open);
            _players[name] = new SoundPlayer(fs);
        }

        public void Load(Stream stream, string name)
        {
            _players[name] = new SoundPlayer(stream);
        }

        public void Play(string name)
        {
            _players[name].Play();
        }
    }
#pragma warning restore CA1416 // Validate platform compatibility
}
