namespace Parse
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using HtmlAgilityPack;

    /// <summary>
    /// Парсер.
    /// </summary>
    public class Parser : IDisposable
    {
        /// <summary>
        /// Загрузчик страниц.
        /// </summary>
        private readonly DataDownloader _downloader;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Parser" />.
        /// </summary>
        /// <param name="downloader">Загрузчик страниц.</param>
        public Parser(DataDownloader downloader)
        {
            _downloader = downloader;
        }

        /// <summary>
        /// Создает задачу, которая парсит страницу.
        /// </summary>
        /// <param name="doc">Represents a complete HTML document.</param>
        /// <returns>Экземпляр задачи, возвращающей модель для сбора.</returns>
        public async Task<PageModel> ParsePage(HtmlDocument doc)
        {
            var imgBase64 = await GetImage(doc).ConfigureAwait(false);
            var titleStr = GEtTitle(doc);
            var dateStr = GetDate(doc);
            var textStr = GetText(doc);

            return new PageModel
                {
                    Text = textStr,
                    DateSting = dateStr,
                    Title = titleStr,
                    ImageBase64 = imgBase64
                };
        }

        /// <summary>
        /// Получает строку парсит текст.
        /// </summary>
        /// <param name="doc">Represents a complete HTML document.</param>
        /// <returns>Строку парсит текст.</returns>
        private string GetText(HtmlDocument doc)
        {
            var texts = doc.DocumentNode.SelectNodes(
                ".//div[@class='article__body js-mediator-article mia-analytics']//div[@class='article__block']//div[@class='article__text']");
            if (texts == null || !texts.Any())
                throw new Exception("На странице не найден текст");

            var textStr = string.Join(Environment.NewLine, texts.Select(x => x.InnerText));
            return textStr;
        }

        /// <summary>
        /// Получает строку с датой.
        /// </summary>
        /// <param name="doc">Represents a complete HTML document.</param>
        /// <returns>Строку с датой.</returns>
        private string GetDate(HtmlDocument doc)
        {
            var date = doc.DocumentNode.SelectSingleNode(".//div[@class='article__info-date']");
            if (date == null)
                throw new Exception("На странице не найдена дата публикации");

            var dateStr = date.InnerText;
            return dateStr;
        }

        /// <summary>
        /// Получает строку с заголовком.
        /// </summary>
        /// <param name="doc">Represents a complete HTML document.</param>
        /// <returns>Строку с заголовком.</returns>
        private string GEtTitle(HtmlDocument doc)
        {
            var title = doc.DocumentNode.SelectSingleNode(".//h1[@class='article__title']");
            if (title == null)
                throw new Exception("На странице не найден заголовок");

            var titleStr = title.InnerText;
            return titleStr;
        }

        /// <summary>
        /// Создает задачу, которая собирает картинку.
        /// </summary>
        /// <param name="doc">Represents a complete HTML document.</param>
        /// <returns>Экземпляр задачи, возвращающей строку с base64 картинкой.</returns>
        private async Task<string> GetImage(HtmlDocument doc)
        {
            var img = doc.DocumentNode.SelectSingleNode(".//div[@class='photoview__open']//img");
            if (img == null)
                throw new Exception("На странице нет изображения");

            var imgLink = img.Attributes["src"].Value;
            var imgBase64 = await _downloader.GetImageBase64(imgLink).ConfigureAwait(false);
            return imgBase64;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _downloader?.Dispose();
        }
    }
}