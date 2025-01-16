using System.Windows.Media;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Serialization;

namespace MinimalisticWPF.Extension
{
    public static class StringExtension
    {
        public static Brush ToBrush(this string source)
        {
            try
            {
                Brush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(source));
                return brush;
            }
            catch (FormatException)
            {
                return Brushes.Transparent;
            }
        }
        public static Color ToColor(this string source)
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(source);
                return color;
            }
            catch (FormatException)
            {
                return Color.FromArgb(0, 0, 0, 0);
            }
        }
        public static RGB ToRGB(this string source)
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(source);
                return new RGB(color.R, color.G, color.B, color.A);
            }
            catch (FormatException)
            {
                return new RGB(0, 0, 0, 0);
            }
        }

        public static List<string> CaptureBetween(this string source, string left, string right)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("TransitionApplied string cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right))
            {
                throw new ArgumentException("The length of the capture identifier cannot be zero.");
            }

            List<string> result = [];

            string escapedLeft = Regex.Escape(left);
            string escapedRight = Regex.Escape(right);

            string pattern = @$"{escapedLeft}(.*?){escapedRight}";
            Regex regex = new(pattern);

            MatchCollection matches = regex.Matches(source);

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    result.Add(match.Groups[1].Value);
                }
            }

            return result;
        }
        public static List<string> CaptureLike(this string source, params string[] features)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("Source string cannot be null or empty.");
            }

            if (features == null || features.Length < 2)
            {
                throw new ArgumentException("At least two features are required.");
            }

            List<string> result = [];

            string pattern = BuildPattern(features);
            Regex regex = new(pattern);

            MatchCollection matches = regex.Matches(source);

            foreach (Match match in matches)
            {
                result.Add(match.Value);
            }

            return result;
        }
        internal static string BuildPattern(string[] features)
        {
            List<string> patternParts = [];
            for (int i = 0; i < features.Length - 1; i++)
            {
                patternParts.Add(Regex.Escape(features[i]) + "(.*?)" + Regex.Escape(features[i + 1]));
            }
            return string.Join("|", patternParts);
        }

        public static string CreatFolder(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("Cannot create a folder with an empty name");
            }

            string result = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, source);

            if (!result.IsPathValid())
            {
                throw new ArgumentException("The generated folder is not formatted correctly");
            }

            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }

            return result;
        }
        public static string CreatFolder(this string source, params string[] nodes)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("Cannot create a folder with an empty name");
            }

            string result = string.Empty;
            for (int i = 0; i < nodes.Length; i++)
            {
                result = Path.Combine(i == 0 ? AppDomain.CurrentDomain.BaseDirectory : result, nodes[i]);
            }
            result = Path.Combine(result, source);

            if (!result.IsPathValid())
            {
                throw new ArgumentException("The generated folder is not formatted correctly");
            }

            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }

            return result;
        }

        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };
        public static string CreatXmlFile<T>(this string source, string folderPath, T targetObject)
        {
            if (!folderPath.IsPathValid() || !Directory.Exists(folderPath))
            {
                throw new ArgumentException($"Unavailable folder path {folderPath}");
            }

            string fileName = source + ".xml";
            string filePath = Path.Combine(folderPath, fileName);

            try
            {
                XmlSerializer serializer = new(typeof(T));
                using (StreamWriter writer = new(filePath))
                {
                    serializer.Serialize(writer, targetObject);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating XML file '{filePath}': {ex.Message}");
            }
        }
        public static string CreatJsonFile<T>(this string source, string folderPath, T targetObject)
        {
            if (!folderPath.IsPathValid() || !Directory.Exists(folderPath))
            {
                throw new ArgumentException("Unavailable folder path");
            }

            string fileName = source + ".json";
            string filePath = Path.Combine(folderPath, fileName);

            try
            {
                string jsonString = JsonSerializer.Serialize(targetObject, options);

                File.WriteAllText(filePath, jsonString);

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating JSON file '{filePath}': {ex.Message}");
            }
        }
        public static T? XmlParse<T>(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("Empty Content !");
            }

            try
            {
                XmlSerializer serializer = new(typeof(T));
                using StringReader reader = new(source);
                return (T?)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing XML file '{source}': {ex.Message}", ex);
            }
        }
        public static T? JsonParse<T>(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("Invalid file path");
            }

            try
            {
                return JsonSerializer.Deserialize<T>(source);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing JSON file '{source}': {ex.Message}");
            }
        }

        public static bool IsPathValid(this string source)
        {
            bool isAbsolutePath = Path.IsPathRooted(source);
            bool driveExists = true;
            if (isAbsolutePath)
            {
                string? drive = Path.GetPathRoot(source);
                if (drive == null) { return false; }
                driveExists = DriveInfo.GetDrives().Any(d => d.Name.Equals(drive, StringComparison.OrdinalIgnoreCase));
            }
            return isAbsolutePath && driveExists;
        }
    }
}
