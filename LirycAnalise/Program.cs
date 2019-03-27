using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace LirycAnalise
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input 1 or 2");
            int a;
            a = Console.Read();
            if(a == 49)
            {
                getTextFromURL();
            }
            if(a == 50)
            {
                analiseSongs();
            }
            Console.ReadKey();
        }

        static void analiseSongs()
        {
            int iterator = 1;
            int iterator2 = 1;
            int countWords = 0;
            int includeSong = 0;
            float sumInvolveProc = 0;
            Dictionary<string, int> lyrics = new Dictionary<string, int>();
            Dictionary<string, int> artistSong = new Dictionary<string, int>();
            List<string> titles = new List<string>();
            List<Song> songs = new List<Song>();

            string title = "";
            string artist = "";
            string text = "";
            string[] textTab;
            string[] words2;
            string[] name;

            string[] fileEntries = Directory.GetFiles("data/lyrics");
            foreach (string fileName in fileEntries)
            {
                name = fileName.Split('_');
                artist = name[0].Substring(12, name[0].Length - 12);
                title = name[1].Substring(0, name[1].Length-4);
                textTab = File.ReadAllLines(fileName);
                text = File.ReadAllText(fileName);

                text = text.ToLower();
                text = text.Replace(".", " ").Replace(",", " ").Replace("?", " ").Replace("!", " ").Replace("ref:", " ");
                text = text.Replace(Environment.NewLine, "");
                text = text.Replace("\n", "");
                text = text.Replace("\r", "");
                text = text.Replace("\r\n", "");
                while (text.IndexOf("  ") != -1)
                {
                    text = text.Replace("  ", " ");
                }

                titles.Add(title);

                if (!artistSong.ContainsKey(artist))
                {
                    artistSong.Add(artist, 1);
                }
                else
                {
                    artistSong[artist]++;
                }


                Song song = new Song();

                song.artist = artist;
                song.title = title;
                song.lines = textTab.Length;
                song.verse = text.Split('_').Length - 1;

                text = text.Replace("_", "");

                words2 = text.Split(' ');
                int count = 0;
                for (int i = 0; i < words2.Length; i++)
                {
                    if (words2[i] != "")
                    {
                        count++;
                    }
                }
                countWords += count;
                song.word = count;

                string[] words = new string[count];
                count = 0;
                for (int i = 0; i < words2.Length; i++)
                {
                    if (words2[i] != "")
                    {
                        words[count++] = words2[i];
                    }
                }

                foreach (string item in words)
                {
                    if (!lyrics.ContainsKey(item))
                    {
                        lyrics.Add(item, 1);
                    }
                    else
                    {
                        lyrics[item]++;
                    }

                    if (!song.lyrics.ContainsKey(item))
                    {
                        song.lyrics.Add(item, 1);
                    }
                    else
                    {
                        song.lyrics[item]++;
                    }
                }
                songs.Add(song);
                Console.WriteLine(iterator + "/" + fileEntries.Length);
                iterator++;
            }
            Console.WriteLine("Write results to output file");

            float proc, v1, v2;

            string output = "";
            string output2 = "";
            string output3 = "";
            string output4 = "";

            output += "Count song: " + titles.Count + Environment.NewLine;
            output += "Count Artist: " + artistSong.Count + Environment.NewLine;
            output += "Count all Words: " + countWords + Environment.NewLine;
            output += "Unique words: " + lyrics.Count + Environment.NewLine;
            v1 = (float)lyrics.Count;
            v2 = (float)countWords;
            proc = v1 / v2;
            output += "Unique lyrics: " + proc * 100 + "%" + Environment.NewLine;
            output += "Repeated lyrics: " + (100 - proc * 100) + "%" + Environment.NewLine;
            output += "Average words per song: " + (float)countWords / (float)titles.Count + Environment.NewLine;
            output += "Average unique words per song: " + (float)lyrics.Count / (float)titles.Count + Environment.NewLine;

            output2 += "Most repeted words" + Environment.NewLine;
            iterator = 1;
            foreach (var item in lyrics.OrderByDescending(i => i.Value))
            {
                if (iterator > 30) break;
                output2 += iterator + ") " + item.Key + " - " + item.Value + Environment.NewLine;
                iterator++;
            }

            output2 += "Song with most repeated words: " + Environment.NewLine;

            Song tmpSong = new Song();
            for (int i = 0; i < songs.Count; i++)
            {
                for (int k = 0; k < songs.Count - 1; k++)
                {
                    if (((float)songs[k].lyrics.Count / songs[k].word) > ((float)songs[k + 1].lyrics.Count / songs[k + 1].word))
                    {
                        tmpSong = songs[k];
                        songs[k] = songs[k + 1];
                        songs[k + 1] = tmpSong;
                    }
                }
            }

            Dictionary<string, float> xdArtist = new Dictionary<string, float>();

            iterator = 1;
            foreach (Song item in songs)
            {
                output2 += iterator + ")" + Environment.NewLine;
                output2 += "Artist: " + item.artist + Environment.NewLine;
                output2 += "Title: " + item.title + Environment.NewLine;
                output2 += "All words: " + item.word + Environment.NewLine;
                output2 += "Unique words: " + item.lyrics.Count + Environment.NewLine;
                proc = ((float)item.lyrics.Count / item.word);

                if (!xdArtist.ContainsKey(item.artist))
                {
                    xdArtist.Add(item.artist, proc);
                }
                else
                {
                    xdArtist[item.artist] += proc;
                    xdArtist[item.artist] /= 2;
                }

                output2 += "Unique lyrics: " + proc * 100 + "%" + Environment.NewLine;
                output2 += "Repeated lyrics: " + (100 - proc * 100) + "%" + Environment.NewLine;
                output2 += "Most repeated words in this song: " + Environment.NewLine;

                int includeWord = 0;
                foreach(var checkInclude in item.lyrics)
                {
                    if (lyrics.ContainsKey(checkInclude.Key) && lyrics[checkInclude.Key] > 1)
                    {
                        includeWord++;
                    }
                }
                if(includeWord == item.lyrics.Count)
                {
                    includeSong++;
                }
                proc = 100 * (float)includeWord / item.lyrics.Count;
                sumInvolveProc += proc;
                output2 += "Word used in other song/per all unie wordd in song: " + includeWord + "/" + item.lyrics.Count + " ( " + proc + "% )" + Environment.NewLine;

                iterator2 = 1;
                foreach (var sybItem in item.lyrics.OrderByDescending(i => i.Value))
                {
                    if (iterator2 > 10) break;
                    output2 += iterator2 + "] " + sybItem.Key + " - " + sybItem.Value + Environment.NewLine;
                    iterator2++;
                }
                output2 += Environment.NewLine;
                iterator++;
            }

            output += "Most repeated artist: " + Environment.NewLine;
            iterator = 1;
            foreach (var item in xdArtist.OrderBy(i => i.Value))
            {
                if (iterator > 10) break;
                output += iterator + "] " + item.Key + " - " + (100 - item.Value * 100) + "%" + Environment.NewLine;
                iterator++;
            }

            output += "How song's word all involve in other song's word: " + includeSong + "/" + songs.Count + " ( " + 100*(float)includeSong / songs.Count + "% )" + Environment.NewLine;
            output += "Average worsd involve in other song: " + (float)sumInvolveProc / songs.Count + "%" + Environment.NewLine;

            output += output2;

            File.WriteAllText("data/output.txt", output);
            Console.WriteLine("End");
        }

        static void getTextFromURL()
        {
            string html = string.Empty;
            string[] urls = File.ReadAllLines("data/urls.txt");
            int index;
            char tmp;
            int index2;
            string copyHtml = "";
            string artist = "";
            string title = "";
            string lyrics = "";
            int iterator = 1;

            foreach (string url in urls)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
                
                copyHtml = html;
                index = html.IndexOf("song-text");
                html = html.Substring(index + 114, html.Length - index - 114);
                index = html.IndexOf("<p>&nbsp;</p>");
                html = html.Substring(0, index - 24);

                html = html.Replace("<br />"+Environment.NewLine+"<br />", " _").Replace("<br />\n<br />", " _").Replace("<br />","");
                html = html.Replace("x2", "(2x)").Replace("x3", "(3x)").Replace("x4", "(4x)");
                while(html.IndexOf("x)") != -1)
                {
                    index = html.IndexOf("x)");
                    tmp = html[index - 1];
                    index2 = index;
                    while (index2 > 0 && html[index2--] != '_');
                    for(int i=int.Parse(tmp.ToString()); i>1; i--)
                    {
                        html += html.Substring(index2, index-index2-4);
                    }
                    var regex = new Regex(Regex.Escape("(" + tmp + "x)"));
                    html = regex.Replace(html, "", 1);
                }

                //html = html.Replace(" _", "");

                /*while(html.IndexOf(Environment.NewLine) != -1)
                {
                    html = html.Replace(Environment.NewLine, " ");
                }*/

                while (html.IndexOf("  ") != -1)
                {
                    html = html.Replace("  ", " ");
                }

                html = html.Replace(",", "");
                html = html.Replace("-", "");

                for (int i=1; i<20; i++)
                {
                    html = html.Replace(i+".", "");
                }

                index = copyHtml.IndexOf("<title>");
                copyHtml = copyHtml.Substring(index+7, copyHtml.Length-index-7);
                index = copyHtml.IndexOf(" - ");
                artist = copyHtml.Substring(0, index);
                copyHtml = copyHtml.Substring(index + 3, copyHtml.Length - index - 3);
                index = copyHtml.IndexOf(" - ");
                title = copyHtml.Substring(0, index);

                lyrics = html;
                File.WriteAllText("data/lyrics/"+artist+"_"+title+".txt", lyrics.Replace("\n", Environment.NewLine));
                Console.WriteLine("Artist: "+artist);
                Console.WriteLine("Title: "+title);
                Console.WriteLine("lyrics: "+lyrics);
                Console.WriteLine(iterator+"/"+urls.Length);
                Console.WriteLine("--------------------------------------");
                iterator++;
            }

            
        }

    }
}
