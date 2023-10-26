using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Requestor
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

			// 3. Receive message
			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += (sender, eventArgs) =>
			{
				var body = eventArgs.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine($"Response received: {message}");
			};

			channel.BasicConsume("responses", true, consumer);

			// 4. Request
			while (true)
			{
				Console.Write("Enter your request: ");
				var request = Console.ReadLine();
				var body = Encoding.UTF8.GetBytes(request);

				if (request.Equals("exit"))
					break;

				channel.BasicPublish("", "requests", null, body);
			}

			// 5. Close connection
			channel.Close();
			connection.Close();
		}
	}
}