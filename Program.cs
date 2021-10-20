using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections;
using System.Net;

namespace Bot_WeatherCounter
{
    class Program
    {
        static ITelegramBotClient _botClient;
        static int counter = 0; 
        static void Main(string[] args)
        {
            _botClient = new TelegramBotClient("2039709074:AAGmVM1KWSTZ2UyJInxXxtFAqu2gCQzqzPY");
            var me = _botClient.GetMeAsync().Result;

            //Para ver en consola
            Console.WriteLine($"Hola, soy {me.Id} y mi nombre es: {me.FirstName}"); 

            //El bot empieza a funcionar
            _botClient.OnMessage += _botClient_OnMessage; 
            _botClient.StartReceiving();

            Console.WriteLine("Pulse una tecla para salir");                        
            Console.ReadKey();

            _botClient.StopReceiving();
        }

        //Obtengo una string con todos los datos del clima
        static String getClima(string ciudad) 
        {
            using (WebClient web = new WebClient())
            {
                string apiId = "6cb89b163832df06b4acab8f9545aa6e";
                string url = string.Format($"http://api.openweathermap.org/data/2.5/weather?q={ciudad}&appid={apiId}");
                var json = web.DownloadString(url);
                return json;
            }
            
        }


        private async static void _botClient_OnMessage(object sender, MessageEventArgs e)
        {
            //Menu
            ReplyKeyboardMarkup replyKeyboard = new[]
            {
                new[] {"¡Quiero saber el clima! 🌥"},
                new[] {"¡Quiero contar! 🔢"},
                new[] {"Cerrar"},
            };

            //Menu
            ReplyKeyboardMarkup replyKeyboard2 = new[]
            {
                new[] {"Montevideo"},
                new[] {"Paysandú"},
                new[] {"Otra Ciudad"},
                new[] {"Cerrar"},
            };


            if (e.Message.Text != null)
            {
                counter++;
                Console.WriteLine($"Mensaje recibido");

                //Comando inicial, devuelve mensaje con menú
                if (e.Message.Text.ToLower().Contains("/start")) 
                {
                    await _botClient.SendTextMessageAsync(
                          chatId: e.Message.Chat.Id,
                          text: "Hola, que necesitas? 😄",
                          replyMarkup: replyKeyboard
                          );
                }
                //Opción clima, devuelve un menu con opciones
                else if (e.Message.Text.ToLower().Contains("¡quiero saber el clima! 🌥")) 
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat.Id,
                        text: "Dime de que ciudad quieres saber el clima",
                        replyMarkup: replyKeyboard2
                        );
                    
                }
                //Opcion contar, devuelve la cantidad de mensajes que el bot recibió
                else if (e.Message.Text.ToLower().Contains("¡quiero contar! 🔢"))
                {
                    await _botClient.SendTextMessageAsync(
                          chatId: e.Message.Chat.Id,
                          text: $"Me has hablado {counter} veces"
                          );
                }
                //Opcion cerrar, cierra el menú
                else if (e.Message.Text.ToLower().Contains("cerrar"))
                {
                    await _botClient.SendTextMessageAsync(
                          chatId: e.Message.Chat.Id,
                          text: "¡Estoy para lo que necesites!",
                          replyMarkup: new ReplyKeyboardRemove()
                          );
                }
                //Opcion otra ciudad, devuelve mensaje
                else if (e.Message.Text.ToLower().Contains("otra ciudad"))
                {
                    await _botClient.SendTextMessageAsync(
                          chatId: e.Message.Chat.Id,
                          text: "¡Dime cual Ciudad!"
                          );
                }
                //Devuelve mensaje con todos los datos de la string sobre el clima
                else
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat.Id,
                        text: getClima(e.Message.Text)
                        );
                    
                }

            }
        }   
    }
}
