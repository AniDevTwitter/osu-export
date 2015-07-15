using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace osu_export.core
{
    public class OsuBeatmap
    {
        private const string ArtistKey = "Artist";
        private const string AudioFilenameKey = "AudioFilename";
        private const string TitleKey = "Title";
        
        private readonly IReadOnlyDictionary<string, string> metadatas;
        private readonly bool audioFileExits;

        private readonly HashSet<string> ValidMetadas = new HashSet<string>()
        {
            OsuBeatmap.TitleKey, OsuBeatmap.AudioFilenameKey, OsuBeatmap.ArtistKey
        };

        public bool ValidForExport
        {
            get
            {
                return this.audioFileExits
                    && this.metadatas.ContainsKey(ArtistKey)
                    && this.metadatas.ContainsKey(AudioFilenameKey)
                    && this.metadatas.ContainsKey(TitleKey);
            }
        }

        public OsuBeatmap(string folderPath)
        {
            var beatmapfile = Directory.GetFiles(folderPath, "*.osu").FirstOrDefault();
            if(!string.IsNullOrWhiteSpace(beatmapfile))
            {
                var potentiallyOnlyMetadatas = File.ReadAllLines(beatmapfile).TakeWhile(x => !x.Contains("[HitObjects]"));
                this.metadatas = potentiallyOnlyMetadatas.Select(x => this.MetadataOrNull(x))
                    .Where(x => x != null).ToDictionary(x => x.Item1, x => x.Item2);
                this.audioFileExits = File.Exists(Path.Combine(folderPath, this.FileName));
            }
            else
            {
                this.audioFileExits = false;  // lazy to create another bool var for this
            }            
        }

        public string FormattedOutputFilename(string format = "%ARTIST% - %TITLE%")
        {
            //makes this generic, matching every tag like %ARTIST% by the equivalent in metadatas
            return format.Replace("%ARTIST%", this.Artist).Replace("%TITLE%", this.Title + this.FileExtension);
        }

        public string Artist
        {
            get
            {
                return this.metadatas[ArtistKey];
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

        public string FileName
        {
            get
            {
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
            return Tuple.Create(key, string.Join(":", splitted.Skip(1)).TrimStart(' '));
        }
    }
}