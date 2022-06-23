namespace Parse
{
    /// <summary>
    /// Модель для сбора.
    /// </summary>
    public class PageModel
    {
        /// <summary>
        /// Получает или задает строку с заголовком.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Получает или задает строку с текстом.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Получает или задает строку с датой.
        /// </summary>
        public string DateSting { get; set; }

        /// <summary>
        /// Получает или задает строку с base64 картинкой.
        /// </summary>
        public string ImageBase64 { get; set; }
    }
}