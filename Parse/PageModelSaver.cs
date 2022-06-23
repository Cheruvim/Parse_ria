namespace Parse
{
    using System.IO;
    using System.Xml.Linq;

    /// <summary>
    /// Сохранятоор.
    /// </summary>
    public class PageModelSaver
    {
        /// <summary>
        /// Сохранить результат.
        /// </summary>
        /// <param name="pageModel">Объект, который собрали.</param>
        /// <param name="folderPath">Путь к файлу для сохранения.</param>
        public void SavePageModel(PageModel pageModel, string folderPath)
        {
            Directory.CreateDirectory(folderPath);

            var xDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("Site",
                    new XElement("Title", pageModel.Title),
                    new XElement("Text", pageModel.Text),
                    new XElement("Date", pageModel.DateSting),
                    new XElement("Img", pageModel.ImageBase64)
                ));


            var fileName = pageModel.Title;
            foreach (var invalidFileNameChar in Path.GetInvalidFileNameChars())
                fileName = fileName.Replace(invalidFileNameChar, ' ');

            xDoc.Save(Path.Combine(folderPath, fileName + ".xml"));
        }
    }
}