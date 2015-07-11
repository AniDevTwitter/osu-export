using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
                var retVal = Path.Combine(this.path, "bg.jpg");
                return File.Exists(retVal) ? retVal : null;
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
                File.Copy(Path.Combine(this.path, beatmap.FileName.TrimStart(' ')), copiedFilePath, true);
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
