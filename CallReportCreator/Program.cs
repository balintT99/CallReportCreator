using CallReportCreator.Classes;
using System.Text;

namespace CallReportCreator
{
    public class Program
    {
        public static List<Input> input = new List<Input>();
        public static List<Country> country = new List<Country>();
        public static List<Area> area = new List<Area>();
        public static List<Output> output = new List<Output>();

        static void Main(string[] args)
        {
            // Collect all the file paths.
            string basePath;
            if (Directory.GetCurrentDirectory().Contains("Release"))
            {
                basePath = Directory.GetCurrentDirectory();
            }
            else
            {
                basePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            }

            FilePaths filePaths = new FilePaths()
            {
                InputPath = Path.Combine(basePath, "Source", "input.txt"),
                CountryPath = Path.Combine(basePath, "Source", "country.txt"),
                AreaPath = Path.Combine(basePath, "Source", "area.txt"),
                OutputPath = Path.Combine(basePath, "Output", "output.txt"),
                ErrorPath = Path.Combine(basePath, "ErrorLogs", "error.txt")
            };

            LoadData(filePaths);
            BuildOutput(filePaths);
        }

        /// <summary>
        /// Import all data from the TXT files into Lists.
        /// </summary> 
        public static void LoadData(FilePaths filePaths)
        {
            try
            {
                // Read the lines from the input.txt and import them into List<Input>
                foreach (string line in File.ReadAllLines(filePaths.InputPath, Encoding.Latin1))
                {
                    string[] lineArray = line.Split('\t');
                    input.Add(new Input
                    {
                        CallDate = lineArray[0],
                        CallTime = lineArray[1],
                        CallerNumber = lineArray[2],
                        CalledPartyNumber = lineArray[3],
                        CallDuration = Convert.ToInt32(lineArray[4])
                    });
                }

                // Read the lines from the country.txt and import them into List<Country>
                foreach (string line in File.ReadAllLines(filePaths.CountryPath, Encoding.Latin1))
                {
                    string[] lineArray = line.Split('\t');
                    country.Add(new Country
                    {
                        Code = lineArray[0],
                        Name = lineArray[1]
                    });
                }

                // Read the lines from the area.txt and import them into List<Area>
                foreach (string line in File.ReadAllLines(filePaths.AreaPath, Encoding.Latin1))
                {
                    string[] lineArray = line.Split('\t');
                    area.Add(new Area
                    {
                        CountryCode = lineArray[0],
                        Code = lineArray[1],
                        Name = lineArray[2]
                    });
                }
            }
            catch (Exception ex)
            {
                LogError(filePaths, ex.Message);
            }
        }

        /// <summary>
        /// Build output.txt based on the incoming data.
        /// </summary> 
        public static void BuildOutput(FilePaths filePaths)
        {
            foreach (var dataRow in area)
            {
                try
                {
                    // Get areaCode, countryCode and areaName from the imported area list.
                    string areaCode = dataRow.Code;
                    string countryCode = dataRow.CountryCode;
                    string areaName = dataRow.Name;

                    // Get countryName from the imported country list based on the countryCode.
                    var requiredCountry = country.FirstOrDefault(x => x.Code == countryCode);
                    if (requiredCountry == null)
                    {
                        continue;
                    }
                    string countryName = requiredCountry.Name;

                    // Get callDuration based on the countryCode and areaCode from the imported area list.
                    var allRequiredInput = input.FindAll(x => x.CalledPartyNumber.Contains($"+{countryCode}{areaCode}"));
                    int durationSum = allRequiredInput.Sum(x => x.CallDuration);

                    // Add new element into the output list from the collected.
                    output.Add(new Output
                    {
                        AreaCode = areaCode,
                        AreaName = areaName,
                        CountyCode = countryCode,
                        CountyName = countryName,
                        DurationSum = durationSum
                    });

                    // Check if file exist and write data to output.txt
                    if (!File.Exists(filePaths.OutputPath))
                    {
                        using (StreamWriter sw = File.CreateText(filePaths.OutputPath));
                    }

                    using (StreamWriter sw = File.AppendText(filePaths.OutputPath))
                    {
                        sw.WriteLine($"{countryName}\t{areaName}\t{countryCode}\t{areaCode}\t{durationSum}");
                    }
                }
                catch (Exception ex)
                {
                    LogError(filePaths ,ex.Message);
                }
            }
        }

        /// <summary>
        /// Logging error into a file.
        /// </summary> 
        public static void LogError(FilePaths filePaths, string message)
        {
            using (StreamWriter sw  = File.AppendText(filePaths.ErrorPath))
            {
                string dateTime = DateTime.Now.ToString();
                sw.WriteLine($"{dateTime} {message}\n\n");
            }
        }
    }
}