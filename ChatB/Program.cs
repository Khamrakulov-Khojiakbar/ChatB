using System.Net;
using System.Net.Http;
using System.Text;

namespace ConsoleChatAppSideB;

public class Program
{
    static HttpClient client = new HttpClient();
    static HttpListener listener = new HttpListener();
    static string cookie;
    static async Task Main(string[] args)
    {
        Console.WriteLine("Your username is Lenovo ");
        string username = "Lenovo";
        //cookie = Guid.NewGuid().ToString();
        client.DefaultRequestHeaders.Add("User-Agent", $"{username}");
        //client.DefaultRequestHeaders.Add("Cookie", $"{cookie}");

        listener.Prefixes.Add("http://127.0.0.1:9589/");
        listener.Start();

        var tasks = new Task[2];
        tasks[0] = Tinglovchi();
        tasks[1] = Sender();

        await Task.WhenAll(tasks);

        listener.Stop();
        await Console.Out.WriteLineAsync("Finished");
    }

    public static async Task Tinglovchi()
    {
        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;

            //string receivedCookie = request.Cookies["chatCookie"].Value;
            //if (receivedCookie != cookie)
            //{
            //    context.Response.StatusCode = 401;
            //    context.Response.Close();
            //    return;
            //}

            using Stream stream = request.InputStream;
            using StreamReader reader = new StreamReader(stream, request.ContentEncoding);
            string resMessage = await reader.ReadToEndAsync();
            Console.WriteLine($"{request.UserAgent}: {resMessage}");

            var response = context.Response;
            //request.Cookies.Add(new Cookie("chatCookie", cookie));
            string text = "Message succesfully handled";
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer);
            await response.OutputStream.FlushAsync();
        }
    }

    public static async Task Sender()
    {
        while (true)
        {
            Console.WriteLine("Xabarni kiriting: ");
            StringContent content = new StringContent(Console.ReadLine() ?? "No message");
            //content.Headers.Add("Cookie", $"{cookie}");
            var response = await client.PostAsync("http://127.0.0.1:9588/", content);
            //if (response.StatusCode == HttpStatusCode.Unauthorized)
            //{
            //    Console.WriteLine("Xabar yuborib bo'lmadi: Cookie mos kelmaydi");
            //    return;
            //}
            await Console.Out.WriteLineAsync(await response.Content.ReadAsStringAsync());
        }
    }
}

