using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace Bot_WeatherCounter
{
    class Program
    {
        static ITelegramBotClient _botClient;
        //Contador
        static int contador = 0;
        //Variables del clima
        static string temperatura, humedad, pais, clima;
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
            //El bot detiene su funcionamiento
            _botClient.StopReceiving();
        }

        //Obtengo una string con todos los datos del clima
        static string getClima(string ciudad) 
        {
            try
            {
                using (WebClient web = new WebClient())
                {
                    string apiId = "6cb89b163832df06b4acab8f9545aa6e";
                    string url = string.Format($"http://api.openweathermap.org/data/2.5/weather?q={ciudad}&appid={apiId}");
                    var json = web.DownloadString(url);
                    Root result = JsonConvert.DeserializeObject<Root>(json);

                    temperatura = string.Format("🌡" + "{0:N1} \u00B0" + "C", result.main.temp - 273, 15);
                    humedad = string.Format("💧" + "{0}", result.main.humidity);
                    pais = string.Format("🌍" + "{0}", result.sys.country);
                    ciudad = string.Format("🗺" + "{0}", result.name);
                    clima = string.Format("☁" + "{0}", result.weather[0].description);

                    string info = $"Ciudad: {ciudad}\n" +
                                  $"País: {pais}\n" +
                                  $"Temperatura: {temperatura}\n" +
                                  $"Humedad: {humedad}%\n" +
                                  $"Clima: {clima}";

                    return info;
                }

            }
            catch (Exception)
            {
                string error = "Lo siento no existe ese lugar 😢";
                return error;
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
                contador++;
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
                          text: $"Me has hablado {contador} veces"
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
        
        public class Coord
        {
            public double lon { get; set; }
            public double lat { get; set; }
        }

        public class Weather
        {
            public int id { get; set; }
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
        }

        public class Main1
        {
            public double temp { get; set; }
            public double feels_like { get; set; }
            public double temp_min { get; set; }
            public double temp_max { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
        }

        public class Wind
        {
            public double speed { get; set; }
            public int deg { get; set; }
            public double gust { get; set; }
        }

        public class Clouds
        {
            public int all { get; set; }
        }

        public class Sys
        {
            public int type { get; set; }
            public int id { get; set; }
            public string country { get; set; }
            public int sunrise { get; set; }
            public int sunset { get; set; }
        }

        public class Root
        {
            public Coord coord { get; set; }
            public List<Weather> weather { get; set; }
            public string @base { get; set; }
            public Main1 main { get; set; }
            public int visibility { get; set; }
            public Wind wind { get; set; }
            public Clouds clouds { get; set; }
            public int dt { get; set; }
            public Sys sys { get; set; }
            public int timezone { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public int cod { get; set; }
        }


    }
}
