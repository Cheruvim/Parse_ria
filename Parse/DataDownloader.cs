namespace Parse
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Загрузчик страниц.
    /// </summary>
    public class DataDownloader : IDisposable
    {
        /// <summary>
        /// Клиент для веба.
        /// </summary>
        private readonly HttpClient _client;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="DataDownloader" />.
        /// </summary>
        public DataDownloader()
        {
            _client = new HttpClient();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _client?.Dispose();
        }

        /// <summary>
        /// Создает задачу, которая получает контент по ссылке.
        /// </summary>
        /// <param name="link">Строка с ссылкой.</param>
        /// <returns>Экземпляр задачи, возвращающей станицу.</returns>
        public async Task<string> GetContent(string link)
        {
            using (var response = await _client.GetAsync(link))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// Создает задачу, которая получает base64 изображение.
        /// </summary>
        /// <param name="link">Ссылка на изображение.</param>
        /// <returns>Экземпляр задачи, возвращающей строку с base64 картинкой.</returns>
        public async Task<string> GetImageBase64(string link)
        {
            using (var response = await _client.GetAsync(link))
            {
                response.EnsureSuccessStatusCode();
                return Convert.ToBase64String(await response.Content.ReadAsByteArrayAsync());
            }
        }
    }
}