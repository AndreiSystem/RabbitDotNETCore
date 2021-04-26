using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Criar conexao com o rabbitMQ
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };
            using var connection = factory.CreateConnection();
            using (var channel = connection.CreateModel())
            {
                // Criar a fila (não é necessário criar no código)
                // dica: Criar a fila no startUp e não dentro do código da regra de negócio
                channel.QueueDeclare(
                    queue: "queue_produtos",
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                
                // Consumidor
                var consumer = new EventingBasicConsumer(channel);
                
                // Received  é chamado toda vez que a mensagem chegar na fila com 
                // os dois parametros "model" e "ea"
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received [0]", message);
                        Console.WriteLine($"Message: {message}");
                        
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        // toda vez que houver uma exceção, ele vai retornar um NAK
                        // e devolver a mensagem para fila
                        Console.WriteLine($"Exception: {ex}.");
                        channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                };
                
                // Configurações do consumer (qual fila ele irá consumir)
                channel.BasicConsume(
                    queue: "queue_produtos",
                    // autoAck = true (toda vez que obter um item da fila ele vai
                    // retornar um AK)
                    autoAck: false,
                    consumer: consumer
                );
                
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}