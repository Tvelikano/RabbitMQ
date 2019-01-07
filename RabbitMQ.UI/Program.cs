using Ninject;
using Ninject.Parameters;

using RabbitMQ.Service;

using System;
using System.Reflection;

namespace RabbitMQ.UI
{
    internal class Program
    {
        private static void Main()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            var userName = Ask("Please enter your nick name: ");

            using (var rabbitService = kernel.Get<IRabbitService>(new ConstructorArgument(nameof(userName), userName)))
            {
                Console.WriteLine("Welcome to the chat (type 'exit' to quit)");

                rabbitService.Received += ConsumerReceived;

                while (true)
                {
                    Console.WriteLine("> ");
                    var input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        continue;
                    }

                    if (input.Equals("exit"))
                    {
                        break;
                    }

                    rabbitService.SendMessage(input);
                }
            }
        }

        private static void ConsumerReceived(object sender, BasicReceiveEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static string Ask(string message)
        {
            string result = null;
            while (string.IsNullOrWhiteSpace(result))
            {
                Console.Write(message);
                result = Console.ReadLine();
            }
            return result;
        }
    }
}

