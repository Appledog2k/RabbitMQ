using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace WorkQueuesFair
{
	internal class Program
	{
		static IModel channel;
		static void Main(string[] args)
		{
			Console.WriteLine("Enter the name for this worker");
			string workerName = Console.ReadLine();

			// 1. Create connection
			var factory = new ConnectionFactory
			{
				Uri = new Uri("amqps://ghvusini:rHDdUg0xm8Rx0dVMWEaeQaepC4h79TRK@octopus.rmq3.cloudamqp.com/ghvusini"),
			};

			// 2. create channel
			var connection = factory.CreateConnection();
			channel = connection.CreateModel();
			channel.BasicQos(0, 1, false);

			// 3. Create Consumer
			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += (sender, e) =>
			{
				string message = Encoding.UTF8.GetString(e.Body.ToArray());
				int durationInSeconds = Int32.Parse(message);
				Console.WriteLine($"[{workerName}] Task Started. Duration: " + durationInSeconds);

				Thread.Sleep(durationInSeconds * 1000);

				Console.WriteLine("Task Finished");

				channel.BasicAck(e.DeliveryTag, false);
			};

			// 4. Make channel consume message from queue
			var consumerTag = channel.BasicConsume("demo-queue1", false, consumer);

			Console.WriteLine("Waiting for message. Press any key to exist");
			Console.ReadKey();

			channel.Close();
			connection.Close();
		}
	}
}