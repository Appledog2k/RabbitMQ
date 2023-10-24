using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace FanoutConsumer
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

			// 3. Create Consumer
			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += Consumer_Received;

			// 4. Make channel consume message from queue
			var consumerTag = channel.BasicConsume("demo-fanout-queue1", false, consumer);

			Console.WriteLine("Waiting for message. Press any key to exist");
			Console.ReadKey();

		}

		private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
		{
			var body = e.Body.ToArray();
			var message = Encoding.UTF8.GetString(body);
			Console.WriteLine("Message:" + message);

			channel.BasicNack(e.DeliveryTag, false, false);
		}
	}
}