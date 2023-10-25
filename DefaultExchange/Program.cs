using RabbitMQ.Client;
using System.Text;

namespace DefaultExchange
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

			// 3. Create Queue
			channel.QueueDeclare("demo-queue1", true, false, false, null);

			channel.QueueDeclare("demo-queue2", true, false, false, null);

			// 4. Publish message
			var message1 = "Message with routing key info";
			var body = Encoding.UTF8.GetBytes(message1);
			channel.BasicPublish("", "demo-queue1", null, body);

			var message2 = "Message with routing key warning";
			body = Encoding.UTF8.GetBytes(message2);
			channel.BasicPublish("", "demo-queue2", null, body);

			Console.WriteLine("Message sent. Press a key to exit.");
			Console.ReadKey();

			// 5. Delete queue
			channel.QueueDelete("demo-queue1");
			channel.QueueDelete("demo-queue2");

			// 6. Close connection
			channel.Close();
			connection.Close();
		}
	}
}