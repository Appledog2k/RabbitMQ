using RabbitMQ.Client;
using System.Text;

namespace AlternateExchange
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
			channel.ExchangeDeclare("demo-fanout-exchange", ExchangeType.Fanout, true, false, null);

			channel.ExchangeDeclare(
				"demo-direct-exchange",
				ExchangeType.Direct,
				true,
				false,
				new Dictionary<string, object>
				{
					{ "alternate-exchange", "demo-fanout-exchange" }
				});

			// 4. Declare queue
			channel.QueueDeclare("demo-queue1", true, false, false, null);
			channel.QueueDeclare("demo-queue2", true, false, false, null);

			channel.QueueDeclare("demo-queue-unrouted", true, false, false, null);

			// 5. Bind queue

			channel.QueueBind("demo-queue1", "demo-direct-exchange", "video", null);
			channel.QueueBind("demo-queue2", "demo-direct-exchange", "image", null);
			channel.QueueBind("demo-queue-unrouted", "demo-fanout-exchange", "video", null);

			// 6. Publish message
			var message1 = "Message with routing key video";
			var body = Encoding.UTF8.GetBytes(message1);
			channel.BasicPublish("demo-direct-exchange", "video", null, body);

			var message2 = "Message with routing key text";
			body = Encoding.UTF8.GetBytes(message2);
			channel.BasicPublish("demo-direct-exchange", "text", null, body);

			Console.WriteLine("Message sent. Press a key to exit.");
			Console.ReadKey();

			// 7. Delete queue
			channel.QueueDelete("demo-queue1");
			channel.QueueDelete("demo-queue2");
			channel.QueueDelete("demo-queue-unrouted");

			// 8. Delete exchange
			channel.ExchangeDelete("demo-direct-exchange");
			channel.ExchangeDelete("demo-fanout-exchange");

			// 9. Close connection
			channel.Close();
			connection.Close();
		}
	}
}