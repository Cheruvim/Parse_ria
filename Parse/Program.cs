using System;
using System.Threading.Tasks;

namespace Parse
{
    class Program
    {
        static async Task Main()
        {
            var downloader = new DataDownloader();
            var parser = new Parser(downloader);
            var saver = new PageModelSaver();
            var grabber = new Grabber(parser, downloader, saver);

            using (grabber)
            {
                Console.WriteLine("Введите ссылку, пример: https://ria.ru/20201102/pitanie-1582693046.html");
                var link = Console.ReadLine();

                Console.WriteLine("Введите адресс папки для сохранения, пример: C://1/\nНазвание файла будет равно заголовку новости");
                var folderPath = Console.ReadLine();

                try
                {
                    await grabber.GrabPageAsync(link, folderPath).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}