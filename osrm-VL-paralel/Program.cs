using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace osrm_VL_paralel
{
    class Program
    {
        static void Main(string[] args)
        {
            bool chek = true;

            if (!chek)
            {

                String coordinateStr = File.ReadAllText("convertcsv.json");
                coordinateStr += "}";
                coordinateStr = "{" + "\"coordinates\":" + coordinateStr;
                JObject coordinate = JObject.Parse(coordinateStr);

                const int n=676;
                const int dN=n*n;
                int[,] Answer = new int [n, n];
                // JArray items = (JArray)coordinate["coordinates"];
                //  int length = items.Count;

                int counterer=1;

                //поехали по координатам
                Parallel.ForEach(coordinate["coordinates"], (start) =>
                                                            {
                                                                double xStart=Double.Parse( start["x"].ToString());
                                                                double yStart=Double.Parse( start["y"].ToString());
                                                                Int64 idStart = Int64.Parse( start["id"].ToString())-1;



                                                                Parallel.ForEach(coordinate["coordinates"], (end) =>
                                                                {
                                                                    double xEnd=Double.Parse( end["x"].ToString());
                                                                    double yEnd=Double.Parse( end["y"].ToString());
                                                                    Int64 idEnd = Int64.Parse( end["id"].ToString())-1;

                                                                    String requestStr="http://router.project-osrm.org/viaroute?loc=";
                                                                    requestStr += xStart.ToString().Replace(',', '.') + "," + yStart.ToString().Replace(',', '.') + "&loc=" + xEnd.ToString().Replace(',', '.') + "," + yEnd.ToString().Replace(',', '.') + "&instructions=false";

                                                                    try
                                                                    {
                                                                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestStr);
                                                                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                                                                        if (response.StatusCode == HttpStatusCode.OK)
                                                                        {
                                                                            JObject json = JObject.Parse(new StreamReader(response.GetResponseStream()).ReadToEnd());


                                                                            Console.Clear();
                                                                            Console.WriteLine(counterer.ToString() + "/" + dN.ToString());
                                                                            counterer++;
                                                                            Answer[idStart, idEnd] = int.Parse(json["route_summary"]["total_time"].ToString());

                                                                            //Console.WriteLine("Всё норм.");  

                                                                        }
                                                                        else if (response.StatusCode == HttpStatusCode.NotFound)
                                                                        {
                                                                            Console.WriteLine("Такой страницы нет:");
                                                                            Console.WriteLine(requestStr);
                                                                        }

                                                                        response.Close();
                                                                    }
                                                                    catch (Exception e)
                                                                    {

                                                                        Console.WriteLine("Ошибка");
                                                                        Console.WriteLine(e);
                                                                        Console.ReadLine();
                                                                        StreamWriter ouFiles = new StreamWriter("answer.csv");
                                                                        for (int i = 0; i < 676; i++)
                                                                        {
                                                                            for (int j = 0; j < 676; j++)
                                                                            {
                                                                                if (i == 0)
                                                                                    ouFiles.Write(j + ",");
                                                                                else
                                                                                {
                                                                                    if (j == 0)
                                                                                    {
                                                                                        ouFiles.Write(i + ",");
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        ouFiles.Write(Answer[i - 1, j - 1] + ",");
                                                                                    }
                                                                                }
                                                                            }
                                                                            ouFiles.Write('\n');
                                                                        }
                                                                        ouFiles.Close();

                                                                    }

                                                                });
                                                            });


                Console.ReadLine();
                Console.WriteLine("Готовы писать");
                StreamWriter ouFile = new StreamWriter("answer.csv");
                for (int i = 0; i < 677; i++)
                {
                    for (int j = 0; j < 677; j++)
                    {
                        if (i == 0)
                            ouFile.Write(j + ",");
                        else
                        {
                            if (j == 0)
                            {
                                ouFile.Write(i + ",");
                            }
                            else
                            {
                                ouFile.Write(Answer[i - 1, j - 1] + ",");
                            }
                        }
                    }
                    ouFile.Write('\n');
                }

                ouFile.Close();

                Console.WriteLine("Написали");
                Console.ReadLine();

            }
            else
            {
                string filePath = @"sameAnswer.csv";
                StreamReader sr = new StreamReader(filePath);
                var lines = new List<string[]>();
                int Row = 0;
                while (!sr.EndOfStream)
                {
                    string[] Line = sr.ReadLine().Split(',');
                    lines.Add(Line);
                    Row++;
                    //Console.WriteLine(Row);
                }

                var data = lines.ToArray();
                //Console.WriteLine(data[2][4]);
                Console.WriteLine("Файл считан.");
                Console.ReadLine();
                Console.WriteLine("Начали проверку файла");
                String coordinateStr = File.ReadAllText("convertcsv.json");
                coordinateStr += "}";
                coordinateStr = "{" + "\"coordinates\":" + coordinateStr;
                JObject coordinate = JObject.Parse(coordinateStr);

                const int n=676;
                const int dN=n*n;
                // JArray items = (JArray)coordinate["coordinates"];
                //  int length = items.Count;

                int counterer=1;


                //


                //поехали по координатам
                Parallel.ForEach(coordinate["coordinates"], (start) =>
                {
                    double xStart=Double.Parse( start["x"].ToString());
                    double yStart=Double.Parse( start["y"].ToString());
                    Int64 idStart = Int64.Parse( start["id"].ToString());



                    Parallel.ForEach(coordinate["coordinates"], (end) =>
                    {
                        double xEnd=Double.Parse( end["x"].ToString());
                        double yEnd=Double.Parse( end["y"].ToString());
                        Int64 idEnd = Int64.Parse( end["id"].ToString());


                        if ((data[idStart][idEnd] == "0" || data[idStart][idEnd] == "") && (idEnd != idStart))
                        {
                            String requestStr="http://router.project-osrm.org/viaroute?loc=";
                            requestStr += xStart.ToString().Replace(',', '.') + "," + yStart.ToString().Replace(',', '.') + "&loc=" + xEnd.ToString().Replace(',', '.') + "," + yEnd.ToString().Replace(',', '.') + "&instructions=false";

                            try
                            {
                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestStr);
                                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    JObject json = JObject.Parse(new StreamReader(response.GetResponseStream()).ReadToEnd());


                                    Console.Clear();
                                    Console.WriteLine(counterer.ToString() + "/" + dN.ToString());
                                    counterer++;
                                    data[idStart][idEnd] = json["route_summary"]["total_time"].ToString();

                                    //Console.WriteLine("Всё норм.");  

                                }
                                else if (response.StatusCode == HttpStatusCode.NotFound)
                                {
                                    Console.WriteLine("Такой страницы нет:");
                                    Console.WriteLine(requestStr);
                                }

                                response.Close();
                            }
                            catch (Exception e)
                            {

                                Console.WriteLine("Ошибка");
                                Console.WriteLine(e);
                                Console.ReadLine();
                                StreamWriter ouFiles = new StreamWriter("answer.csv");
                                for (int i = 0; i < 676; i++)
                                {
                                    for (int j = 0; j < 676; j++)
                                    {
                                        if (i == 0)
                                            ouFiles.Write(j + ",");
                                        else
                                        {
                                            if (j == 0)
                                            {
                                                ouFiles.Write(i + ",");
                                            }
                                            else
                                            {
                                                ouFiles.Write(data[i][j] + ",");
                                            }
                                        }
                                    }
                                    ouFiles.Write('\n');
                                }
                                ouFiles.Close();

                            }



                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(counterer.ToString() + "/" + dN.ToString());
                            counterer++;
                        }
                    });
                });

                //

                Console.ReadLine();
                Console.WriteLine("Готовы писать");
                StreamWriter ouFile = new StreamWriter("answer.csv");
                for (int i = 0; i < 677; i++)
                {
                    for (int j = 0; j < 677; j++)
                    {
                        if (i == 0)
                            ouFile.Write(j + ",");
                        else
                        {
                            if (j == 0)
                            {
                                ouFile.Write(i + ",");
                            }
                            else
                            {
                                ouFile.Write(data[i][j] + ",");
                            }
                        }
                    }
                    ouFile.Write('\n');
                }

                ouFile.Close();

                Console.WriteLine("Написали");
                Console.ReadLine();
            }

        }

    }
}
