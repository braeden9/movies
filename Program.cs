using System;
using System.IO;
using NLog.Web;
using System.Collections.Generic;
using System.Linq;

namespace mod4movie
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory() + "\\nlog.config";
            var logger = NLog.Web.NLogBuilder.ConfigureNLog(path).GetCurrentClassLogger();
            var file = "movies.csv";

            logger.Info("Program started");

            if (!File.Exists(file)) {
                logger.Error($"File not found: {file}");
            } else {
                List<int> ids = new List<int>();
                List<string> titles = new List<string>();
                List<string> genres = new List<string>();
                try {                    
                    StreamReader sr = new StreamReader(file);
                    sr.ReadLine();
                    while (!sr.EndOfStream) {
                        string line = sr.ReadLine();
                        if (!line.Contains('"')) {
                            string[] parts = line.Split(',');
                            ids.Add(int.Parse(parts[0]));
                            titles.Add(parts[1]);
                            genres.Add(parts[2].Replace("|", "& "));
                        } else {
                            string[] parts = line.Split(',');
                            ids.Add(int.Parse(parts[0]));
                            string title = "";
                            for(int i = 1; i < parts.Length; i++) {
                                title += parts[i];
                                if(i != parts.Length - 1) {
                                    title += " ";
                                }
                            }
                            titles.Add(title);
                            genres.Add(parts[parts.Length-1].Replace("|", "& "));
                        }
                    }
                    sr.Close();
                } catch (Exception ex) {
                    logger.Error(ex.Message);
                }

                string resp;
                do {
                    Console.WriteLine("Enter 1 to read movie file.");
                    Console.WriteLine("Enter 2 to add a movie.");
                    Console.WriteLine("Enter anything else to quit.");

                    resp = Console.ReadLine();
                    if (resp == "1") {
                        for (int i = 0; i < ids.Count; i++) {
                            Console.WriteLine($"Index: {ids[i]} Title: {titles[i]} Genre(s): {genres[i]}");
                        }
                    }
                    else if (resp == "2") {
                        // TODO: add data file
                        Console.WriteLine("Enter the title of movie:");
                        string title = Console.ReadLine();
                        if(titles.Contains(title)) {
                            logger.Info($"Duplicate movie found: {title}");
                        } else {
                            int id = ids.Max() + 1;
                            List<string> genre = new List<string>();
                            string inputGenre;
                            do {
                                Console.WriteLine("Enter genre of movie (done to finish)");
                                inputGenre = Console.ReadLine();
                                if (inputGenre != "done") {
                                    genre.Add(inputGenre);
                                }
                            } while (inputGenre != "done");
                            string genresString = string.Join("|", genre);
                            if(title.Contains(',')){
                                title = '"' + title + '"';
                            }
                            StreamWriter sw = new StreamWriter(file, true);
                            sw.WriteLine($"{id},{title},{genresString}");
                            sw.Close();
                        }
                    } else {
                        Console.WriteLine("Goodbye.");
                    }
                } while (resp == "1" || resp == "2");
                logger.Info("Program ended");
            }
        }
    }
}
