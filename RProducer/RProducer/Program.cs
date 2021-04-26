using System;
using System.Text;
using RabbitMQ.Client;

namespace RProducer
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
            
            // Ativar vários canais através da mesma conexão
            using (var channel = connection.CreateModel())
            {
                // Criar a fila (não é necessário criar no código)
                // dica: Criar a fila no startUp e não dentro do código da regra de negócio
                channel.QueueDeclare(
                    queue: "queue_produtos",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                string message = "Produto Teste";
                var body = Encoding.UTF8.GetBytes(message);
                
                // Publicação da mensagem
                channel.BasicPublish(
                    exchange: "",
                    routingKey: "queue_produtos",
                    basicProperties: null,
                    body: body
                );
                
                Console.Write("[x] Sent {0}", message);
            }
            
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}