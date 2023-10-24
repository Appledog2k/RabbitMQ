using RabbitMQ.Client;
using System.Text;

namespace FanoutPublisher
{
	internal class Program
	{
		static void Main(string[] args)
		{
			// 1. Create connection
			var factory = new ConnectionFactory
			{
				Uri = new Uri("amqps://ghvusini:rHDdUg0xm8Rx0dVMWEaeQaepC4h79TRK@octopus.rmq3.cloudamqp.com/ghvusini"),
			};
			// 2. create channel
			var connection = factory.CreateConnection();
			using var channel = connection.CreateModel();

			// 3. Declare exchange
			channel.ExchangeDeclare("demo-fanout-exchange", ExchangeType.Fanout, true, false, null);

			// 4. Declare queue
			channel.QueueDeclare("demo-fanout-queue1", true, false, false, null);
			channel.QueueDeclare("demo-fanout-queue2", true, false, false, null);

			// 5. Bind queue to exchange
			channel.QueueBind("demo-fanout-queue1", "demo-fanout-exchange", "", null);
			channel.QueueBind("demo-fanout-queue2", "demo-fanout-exchange", "", null);

			// 6. Publish message
			var message1 = "Hello World";
			var body = Encoding.UTF8.GetBytes(message1);
			channel.BasicPublish("demo-fanout-exchange", "", null, body);

			var message2 = "Hello World Again";
			body = Encoding.UTF8.GetBytes(message2);
			channel.BasicPublish("demo-fanout-exchange", "", null, body);

			Console.WriteLine("Message sent. Press a key to exit.");
			Console.ReadKey();

			// 7. Delete queue
			channel.QueueDelete("demo-fanout-queue1");
			channel.QueueDelete("demo-fanout-queue2");

			// 8. Delete exchange
			channel.ExchangeDelete("demo-fanout-exchange");

			// 9. Close connection
			channel.Close();
			connection.Close();
		}
	}
}