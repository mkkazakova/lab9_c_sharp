using System;

class lab9_1
{
    static readonly Mutex mutex = new Mutex();

    static async Task Main()
    {
        List<string> tickers = new List<string>(); // Список названий акций

        // Чтение из файла ticker.txt
        using (StreamReader reader = new StreamReader("ticker.txt"))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                tickers.Add(line);
            }
        }

        // Делаем запросы для каждого tickerа
        using (HttpClient client = new HttpClient())
        {
            List<Task> tasks = new List<Task>();

            foreach (string ticker in tickers)
            {
                tasks.Add(GetDataForTicker(client, ticker));
            }

            await Task.WhenAll(tasks);
        }
    }

    static async Task GetDataForTicker(HttpClient client, string ticker)
    {
        try
        {
            // Устанавливаем даты
            DateTime startDate = DateTime.Now.AddYears(-1);
            DateTime endDate = DateTime.Now;

            long startUnixTime = ((DateTimeOffset)startDate).ToUnixTimeSeconds();
            long endUnixTime = ((DateTimeOffset)endDate).ToUnixTimeSeconds();

            string url = $"https://query1.finance.yahoo.com/v7/finance/download/{ticker}?period1={startUnixTime}&period2={endUnixTime}&interval=1d&events=history&includeAdjustedClose=true";

            HttpResponseMessage response = await client.GetAsync(url); // отправляем запрос
            response.EnsureSuccessStatusCode(); // гарантирует, что HTTP-запрос завершился успешно 
            string csvData = await response.Content.ReadAsStringAsync(); // Сериализация содержимого HTTP в строку асинхронно 

            string[] lines = csvData.Split('\n'); // Разбивает строку на подстроки на основе указанных символов-разделителей (1 строка - 1 день)
            double totalAveragePrice = 0.0;
            int totalRowCount = 0;

            // считаем среднее по дням и количество дней
            for (int i = 1; i < lines.Length - 1; i++) // lines[0] - "Date,Open,High,Low,Close,Adj Close,Volume", её не читаем 
            {
                try
                {
                    string[] values = lines[i].Split(',');

                    double high = Convert.ToDouble(values[2], new System.Globalization.CultureInfo("en-US"));
                    double low = Convert.ToDouble(values[3], new System.Globalization.CultureInfo("en-US"));

                    double averagePrice = (high + low) / 2; // за день

                    totalAveragePrice += averagePrice;
                    totalRowCount++; // чтобы отслеживать общее количество дней
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обработке строки для {ticker}: {ex.Message}");
                }

            }

            // вывод среднего за год
            if (totalRowCount > 0)
            {
                double totalAverage = totalAveragePrice / totalRowCount;
                string result = $"{ticker}:{totalAverage}";

                mutex.WaitOne(); // приостанавливает выполнение потока до тех пор, пока не будет получен mutex
                try
                {
                    File.AppendAllText("results.txt", result + Environment.NewLine); // записываем результат в файл results.txt
                }
                finally
                {
                    mutex.ReleaseMutex(); // освобождение mutex
                }

                Console.WriteLine($"Средняя цена акции для {ticker} за год: {totalAverage}");
            }
            else
            {
                Console.WriteLine($"Для {ticker} нет данных за год.");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обработке {ticker}: {ex.Message}");
        }
    }
}
