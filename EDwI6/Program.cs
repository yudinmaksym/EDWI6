using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene;
using System.Text.RegularExpressions;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Lucene.Net.Messages;
using Lucene.Net.Spatial;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using System.IO;

using static Lucene.Net.Search.SimpleFacetedSearch;
using static Lucene.Net.Search.Searcher;
using static Lucene.Net.Search.Query;

namespace EDwI5
{
    class Program
    {

        static void Main(string[] args)
        {



            string alltext="";
            int counter = 0;
            DirectoryInfo dinfo = new DirectoryInfo(@"E:\Downloads\books");
            var fs = FSDirectory.Open("E:\\Downloads\\books");
            FileInfo[] Files = dinfo.GetFiles("*.txt", SearchOption.AllDirectories);
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            IndexWriter wr = new IndexWriter(fs, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);

            foreach (FileInfo file in Files)
            {
                var keyword = "Title:";
                string title="";
                var list = new List<string>();
                using (var sr = new StreamReader(file.DirectoryName + "\\" + file))
                {
                    
                    
                    while (!sr.EndOfStream)
                    {
                        
                        
                        string line = sr.ReadLine();


                        if (String.IsNullOrEmpty(line)) continue;
                        if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            title = line;
                        }

                            if (line.Contains("End of Project Gutenberg") || line.Contains("End of the Project Gutenberg"))
                            {
                              
                                
                                 
                                for(int i =27; i<list.Count - 1; i++)
                                {
                                    //Console.WriteLine(list[i]);
                                    
                                }

                                StringBuilder builder = new StringBuilder();
                                foreach (string lines in list) // Loop through all strings
                                {
                                    builder.Append(lines).Append(" "); // Append string to StringBuilder
                                }
                                string result = builder.ToString();
                                var doc = new Document();
                                counter++;
                               
                                doc.Add(new Field("id", counter.ToString(), Field.Store.YES, Field.Index.NO));
                                doc.Add(new Field("Title", title, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.NO));
                                doc.Add(new Field("Main", result, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.NO));
                                wr.AddDocument(doc);
                            Console.WriteLine(counter);


                        }

                        list.Add(line);

                       

                    }
                }
                }
            
            wr.Optimize();
            wr.Commit();
            wr.Dispose();


        metka:


            //var reader = wr.GetReader();
            //var searcher = new IndexSearcher(reader);
            Console.WriteLine("Input ur title:");
            string yourinput = Console.ReadLine();

            var mydirectory = FSDirectory.Open(new DirectoryInfo(@"E:\Downloads\books"));
            Analyzer myanalyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { "Title","Main" }, myanalyzer); // (1)
            Query query = parser.Parse(yourinput);
            var searcher = new IndexSearcher(mydirectory, true);

            TopDocs topDocs = searcher.Search(query, 10);
            int results = topDocs.ScoreDocs.Length;
            for (int i = 0; i < results; i++)
            {
                ScoreDoc scoreDoc = topDocs.ScoreDocs[i];
                float score = scoreDoc.Score;
                int docId = scoreDoc.Doc;
                Document doc = searcher.Doc(docId);
                Console.WriteLine("ID:" + doc.Get("id") + " " + doc.Get("Title"));


            }

            goto metka;

        }


    }




}

