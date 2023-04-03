using System;
using System.Data.SqlClient;
using System.IO;
using CsvHelper;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Connect to SQL Server database
            string connectionString = "Data Source=DESKTOP-HAE3NHD;Initial Catalog=UserInformation;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Retrieve personal data from SQL table
                string sql = "SELECT * FROM sign_up";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        using (var writer = new StreamWriter("C:\\Users\\Gürkan\\Desktop\\outlook.csv"))

                        using (var csvWriter = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
                        {
                            csvWriter.WriteRecords(reader);
                        }
                    }
                }

                // Close database connection
                connection.Close();
            }

            // Send email with attachment using EmailJS
            var emailJsUserId = "b7zDFG5EnfTBGp91I";
            var emailJsServiceId = "service_9atfwim";
            var emailJsTemplateId = "template_mao0qyh";
            var emailJsApiKey = "ZU2J4OcvUyehW2YkiG5Nd";

            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.emailjs.com/");

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(emailJsUserId), "b7zDFG5EnfTBGp91I");
            content.Add(new StringContent(emailJsServiceId), "service_9atfwim");
            content.Add(new StringContent(emailJsTemplateId), "template_mao0qyh");
            content.Add(new StringContent(emailJsApiKey), "ZU2J4OcvUyehW2YkiG5Nd");
            content.Add(new StringContent("gurkanalikapsizlar2@gmail.com"), "to");
            content.Add(new StringContent("Sender"), "from_name");
            content.Add(new StringContent("Your Personal Data"), "subject");
            content.Add(new StringContent("Please find attached your personal data file."), "message_html");

            var fileContent = new ByteArrayContent(File.ReadAllBytes("C:\\Users\\Gürkan\\Desktop\\outlook.csv"));
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = "outlook.csv"
            };

            content.Add(fileContent);

            var response = await client.PostAsync("/api/v1.0/email/send", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Email sent.");
            }
            else
            {
                Console.WriteLine($"Failed to send email. Status code: {response.StatusCode}");
            }
        }
    }
}
