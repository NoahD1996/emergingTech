using System;
using System.Collections.Generic;
using System.IO;

namespace Project1
{

    /*
    Sentences are seperated into engrams individually
    then converted into ngram of level 2-4(2-4 words)
    then engrams are matched with definitions

    step 1 read in files and 
    */
    static class Program
    {
        struct Ngram{
           public string noun;
           public string definition;
        }
        private static IDictionary<string, string> dataDict = new Dictionary<string, string>();
        private static IDictionary<string, string> indexDict = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            if(args.Length < 1){
                Console.WriteLine("Please specify filename in the command args");
                return;
            }
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string directory = Path.GetDirectoryName(path);

            string nounIndex = directory + @"\data\NounsIndex.txt";
            string nounData = directory + @"\data\NounsData.txt";
            
            LoadFile(nounData, ref dataDict);
            LoadFile(nounIndex,ref indexDict);
            LoadFileaAnNgrams(args[0]);
        }
        private static void LoadFile(string indexFile,ref IDictionary<string, string> retDict)
        {
            StreamReader input = new StreamReader(indexFile);
            string inLine;

            while ((inLine = input.ReadLine()) != null)
            {
                inLine = inLine.TrimEnd();
                string[] argArray = inLine.Split('|');
                if (!retDict.ContainsKey(argArray[0]))
                {
                    retDict.Add(argArray[0], argArray[1]);
                }
                
                    
            }
            input.Close();
        }
        private static void LoadFileaAnNgrams(string filename){
            StreamReader input = new StreamReader(filename);
            StreamWriter output = new StreamWriter("./debug.txt");
            string inLine;
            inLine = input.ReadLine();
            if(inLine != null)
            {
                output.WriteLine(inLine + "\n");
                string[] sentences = inLine.Split(". ");
                foreach(string sentence in sentences)
                {
                    output.WriteLine(sentence + "\n");
                    string[] words = sentence.Split(" ");                    
                    //size of ngram
                    for(int i = 2; i <= 4; ++i)
                    {
                        output.WriteLine(i + " level n-gram\n");
                        for(int x = i - 1; x < words.Length; x++)
                        {  
                            int cycles = i - 1;
                            Ngram ngram = new Ngram{noun = "", definition = ""};
                            do{
                                ngram.noun += cycles == 0
                                    ? words[x]
                                    : words[x - cycles] + '_';
                                    cycles--;
                            }while(cycles >= 0);
                            //remove any period from ngram(only happens on last sentence)
                            if(ngram.noun.Contains('.')){
                                ngram.noun = ngram.noun.Remove(ngram.noun.Length - 1);
                            }
                            //replace this with writing to file and matching indexes
                            if(indexDict.ContainsKey(ngram.noun.ToLower()))
                            {                                
                                string key = indexDict[ngram.noun.ToLower()];
                                string definition = dataDict[key];
                                string[] splitdefinition = definition.Split("; ");
                                if(splitdefinition.Length == 1){
                                    ngram.definition += splitdefinition[0];
                                }
                                else
                                    foreach(string def in splitdefinition)
                                    {
                                        ngram.definition += def + " << and >> ";
                                    }
                            }
                            if(x == words.Length - 1)
                                output.WriteLine(ngram.noun + ", " + ngram.definition + "\n");
                            else
                                output.WriteLine(ngram.noun + ", " + ngram.definition);
                        }
                        
                    }
                }
            }
            input.Close();
            output.Close();
        }
    }
}
