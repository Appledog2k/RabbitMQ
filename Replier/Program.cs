using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Replier
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
				Console.WriteLine($"Request received: {message}");

				string response = $"Response for {message}";

				channel.BasicPublish("", "responses", null, Encoding.UTF8.GetBytes(response));
			};

			channel.BasicConsume("requests", true, consumer);

			Console.WriteLine("Press a key to exit");
			Console.ReadKey();

			// 4. Close connection
			channel.Close();
			connection.Close();
		}
	}
}