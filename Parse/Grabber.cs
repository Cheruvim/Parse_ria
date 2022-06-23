namespace Parse
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using HtmlAgilityPack;

    /// <summary>
    /// Сборщик с html страницы.
    /// </summary>
    public class Grabber : IDisposable
    {
        /// <summary>
        /// Парсер.
        /// </summary>
        private readonly Parser _parser;

        /// <summary>
        /// Загрузчик страниц.
        /// </summary>
        private readonly DataDownloader _downloader;

        /// <summary>
        /// Сохранятор.
        /// </summary>
        private readonly PageModelSaver _saver;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Grabber" />.
        /// </summary>
        /// <param name="parser">Парсер.</param>
        /// <param name="downloader">Загрузчик страниц.</param>
        /// <param name="saver">Сохранятор результатов.</param>
        public Grabber(Parser parser, DataDownloader downloader, PageModelSaver saver)
        {
            _parser = parser;
            _downloader = downloader;
            _saver = saver;
        }

        /// <summary>
        /// Создает задачу, которая парсит страницу.
        /// </summary>
        /// <param name="url">Строка с сылкой на запись для парсинга.</param>
        /// <param name="folderPath">Путь к файлу для сохранения результата.</param>
        /// <returns>Экземпляр задачи, представляющей операцию парсинга записи.</returns>
        public async Task GrabPageAsync(string url, string folderPath)
        {
            var linkIsValid = ValidateUrl(url);
            if (!linkIsValid)
                throw new Exception("Ссылка не соответствует шаблону ссылки на новость риа ру");

            var html = await _downloader.GetContent(url).ConfigureAwait(false);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var page = await _parser.ParsePage(doc).ConfigureAwait(false);
            _saver.SavePageModel(page, folderPath);
        }

        /// <summary>
        /// Проверяет, валидна ли ссылка.
        /// </summary>
        /// <param name="link">Строка со входящей ссылкой.</param>
        /// <returns>Значение, показывающее, валидна ли ссылка.</returns>
        private static bool ValidateUrl(string link)
        {
            return Regex.IsMatch(link, "https://ria\\.ru/\\d{8}/[\\w-]+\\.html");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _parser?.Dispose();
            _downloader?.Dispose();
        }
    }
}