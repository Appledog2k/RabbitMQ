using RabbitMQ.Client;
using System.Text;

namespace DirectExchange
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

			// 3. Declare exchange (direct)
			channel.ExchangeDeclare("demo-direct-exchange", ExchangeType.Direct, true, false, null);

			// 4. Declare queue
			channel.QueueDeclare("demo-direct-queue-info", true, false, false, null);
			channel.QueueDeclare("demo-direct-queue-warning", true, false, false, null);
			channel.QueueDeclare("demo-direct-queue-error", true, false, false, null);

			// 5. Bind queue to exchange
			channel.QueueBind("demo-direct-queue-info", "demo-direct-exchange", "info", null);
			channel.QueueBind("demo-direct-queue-warning", "demo-direct-exchange", "warning", null);
			channel.QueueBind("demo-direct-queue-error", "demo-direct-exchange", "error", null);

			// 6. Publish message
			var message1 = "Message with routing key info";
			var body = Encoding.UTF8.GetBytes(message1);
			channel.BasicPublish("demo-direct-exchange", "info", null, body);

			var message2 = "Message with routing key warning";
			body = Encoding.UTF8.GetBytes(message2);
			channel.BasicPublish("demo-direct-exchange", "warning", null, body);

			var message3 = "Message with routing key error";
			body = Encoding.UTF8.GetBytes(message3);
			channel.BasicPublish("demo-direct-exchange", "error", null, body);

			Console.WriteLine("Message sent. Press a key to exit.");
			Console.ReadKey();

			// 7. Delete queue
			channel.QueueDelete("demo-direct-queue-info");
			channel.QueueDelete("demo-direct-queue-warning");
			channel.QueueDelete("demo-direct-queue-error");

			// 8. Delete exchange
			channel.ExchangeDelete("demo-direct-exchange");

			// 9. Close connection
			channel.Close();
			connection.Close();
		}
	}
}