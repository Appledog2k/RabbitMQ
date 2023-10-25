using RabbitMQ.Client;
using System.Text;

namespace HeadersExchange
{
	internal class Program
	{
		static void Main(string[] args)
		{
			IModel channel;
			// 1. Create connection
			var factory = new ConnectionFactory
			{
				Uri = new Uri("amqps://ghvusini:rHDdUg0xm8Rx0dVMWEaeQaepC4h79TRK@octopus.rmq3.cloudamqp.com/ghvusini"),
			};

			// 2. create channel
			var connection = factory.CreateConnection();
			channel = connection.CreateModel();

			// 3. Declare exchange
			channel.ExchangeDeclare("demo-headers-exchange", ExchangeType.Headers, true, false, null);

			// 4. Declare queue
			channel.QueueDeclare("demo-headers-queue1", true, false, false, null);
			channel.QueueDeclare("demo-headers-queue2", true, false, false, null);

			// 5. Bind queue
			channel.QueueBind("demo-headers-queue1", "demo-headers-exchange", string.Empty, new Dictionary<string, object>
			{
				{ "x-match", "all" },
				{ "job", "convert" },
				{ "format", "jpeg" }
			});

			channel.QueueBind("demo-headers-queue2", "demo-headers-exchange", string.Empty, new Dictionary<string, object>
			{
				{ "x-match", "any" },
				{ "job", "convert" },
				{ "format", "jpeg" }
			});

			// 6. Publish message

			var properties = channel.CreateBasicProperties();
			properties.Headers = new Dictionary<string, object>
			{
				{ "job", "convert" },
				{ "format", "jpeg" }
			};

			channel.BasicPublish("demo-headers-exchange", string.Empty, properties, Encoding.UTF8.GetBytes("Hello World 1"));

			properties = channel.CreateBasicProperties();
			properties.Headers = new Dictionary<string, object>
			{
				{ "job", "convert" },
				{ "format", "bmp" }
			};

			channel.BasicPublish("demo-headers-exchange", string.Empty, properties, Encoding.UTF8.GetBytes("Hello World 2"));

			Console.WriteLine("Message sent. Press a key to exit.");
			Console.ReadKey();

			// 7. Delete queue
			channel.QueueDelete("demo-headers-queue1");
			channel.QueueDelete("demo-headers-queue2");

			// 8. Delete exchange
			channel.ExchangeDelete("demo-headers-exchange");

			// 9. Close connection
			channel.Close();
			connection.Close();
		}
	}
}