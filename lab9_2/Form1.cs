using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace lab9_2
{
    public partial class Form1 : Form
    {
        private string apiKey = "911441194c8b808b43bc240a6513c630"; // API key
        private string apiUrl = "https://api.openweathermap.org/data/2.5/weather";
        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadCityDataAsync();
        }

        private async Task LoadCityDataAsync()
        {
            try
            {
                var cities = await ReadCitiesFromFileAsync("city.txt");
                listBox1.Items.AddRange(cities.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading city data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a city", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedCity = listBox1.SelectedItem.ToString();
            var coordinates = selectedCity.Split(',');

            if (coordinates.Length != 3)
            {
                MessageBox.Show("Invalid city data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double latitude = Convert.ToDouble(coordinates[1], new System.Globalization.CultureInfo("en-US"));
            double longitude = Convert.ToDouble(coordinates[2], new System.Globalization.CultureInfo("en-US"));


            var weather = await GetWeatherDataAsync(latitude, longitude);
            if (weather != null)
            {
                DisplayWeatherInfo(weather);
            }
            else
            {
                label1.Text = "Failed to fetch weather data";
            }

            Refresh();
        }
        private async Task<List<string>> ReadCitiesFromFileAsync(string filePath)
        {
            try
            {
                List<string> cities = new List<string>();

                using (StreamReader reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();
                        string[] parts = line.Split('\t');

                        if (parts.Length == 2)
                        {
                            string cityName = parts[0].Trim();
                            string coordinates = parts[1].Trim();

                            if (IsValidCoordinates(coordinates))
                            {
                                cities.Add($"{cityName},{coordinates}");
                            }
                            else
                            {
                                MessageBox.Show($"Invalid coordinates format in line: {line}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Invalid line format: {line}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                return cities;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading cities from file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<string>();
            }
        }

        private bool IsValidCoordinates(string coordinates)
        {
            if (Regex.IsMatch(coordinates, @"^\s*-?\d+(\.\d+)?,\s*-?\d+(\.\d+)?\s*$"))
            {
                return true;
            }
            return false;
        }

        private async Task<Weather> GetWeatherDataAsync(double latitude, double longitude)
        {
            using (HttpClient client = new HttpClient())
            {
                int maxAttempts = 10;
                int attempt = 0;

                while (attempt < maxAttempts)
                {
                    try
                    {
                        var response = await client.GetStringAsync($"{apiUrl}?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric");
                        var weatherInfo = JsonSerializer.Deserialize<WeatherInfo>(response);

                        if (weatherInfo != null && !string.IsNullOrEmpty(weatherInfo.sys.country))
                        {
                            string country = weatherInfo.sys.country;
                            string name = weatherInfo.name;
                            double temp = weatherInfo.main.temp;
                            string description = weatherInfo.weather[0].description;

                            return new Weather
                            {
                                Country = country,
                                Name = name,
                                Temp = temp,
                                Description = description
                            };
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        MessageBox.Show($"HTTP request error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        attempt++;
                    }
                    catch (JsonException ex)
                    {
                        MessageBox.Show($"JSON deserialization error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        attempt++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        attempt++;
                    }
                }

                return null;
            }
        }

        private void DisplayWeatherInfo(Weather weather)
        {
            if (weather != null)
            {
                string weatherInfoText = $"Country: {weather.Country}\nCity: {weather.Name}\nTemperature: {weather.Temp}°C\nDescription: {weather.Description}";
                label1.Text = weatherInfoText;
            }
            else
            {
                label1.Text = "Weather data not available.";
            }
        }

    }
    class Weather
    {
        public string Country { get; set; } // страна
        public string Name { get; set; } // город или название местности
        public double Temp { get; set; } // температура
        public string Description { get; set; } // описание погоды
    }

    class WeatherInfo
    {
        public MainInfo main { get; set; }
        public WeatherDescription[] weather { get; set; }
        public string name { get; set; }
        public SysInfo sys { get; set; }
    }

    class MainInfo
    {
        public double temp { get; set; }
    }

    class WeatherDescription
    {
        public string description { get; set; }
    }

    class SysInfo
    {
        public string country { get; set; }
    }
}