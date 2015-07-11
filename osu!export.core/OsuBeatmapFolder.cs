using System;
using System.IO;
using System.Linq;

namespace osu_export.core
{
    public class OsuBeatmapFolder
    {
        public const string Subpath = @"Songs";
        private readonly string path;

        public OsuBeatmapFolder(string path)
        {
            this.path = path;
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
                return new OsuBeatmap(this.path);
            }
        }

        public void ExportSong(string folderPath)
        {
            try
            {
                var beatmap = this.Beatmap;
                var copiedFilePath = Path.Combine(folderPath, beatmap.Title + beatmap.FileExtension);
                File.Copy(Path.Combine(this.path, beatmap.FileName), copiedFilePath, true);
                using (var copiedFile = TagLib.File.Create(copiedFilePath))
                {
                    copiedFile.RemoveTags(TagLib.TagTypes.Id3v2);
                    copiedFile.Tag.Title = beatmap.Title;
                    copiedFile.Tag.Performers = new string[] { beatmap.Artist };
                    var artworkPath = this.ArtworkPath;
                    if (artworkPath != null)
                    {
                        copiedFile.Tag.Pictures = new[] { new TagLib.Picture(artworkPath) };
                    }
                    copiedFile.Save();
                }
            }
            catch (Exception ex)
            {
                ExportLogger.GetInstance().LogError(ex, "put info");
            }
        }
    }
}