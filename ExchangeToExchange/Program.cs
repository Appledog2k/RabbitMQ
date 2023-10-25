using RabbitMQ.Client;
using System.Text;

namespace ExchangeToExchange
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
			channel.ExchangeDeclare("demo-direct-exchange1", ExchangeType.Direct, true, false, null);
			channel.ExchangeDeclare("demo-direct-exchange2", ExchangeType.Direct, true, false, null);

			// 4. Declare queue
			channel.QueueDeclare("demo-direct-queue1", true, false, false, null);
			channel.QueueDeclare("demo-direct-queue2", true, false, false, null);

			// 5. Bind queue to exchange
			channel.QueueBind("demo-direct-queue1", "demo-direct-exchange1", "key1", null);
			channel.QueueBind("demo-direct-queue2", "demo-direct-exchange2", "key2", null);

			channel.ExchangeBind("demo-direct-exchange2", "demo-direct-exchange1", "key2", null);

			// 6. Publish message
			var message1 = "Message with routing key 1";
			var body = Encoding.UTF8.GetBytes(message1);
			channel.BasicPublish("demo-direct-exchange1", "key1", null, body);

			var message2 = "Message with routing key 2";
			body = Encoding.UTF8.GetBytes(message2);
			channel.BasicPublish("demo-direct-exchange1", "key2", null, body);

			Console.WriteLine("Message sent. Press a key to exit.");
			
			Console.ReadKey();

			// 7. Delete queue
			channel.QueueDelete("demo-direct-queue1");
			channel.QueueDelete("demo-direct-queue2");

			// 8. Delete exchange
			channel.ExchangeDelete("demo-direct-exchange1");
			channel.ExchangeDelete("demo-direct-exchange2");

			// 9. Close connection
			channel.Close();
			connection.Close();
		}
	}
}