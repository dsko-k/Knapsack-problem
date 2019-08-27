using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using System.IO;

/*
Программа позволяет решить т.н. задачу о максимальной загрузке (knapsack problem): 
отыскать такой набор предметов из перечня, который помещается в автомобиль, при условии, что: 

1.	Выбранный набор предметов не превышает грузоподъемность автомобиля

2.	Выбранный набор предметов имеет максимальную суммарную стоимость, с учётом 
    ограничения на грузоподъемность автомобиля
*/

namespace Knapsack_problem
{
    /// <summary>
    /// Класс, описывающий характеристики предмета: название, вес, цена
    /// </summary>
    class Subject
    {
        public Subject(string name, int weight, int price)
        {
            Name = name;
            Weight = weight;
            Price = price;
        }

        public string Name { get; set; }

        public int Weight { get; set; }

        public int Price { get; set; }
    }


    /// <summary>
    /// Абстрактный класс с методом для получения массива из всех комбинаций предметов, где 1 и 0 означают:
    /// "1" - i-й предмет включается в набор,  "0" - i-й предмет не включается в набор
    /// </summary>
    abstract class AbstractKnapsack
    {
        public AbstractKnapsack(int maxWeight)
        {
            MaxWeight = maxWeight;
        }

        /// <summary>
        /// Свойство для задания максимально допустимого веса набора предметов
        /// </summary>
        public int MaxWeight { get; set; }

        /// <summary>
        /// Абстрактный метод для получения массива из всех комбинаций предметов, где 1 и 0 означают:
        /// "1" - i-й предмет включается в набор,  "0" - i-й предмет не включается в набор
        /// </summary>
        /// <param name="length">Количество предметов в файле</param>
        /// <returns>Массив строк в виде комбинаций "0" и "1"</returns>
        public abstract string[] AllCombinations(int length);
    }


    /// <summary>
    /// Конкретный класс, реализующий интерфейс абстрактного класса AbstractKnapsack
    /// </summary>
    class Knapsack : AbstractKnapsack
    {
        Data data;
        Subject[] subjects;

        public Knapsack(int maxWeight)
            : base(maxWeight)
        {
            data = new Data();

            subjects = ConvertToSubject();
        }

        /// <summary>
        /// Получить массив типа Subject[] из данных файла со сведениями о предметах
        /// </summary>
        /// <returns>Массив типа Subject[] </returns>
        public Subject[] ConvertToSubject()
        {
            string [] fileData = data.ReadFile();

            List<Subject> listSubject = new List<Subject>();

            foreach (var item in fileData)
            {
                string[] lineFileData = item.Split('\t');

                try
                {
                    Subject sbjct = new Subject(lineFileData[0], int.Parse(lineFileData[1]), int.Parse(lineFileData[2]));

                    listSubject.Add(sbjct);
                }
                catch (Exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nПроверьте корректность файла Subjects.txt, со сведениями о предметах!\n");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                
            }

            return listSubject.ToArray();
        }


        /// <summary>
        /// Реализация метода-обёртки для получения массива из всех комбинаций предметов, где 1 и 0 означают:
        /// "1" - i-й предмет включается в набор,  "0" - i-й предмет не включается в набор
        /// </summary>
        /// <param name="length">Количество предметов в файле</param>
        /// <returns>Массив строк в виде комбинаций "0" и "1"</returns>
        public override string[] AllCombinations(int length)
        {
            string[] init =
                           {
                              ""
                           };


            string[] result = RecursCombinations(init, length);


            return result;
        }


        /// <summary>
        /// Получение массива всех комбинаций из "0" и "1", где 
        /// "1" - i-й предмет включается в набор,  "0" - i-й предмет не включается в набор
        /// </summary>
        /// <param name="previous">Массив из "0" и "1", полученный в предыдущих итерациях</param>
        /// <param name="length">Счетчик итераций вызова рекурсивного метода</param>
        /// <returns>Массив из "0" и "1"</returns>
        string[] RecursCombinations(string[] previous, int length)
        {
            if (length == 0)
            {
                //Show(previous);

                return previous;
            }
            else
            {
                List<string> result = new List<string>();

                for (int i = 0; i < previous.Length; i++)
                {
                    
                    result.Add(previous[i] + "1"); //  "1" - предмет берётся в набор
                    result.Add(previous[i] + "0"); //  "0" - предмет не берётся в набор
                }
                
                length--;

                return RecursCombinations(result.ToArray(), length);
            }

        }


        /// <summary>
        /// Суммарная стоимость и вес взятых предметов каждой комбинации
        /// </summary>
        /// <returns>Массив строк в формате: комбинация\tсуммарный вес\tсуммарная стоимость</returns>
        public string[] Sum()
        {
            string[] combinations = AllCombinations(subjects.Length);

            string pattern = "([0-1])";

            Regex regex = new Regex(pattern);
            

            for (int i = 0; i < combinations.Length; i++)
            {
                MatchCollection match = regex.Matches(combinations[i]);

                int sumWeight = 0;
                int sumPrice = 0;

                foreach (Match item in match)
                {
                    if (item.Groups[1].Value == "1")
                    {
                        sumPrice = sumPrice + subjects[item.Groups[1].Index].Price;

                        sumWeight = sumWeight + subjects[item.Groups[1].Index].Weight;
                    }
                }

                combinations[i] = combinations[i] + '\t' + sumWeight+ '\t' + sumPrice;

            }
            
            //Show(combinations);


            return combinations;
        }


               
        /// <summary>
        /// Найти комбинацию предметов с максимальной суммарной стоимостью и не превышающей грузоподъемность MaxWeight
        /// </summary>
        public void MaxAmount()
        {
            string[] combinations = Sum();

            var query = combinations.Select(a => {
                                                    string[] s = a.Split('\t');

                                                    string subj = "";

                                                    int counter = 0;

                                                    foreach (char digit in s[0])
                                                    {

                                                        if (digit == '1')
                                                        {
                                                            subj = subj + subjects[counter].Name + " ";
                                                        }

                                                        counter++;
                                                    }

                                                    return new
                                                    {
                                                        Combination = s[0],
                                                        Subjects = subj,
                                                        SumWeight = int.Parse(s[1]),
                                                        SumPrice = int.Parse(s[2]),
                                                    };
                                                 }
                                            
                                           );


            Console.WriteLine("\n\t\tВсе комбинации из наборов предметов:\n");

            foreach (var item in query)
            {
                Console.WriteLine("{0}\n", item);
            }

            var maxPrice = query
                           .Where(a => a.SumWeight <= MaxWeight)                           
                           .Max(b => b.SumPrice);



            var result = query
                         .Where(a => a.SumPrice == maxPrice)
                         .Select(a => a);

            Console.WriteLine(new string('-', 30));


            StringBuilder builder = new StringBuilder();

            
            Console.WriteLine("\n\t\tДанные о наборе предметов с максимальной стоимостью:\n");
            builder.AppendLine(String.Format("{0}\t\tДанные о наборе предметов с максимальной стоимостью:{0}", Environment.NewLine));


            foreach (var item in result)
            {
                Console.WriteLine(String.Format("{0}", item));
                builder.AppendLine(String.Format("Предметы: {0}\tСтоимость: {1}\tВес: {2}", item.Subjects, item.SumPrice, item.SumWeight));
            }

            data.WriteResult(builder.ToString());
        }


        /// <summary>
        /// Показать сожержимое массива
        /// </summary>
        /// <param name="array">Массив</param>
        void Show(string [] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.WriteLine(" array[{0}] = {1}", i, array[i]);
            }

            Console.WriteLine(new string('-', 80));
        }

    }


