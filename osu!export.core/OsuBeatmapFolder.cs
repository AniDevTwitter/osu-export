using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace osu_export.core
{
    public class OsuBeatmapFolder
    {
        public const string Subpath = @"Songs";
        private readonly string path;
        private readonly Lazy<OsuBeatmap> beatmap;

        public OsuBeatmapFolder(string path)
        {
            this.path = path;
            this.beatmap = new Lazy<OsuBeatmap>(() => new OsuBeatmap(this.path));
        }

        public bool ValidForExport
        {
            get
            {
                return this.Beatmap.ValidForExport;
            }
        }

        /// <summary>
        /// Return the artwork path, if there's one, otherwise null
        /// </summary>
        public string ArtworkPath
        {
            get
            {
                var imageFiles = Directory.EnumerateFiles(this.path, "*.jpg").Concat(Directory.EnumerateFiles(this.path, ".png")).ToList();
                // here a method to determine the artworkPath to use would be great
                return imageFiles.Any() ? imageFiles.First() : null;
            }
        }

        public OsuBeatmap Beatmap
        {
            get
            {
                return this.beatmap.Value;
            }
        }

        public void ExportSong(Executer fileAccess, string folderPath, string format = "%ARTIST% - %TITLE%")
        {
#if DEBUG
#else
            try
            {
#endif
            var beatmap = this.Beatmap;
            var copiedFilePath = Path.Combine(folderPath, beatmap.FormattedOutputFilename(format).AsValidPath());
            fileAccess.AddAction(() => File.Copy(Path.Combine(this.path, beatmap.FileName), copiedFilePath, true), "Copying file : " + copiedFilePath);
            using (var copiedFile = fileAccess.AddFunc(() => TagLib.File.Create(copiedFilePath), "Creating taggable file from : " + copiedFilePath))
            {
                copiedFile.RemoveTags(TagLib.TagTypes.Id3v2);
                copiedFile.Tag.Title = beatmap.Title;
                copiedFile.Tag.Performers = new string[] { beatmap.Artist };
                var artworkPath = this.ArtworkPath;
                if (artworkPath != null)
                {
                    copiedFile.Tag.Pictures = new[] { new TagLib.Picture(artworkPath) };
                }
                fileAccess.AddAction(() => copiedFile.Save(), "Saving tagged file : " + copiedFilePath);
            }

#if DEBUG
#else
            }
            catch (Exception ex)
            {
                ExportLogger.GetInstance().LogError(ex, "put info");
            }
#endif
        }
    }
}
