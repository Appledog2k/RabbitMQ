using RabbitMQ.Client;
using System.Text;

namespace TopicExchange
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

			// 3. Declare exchange (topic)
			channel.ExchangeDeclare("demo-topic-exchange", ExchangeType.Topic, true, false, null);

			// 4. Declare queue
			channel.QueueDeclare("demo-topic-queue1", true, false, false, null);
			channel.QueueDeclare("demo-topic-queue2", true, false, false, null);
			channel.QueueDeclare("demo-topic-queue3", true, false, false, null);

			// 5. Bind queue to exchange
			channel.QueueBind("demo-topic-queue1", "demo-topic-exchange", "*.image.*", null);
			channel.QueueBind("demo-topic-queue2", "demo-topic-exchange", "#.image", null);
			channel.QueueBind("demo-topic-queue3", "demo-topic-exchange", "image.#", null);

			// 6. Publish message
			var message1 = "Message with routing key convert.image.bmp";
			var body = Encoding.UTF8.GetBytes(message1);
			channel.BasicPublish("demo-topic-exchange", "convert.image.bmp", null, body);

			var message2 = "Message with routing key convert.bitmap.image";
			body = Encoding.UTF8.GetBytes(message2);
			channel.BasicPublish("demo-topic-exchange", "convert.bitmap.image", null, body);

			var message3 = "Message with routing key convert.bitmap.32bit";
			body = Encoding.UTF8.GetBytes(message3);
			channel.BasicPublish("demo-topic-exchange", "convert.bitmap.32bit", null, body);

			Console.WriteLine("Message sent. Press a key to exit.");
			Console.ReadKey();

			// 7. Delete queue
			channel.QueueDelete("demo-topic-queue1");
			channel.QueueDelete("demo-topic-queue2");
			channel.QueueDelete("demo-topic-queue3");

			// 8. Delete exchange
			channel.ExchangeDelete("demo-topic-exchange");

			// 9. Close connection
			channel.Close();
			connection.Close();
		}
	}
}