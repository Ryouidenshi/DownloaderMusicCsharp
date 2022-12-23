using MediaToolkit.Model;
using MediaToolkit;
using VideoLibrary;
using System.Runtime.InteropServices;

namespace DownloaderMusic
{
    public static class Programm
    {
        public static void Main()
        {
            Console.WriteLine("Введите ссылку");
            string path = Console.ReadLine();
            if(path == null)
            {
                path = "https://www.youtube.com/watch?v=eF9ytz99Slw";
            }
            Download(path);
        }

        public static void Download(string path)
        {
            var source = GetDownloadsPath();
            var youtube = YouTube.Default;
            var vid = youtube.GetVideo(path);

            File.WriteAllBytes(source + vid.FullName, vid.GetBytes());

            var inputFile = new MediaFile { Filename = source + vid.FullName };
            var outputFile = new MediaFile { Filename = $"{source + vid.FullName}.mp3" };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                engine.Convert(inputFile, outputFile);
            }
        }

        public static string GetDownloadsPath()
        {
            if (Environment.OSVersion.Version.Major < 6) throw new NotSupportedException();
            IntPtr pathPtr = IntPtr.Zero;
            try
            {
                SHGetKnownFolderPath(ref FolderDownloads, 0, IntPtr.Zero, out pathPtr);
                return Marshal.PtrToStringUni(pathPtr);
            }
            finally
            {
                Marshal.FreeCoTaskMem(pathPtr);
            }
        }

        private static Guid FolderDownloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetKnownFolderPath(ref Guid id, int flags, IntPtr token, out IntPtr path);
    }
}