    /// <summary>
    /// Класс для работы с файлами (чтение файла с данными о предметах, запись результата в файл)
    /// </summary>
    class Data
    {
        static string pathDirectory;
        
        static Data()
        {
            pathDirectory = new DirectoryInfo(@".").FullName;
        }

        /// <summary>
        /// Найти путь к папке, в которой находится проект
        /// </summary>
        /// <returns>Строка, содержащая путь проекта</returns>
        public string ProjectPath()
        {

            string pattern = @"(\\)";

            string projectPath = pathDirectory;

            Regex regex = new Regex(pattern);

            Match match = regex.Match(pathDirectory);

            List<int> listIndexes = new List<int>();

            
            int index = 0;
            int counter = 0;

            while (match.Success)
            {
                index = match.Index;
                listIndexes.Add(index);

                match = match.NextMatch();
                counter++;
            }

            index = listIndexes[listIndexes.Count - 3];

            projectPath = projectPath.Remove(index);

            //Console.WriteLine("result = {0}", result);


            return projectPath;
        }


        /// <summary>
        /// Прочитать файл со сведениями о предметах
        /// </summary>
        /// <returns>Массив со сведениями о предметах</returns>
        public string[] ReadFile()
        {
            string pathFile = Path.Combine(ProjectPath(), "Subjects.txt");
            
            string[] result = File.ReadAllLines(pathFile, Encoding.Default);

            result = result.Skip(1).ToArray();


            return result;
        }

                
        /// <summary>
        /// Записать файл с результатами поиска набора с максимальной стоимостью и весом, не превышающим допустимый вес
        /// </summary>
        /// <param name="text">Текст для записи в файл</param>
        public void WriteResult(string text)
        {
            MemoryStream memory = new MemoryStream();
            BufferedStream buffer = new BufferedStream(memory);
            StreamWriter writer = new StreamWriter(buffer, Encoding.Default);
            
            writer.Write(text);
            writer.Flush();
            
            string pathFile = Path.Combine(ProjectPath(), "Result.txt");

            FileStream file = File.Open(pathFile, FileMode.Create, FileAccess.ReadWrite);

            memory.WriteTo(file);

            FileInfo fileInfo = new FileInfo(pathFile);

            memory.Close();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nФайл {0} записан!\n Путь к нему: \n\n{1}", fileInfo.Name, fileInfo.FullName);
            Console.ForegroundColor = ConsoleColor.Gray;

            file.Close();
            writer.Close();
            buffer.Close();
        }

    }

    class Program
    {
        static void Main(string[] args)
        {

            Knapsack knapsack = new Knapsack(7);

            


            knapsack.MaxAmount();

            



            Console.ReadKey();
        }
    }
}
