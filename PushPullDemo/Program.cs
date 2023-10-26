using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PushPullDemo
{
	internal class Program
	{
		static IModel channel;
		static void Main(string[] args)
		{
			// 1. Create connection
			var factory = new ConnectionFactory
			{
				Uri = new Uri("amqps://ghvusini:rHDdUg0xm8Rx0dVMWEaeQaepC4h79TRK@octopus.rmq3.cloudamqp.com/ghvusini"),
			};

			// 2. create channel
			var connection = factory.CreateConnection();
			channel = connection.CreateModel();

			//readMessagesWithPushModel();
			readMessagesWithPullModel();

			// 3. close channel and connection
			channel.Close();
			connection.Close();
		}

		private static void readMessagesWithPushModel()
		{
			// 3. Create Consumer
			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += (sender, e) =>
			{
				string message = Encoding.UTF8.GetString(e.Body.ToArray());
				Console.WriteLine("Message: " + message);
			};

			// 4. Make channel consume message from queue
			var consumerTag = channel.BasicConsume("demo-queue1", true, consumer);

			Console.WriteLine("Waiting for message. Press any key to exist");
			Console.ReadKey();

			channel.BasicCancel(consumerTag);
		}

		private static void readMessagesWithPullModel()
		{
			Console.WriteLine("Reading messages with pull model");

			while (true)
			{
				Console.WriteLine("Trying to get a message from the queue");

				var result = channel.BasicGet("demo-queue1", true);
				if(result != null)
				{
					string message = Encoding.UTF8.GetString(result.Body.ToArray());
					Console.WriteLine("Message: " + message);
				}

				if (Console.KeyAvailable)
				{
					var keyInfo = Console.ReadKey();
					if(keyInfo.KeyChar == 'e' || keyInfo.KeyChar == 'E')
						return;
				}
				
				Thread.Sleep(2000);
			}

		}
	}
}