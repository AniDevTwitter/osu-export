using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace osu_export.core
{
    public class OsuBeatmap
    {

        private readonly IReadOnlyDictionary<string, string> metadatas;
        private const string TitleKey = "Title";
        private const string AudioFilenameKey = "AudioFilename";
        private const string ArtistKey = "Artist";

        private readonly HashSet<string> ValidMetadas = new HashSet<string>()
        {
            OsuBeatmap.TitleKey, OsuBeatmap.AudioFilenameKey, OsuBeatmap.ArtistKey
        };

        public OsuBeatmap(string path)
        {
            string beatmapfile = Directory.GetFiles(path, "*.osu")[0];
            var potentiallyOnlyMetadatas = File.ReadAllLines(beatmapfile).TakeWhile(x => !x.Contains("[HitObjects]"));
            this.metadatas = potentiallyOnlyMetadatas.Select(x => this.MetadataOrNull(x))
                .Where(x => x != null).ToDictionary(x => x.Item1, x => x.Item2);
        }

        public string FileName
        {
            get
            {
                // We don't trim or anything, the goal of this class is to provide data "as is"
                return this.metadatas[OsuBeatmap.AudioFilenameKey];
            }
        }

        public string Title
        {
            get
            {
                return this.metadatas[OsuBeatmap.TitleKey];
            }
        }

        public string FileExtension
        {
            get
            {
                var fileName = this.FileName;
                // This should include "." in the extension
                return fileName.Substring(fileName.LastIndexOf('.')); 
            }
        }

        public string Artist
        {
            get
            {
                return this.metadatas[ArtistKey];
            }
        }

        private Tuple<string, string> MetadataOrNull(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return null;
            }
            var splitted = line.Split(':');
            if (splitted.Length < 2)
            {
                return null;
            }
            var key = splitted.First();
            if (!ValidMetadas.Contains(key))
            {
                return null;
            }
            // The join is here to place the removed ":", potentially avoid problems
            return Tuple.Create(key, string.Join(":", splitted.Skip(1)));
        }
    }
}
