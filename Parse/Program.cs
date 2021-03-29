using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Parse
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main()
        {
            Console.WriteLine("Введите ссылку, пример: https://ria.ru/20201102/pitanie-1582693046.html");
            try
            {

                #region parse html
                //берем html сайта
                string link = Console.ReadLine(); //вводим ссылку
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();//созадем обьект под html
                doc.LoadHtml(await GetContent(link)); //вытаскиваем html

                //вытаскиваем ссылку на картинку
                var img = doc.DocumentNode.SelectSingleNode(".//div[@class='photoview__open']//img");
                if (img == null) { throw new Exception("Некоректная ссылка"); }
                string imgStr = img.Attributes["src"].Value;
                string jpeg = Convert.ToBase64String(new System.Net.WebClient().DownloadData(imgStr));
                Console.WriteLine(imgStr);

                //заголовок
                var title = doc.DocumentNode.SelectSingleNode(".//h1[@class='article__title']");
                if (title == null) { throw new Exception("Некоректная ссылка"); }
                string titleStr = title.InnerText;
                Console.WriteLine(titleStr);

                //дату публикации
                var date = doc.DocumentNode.SelectSingleNode(".//div[@class='article__info-date']");
                if (date == null) { throw new Exception("Некоректная ссылка"); }
                string dateStr = date.InnerText;
                Console.WriteLine(dateStr);

                //текст новости
                string textStr = "";
                var texts = doc.DocumentNode.SelectNodes(".//div[@class='article__body js-mediator-article mia-analytics']//div[@class='article__block']//div[@class='article__text']");
                if (texts == null) { throw new Exception("Некоректная ссылка"); }
                //собираем такст из нескольких div в 1 переменную
                foreach (var text in texts)
                {
                    textStr += text.InnerText;
                    Console.WriteLine(text.InnerText);
                }
                #endregion

                try
                {

                    Console.WriteLine("Введите адресс папки для сохранения, пример: C://1/\nНазвание файла бедет равно заголовку новости");
                    string address = Console.ReadLine();
                    XDocument xDoc = new XDocument(
                        new XDeclaration("1.0", "utf-8", "yes"),
                        new XElement("Site",
                            new XElement("Title", titleStr),
                            new XElement("Text", textStr),
                            new XElement("Date", dateStr),
                            new XElement("Img", jpeg)
                            )
                        );
                    xDoc.Save($"{address}{titleStr}.xml");
                    Console.WriteLine("Success!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("\nException Caught! \nWrong file address  ");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught! \nWrong link  ");
            }
        }
        // получение html по ссылке
        static async Task<string> GetContent(string link)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(link);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }
    }
}
