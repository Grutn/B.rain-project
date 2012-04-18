using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BandMaster.State
{
    public class Song
    {
        public String[] Instruments;
        public List<int[]> Lines;
        int size;

        public Song()
        {
            Lines = new List<int[]>();
        }
        public Song(String addres)
        {
            Lines = new List<int[]>();
            LoadSong(addres);

        }

        public void LoadSong(String addres)
        {
            using (StreamReader file = new StreamReader("Content/" + addres))
            {
                char[] delimiter = { ' ', ':' };
                file.ReadLine();
                file.ReadLine();
                Instruments = file.ReadLine().Split(delimiter);
                int[] temp;
                string[] columns;
                string column;
                column = file.ReadLine();
                size = Convert.ToInt32(column);
                for (int i = 0; i < Instruments.Length; i++)
                {
                    int[] lineArray = new int[size];
                    Lines.Add(lineArray);
                }
                int[] pre = new int[Instruments.Length];

                for (int d = 0; d < Instruments.Length; d++)
                {
                    pre[d] = 0;
                }

                while ((column = file.ReadLine()) != null)
                {
                    columns = column.Split(delimiter);
                    temp = Lines.ElementAt(Convert.ToInt32(columns[1]));
                    for (int r = pre[Convert.ToInt32(columns[1])]; r < Convert.ToInt32(columns[0]); r++)
                    {
                        temp[r] = temp[pre[Convert.ToInt32(columns[1])]];
                    }
                    temp[Convert.ToInt32(columns[0])] = Convert.ToInt32(columns[2]);
                    pre[Convert.ToInt32(columns[1])] = Convert.ToInt32(columns[0]);

                }
                for (int i = 0; i < Instruments.Length; i++)
                {
                    temp = Lines.ElementAt(i);
                    for (int d = pre[i]; d < size; d++)
                    {
                        temp[d] = temp[pre[i]];
                    }
                }
            }
        }
    }
}
