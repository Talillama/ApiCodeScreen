using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace ApiCodeScreen
{
    class Program
    {
        const string dataSourceUrl = "https://k7o2mgxtv8.execute-api.us-east-1.amazonaws.com/public/manufacturers";
        const string carNamesPostUrl = "https://k7o2mgxtv8.execute-api.us-east-1.amazonaws.com/public/names";
        const string carColorsPostUrl = "https://k7o2mgxtv8.execute-api.us-east-1.amazonaws.com/public/colors";

        static void Main(string[] args)
        {
            var data = GetManufacturers();
            var obj = JsonConvert.DeserializeObject<Root>(data).Manufacturers;
            if (obj != null)
            {
                PostFordCarsAlphabetically(obj);
                PostCarColors(obj);
            }
            else
            {
                Console.WriteLine("obj was null");
            }
            
        }

        private static string GetManufacturers()
        {
            var url = dataSourceUrl;

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using var webResponse = (HttpWebResponse)request.GetResponse();
            using var webStream = webResponse.GetResponseStream();

            using var reader = new StreamReader(webStream);
            var data = reader.ReadToEnd();

            Console.WriteLine("Manufacturer list from Get:");
            Console.WriteLine(data);
            Console.WriteLine();
            Console.WriteLine("Manufacturer Get Status:" + webResponse.StatusCode.ToString());

            return data;
        }

        private static void PostFordCarsAlphabetically(List<Manufacturer> obj)
        {
            var ford = obj.FirstOrDefault(x => x.Name == "Ford");
            var cars = ford.Cars;
            Console.WriteLine();
            Console.WriteLine("Ford cars received:");
            foreach (var car in cars)
            {
                Console.WriteLine(car.Name.ToString());
            }
            var sortedCars = cars.OrderBy(x => x.Name).ToList();
            List<string> carsToReturn = new List<string>();
            Console.WriteLine();
            Console.WriteLine("Alphabetized Ford cars:");
            for (int i = 0; i < cars.Count; i++)
            {
                carsToReturn.Add(sortedCars[i].Name.ToString());
                Console.WriteLine(carsToReturn[i]);
            }
            var fordCars = new FordCars() { Names = carsToReturn };
            var json = JsonConvert.SerializeObject(fordCars);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(carNamesPostUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Console.WriteLine();
            Console.WriteLine("Alphabetized Ford Cars Posted Status: " + httpResponse.StatusCode.ToString());
        }

        private static void PostCarColors(List<Manufacturer> obj)
        {
            var chevyCars = obj.FirstOrDefault(x => x.Name == "Chevy").Cars;
            var carColors = chevyCars.Select(x => x.Colors).ToList();
            var allColors = new List<string>();
            Console.WriteLine();
            Console.WriteLine("All colors received for all Chevy models:");
            foreach (var colorArray in carColors)
            {
                foreach (var color in colorArray)
                {
                    allColors.Add(color.ToString());
                    Console.WriteLine(color);
                }
            }
            
            var uniqueColorsToReturn = allColors.Distinct().OrderBy(x => x).ToList();
            Console.WriteLine();
            Console.WriteLine("All unique Chevy car colors:");
            foreach (var color in uniqueColorsToReturn)
            {
                Console.WriteLine(color);
            }
            var chevyColors = new ChevyColors() { Colors = uniqueColorsToReturn };
            var json = JsonConvert.SerializeObject(chevyColors);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(carColorsPostUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Console.WriteLine();
            Console.WriteLine("Alphabetized Unique Chevy Car Colors Posted Status: " + httpResponse.StatusCode.ToString());
        }
    }



    class Root
    {
        public List<Manufacturer> Manufacturers { get; set; } = null!;
    }

    class Manufacturer
    {
        public string Name { get; set; } = null!;
        public List<Car> Cars { get; set; } = new List<Car>();
    }

    class Car
    {
        public string Name { get; set; } = null!;
        public List<string> Colors { get; set; } = new List<string>();
    }

    class FordCars
    {
        public List<string> Names { get; set; } = new List<string>();
    }

    class ChevyColors
    {
        public List<string> Colors { get; set;} = new List<string>();
    }
}